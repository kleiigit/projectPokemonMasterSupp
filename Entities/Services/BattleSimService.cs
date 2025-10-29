
using DocumentFormat.OpenXml.Drawing;
using ProjetoPokemon.Entities.Data;
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Profiles;
using ProjetoPokemon.Services;
using System.Globalization;

namespace ProjetoPokemon.Entities.Services
{
    static class BattleSimService // classe que controla as simulações de combate
    {
        private static readonly Move nullMove = new Move(0, TypePokemon.None, "No Move", 0, 6);
        private static readonly ConsoleColor atkColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor defColor = ConsoleColor.Magenta;

        public static void BattleWildPokemon(BoxPokemon profile)
        {
            BattlerManager wildPokemonMod = new BattlerManager(WildGeneratorService.WildByColor(ColorToken.Blue), SetupBattle.Defender);
            Console.WriteLine($"Um {wildPokemonMod.SelectedPokemon.Name} selvagem apareceu! \n{wildPokemonMod.SelectedPokemon}");
            Console.ReadLine();
            do { 
            BattlerManager playerPokemonMod = BattlerManager.CreateBattlerTrainer(profile, atkColor, SetupBattle.Attacker);
                BattleSetup(playerPokemonMod, wildPokemonMod);
            } while (profile.ListPokemon.Any(p => p.Conditions != StatusConditions.KNOCKED) && wildPokemonMod.SelectedPokemon.Conditions != StatusConditions.KNOCKED);
            if(ConsoleMenu.ShowYesNo("Do you want catch " + wildPokemonMod.SelectedPokemon.Name + "? - Catch Rate: +"+ wildPokemonMod.SelectedPokemon.Pokemon.CathRate))
            {

                ItemCard bonusCard = profile.SelectItem(TypeItemCard.Catch);
                int bonus = 1;
                if (bonusCard != null)
                {
                    Console.WriteLine($"\n{profile.Nickname} used the item card {bonusCard.Name}!");
                }

                if (WildGeneratorService.CatchWildPokemon(wildPokemonMod.SelectedPokemon.Pokemon.CathRate, bonus))
                {
                    wildPokemonMod.SelectedPokemon.NickNamePokemon();
                    profile.AddPokemon(new ProfilePokemon(wildPokemonMod.SelectedPokemon.Pokemon, wildPokemonMod.SelectedPokemon.Name, 0));
                }
            }
        }
        public static void BattleTrainerPVP()
        {
            // Select Profiles
            int index = ConsoleMenu.ShowMenu(atkColor, DataLists.AllProfiles.Select(m => m.Nickname).ToList(), "Choose a Attacker trainer Profile");
            BoxPokemon ProfileA = DataLists.AllProfiles[index]; // Attacker Profile
            index = ConsoleMenu.ShowMenu(defColor, DataLists.AllProfiles.Select(m => m.Nickname).ToList(), "Choose a Defender trainer Profile");
            BoxPokemon ProfileB = DataLists.AllProfiles[index]; // Defender Profile

            // Choose Pokemon Attacker Pokemon
            BattlerManager profileAtk = BattlerManager.CreateBattlerTrainer(ProfileA, atkColor, SetupBattle.Attacker);

            // Choose Pokemon Defender Pokemon
            BattlerManager profileDef = BattlerManager.CreateBattlerTrainer(ProfileB, defColor, SetupBattle.Defender);

            do {
                if (profileAtk.SelectedPokemon.Conditions == StatusConditions.KNOCKED) profileAtk = BattlerManager.CreateBattlerTrainer(ProfileA, atkColor, SetupBattle.Attacker);
                if (profileDef.SelectedPokemon.Conditions == StatusConditions.KNOCKED) profileDef = BattlerManager.CreateBattlerTrainer(ProfileB, defColor, SetupBattle.Defender);
                BattleSetup(profileAtk, profileDef);

                } while (ProfileA.ListPokemon.Any(p => p.Conditions != StatusConditions.KNOCKED) && ProfileB.ListPokemon.Any(p => p.Conditions != StatusConditions.KNOCKED));
                ProfileA.RecoverPokémon();
                ProfileB.RecoverPokémon();
        }
 
        private static void BattleSetup(BattlerManager attacker, BattlerManager defender)
        {
            attacker.NumberDices = 0;
            defender.NumberDices = 0;
            string textMoveUsed = "";
            // move half level effect
            ApplyHalfLevelEffect(attacker, defender);
            ApplyHalfLevelEffect(defender, attacker);

            // move selection
            if (MoveStatusCheck(attacker))
            {
                    attacker.UsedMove = attacker.SelectedPokemon.SelectMovePokemon(attacker, defender);
                    textMoveUsed = $"=> {attacker} used {attacker.UsedMove.Name}.\n" + attacker.UsedMove + "\n";
            }

            if (MoveStatusCheck(defender))
            {
                if (defender.TrainerName == null)
                {
                    defender.UsedMove = defender.SelectedPokemon.RandomMovePokemon();
                    BattleLog.AddLog(textMoveUsed+$"=> {defender} used {defender.UsedMove.Name}.\n" + defender.UsedMove + "\n");
                }
                else
                {
                    defender.UsedMove = defender.SelectedPokemon.SelectMovePokemon(defender, attacker);
                    BattleLog.AddLog(textMoveUsed + $"=> {defender} used {defender.UsedMove.Name}.\n" + defender.UsedMove + "\n");
                }

            }

            // card selection (somente se forem PlayerBattler)
            if (attacker.TrainerBox != null)
            {
                attacker.UsedCard = attacker.TrainerBox.SelectItem(TypeItemCard.Battle);
                if (attacker.UsedCard != null)
                {
                    BattleLog.AddLog($"\n{attacker.TrainerName} used the item card {attacker.UsedCard.Name}!");
                    EffectManager.CardEffectBattle(attacker.UsedCard.BattleCard(), attacker);
                }
            }
            if (defender.TrainerBox != null)
            {
                if (defender is BattlerManager playerD)
                {
                    playerD.UsedCard = playerD.TrainerBox.SelectItem(TypeItemCard.Battle);
                    if (playerD.UsedCard != null)
                    {
                        BattleLog.AddLog($"{playerD.TrainerName} used the item card {playerD.UsedCard.Name}!");
                        EffectManager.CardEffectBattle(playerD.UsedCard.BattleCard(), playerD);
                    }
                }
            }

            EffectMove(attacker, defender);
            EffectMove(defender, attacker);

            BattleLog.ShowLogs();
            BattleLog.ClearLogs();

            do
            {
                BattleLog.AddLog("\n==== ROLLING DICES ====");
                BattleCalculation(attacker, defender.SelectedPokemon.Pokemon);
                BattleCalculation(defender, attacker.SelectedPokemon.Pokemon);

                // battle log
                BattleLog.AddLog("\n==== RESULT COMBAT ====");
                attacker.DisplayBattleStatus();
                defender.DisplayBattleStatus();
                BattleLog.ShowLogs();
                VictoryChanceService.ShowResult();

                if (attacker.TotalResult == defender.TotalResult)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Tie! Roll the dice again.\n");
                    Console.ResetColor();
                    Console.ReadLine();
                }

            } while (attacker.TotalResult == defender.TotalResult && defender.TrainerName != null);

            ResultBattle(attacker, defender);

            BattleLog.ClearLogs();
            Console.ReadLine();
        }
        public static void BattleCalculation(BattlerManager attacker, Pokemon defender)
        {
            attacker.Roll = RollCombatDices(attacker); // rolagem do dado do move, de acordo com o efeito
            attacker.EffectiveBonus = EffectiveTypeService.GetTypeModifier(attacker.UsedMove.Type, defender.Type); // effetivo de tipo
            attacker.StatusBonus = StatusPenalty(attacker); // penalidade de status

            attacker.WeatherBonus = FieldControlService.WeatherBonus(attacker); // efeito do clima
            if (!attacker.SelectedPokemon.CanAttack) attacker.UsedMove = nullMove;

            attacker.CalculateTotalPower();
        }
        private static void ResultBattle(BattlerManager attacker, BattlerManager defender)
        {
            ProfilePokemon pokemonAtk = attacker.SelectedPokemon;
            ProfilePokemon pokemonDef = defender.SelectedPokemon;

            string victoryPokemon = attacker.TotalResult >= defender.TotalResult ? attacker.ToString() : defender.ToString();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + victoryPokemon + " won the battle!\n");
            Console.ResetColor();

            // KNOCKED OUT, POISONED check and level up
            if (pokemonAtk.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(attacker);
            if (pokemonDef.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(defender);
            if (attacker.TotalResult > defender.TotalResult) defender.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            else if (attacker.TotalResult < defender.TotalResult) attacker.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            if (pokemonAtk.Conditions == StatusConditions.KNOCKED) Console.WriteLine(attacker + " has been knocked out!");
            if (pokemonDef.Conditions == StatusConditions.KNOCKED) Console.WriteLine(defender + " has been knocked out!");


            if (pokemonAtk.LevelPokemon() <= pokemonDef.LevelPokemon() && pokemonAtk.Conditions != StatusConditions.KNOCKED)
                pokemonAtk.LevelUpPokemon();
            if (pokemonDef.LevelPokemon() <= pokemonAtk.LevelPokemon() && pokemonDef.Conditions != StatusConditions.KNOCKED && defender.TrainerName != null)
                pokemonDef.LevelUpPokemon();
        }


        #region(Combat Methods)
        private static bool MoveStatusCheck(BattlerManager battler)
        {
            if (battler.SelectedPokemon.Conditions == StatusConditions.PARALYZED)
            {
                if (!BattleConditions.ParalyzedRoll())
                {
                    BattleLog.AddLog(battler + " is paralyzed and cannot attack!");
                    return false;
                }
            }
            else if (battler.SelectedPokemon.Conditions == StatusConditions.SLEEP)
            {
                battler.SelectedPokemon.ConditionCount--;
                if (battler.SelectedPokemon.ConditionCount == 0) { BattleLog.AddLog(battler + " woke up!"); battler.SelectedPokemon.Conditions = StatusConditions.NORMAL; return true; }
                else
                {
                    BattleLog.AddLog(battler + " is still sleeping...");
                    return false;
                }
            }
            else if (battler.SelectedPokemon.Conditions == StatusConditions.FROZEN)
            {
                if (BattleConditions.FrozenRoll())
                {
                    battler.SelectedPokemon.Conditions = StatusConditions.NORMAL;
                    BattleLog.AddLog(battler + " has been unfrozen and can attack!");
                    return true;
                }
                else BattleLog.AddLog(battler + " is still frozen."); return false;
            }
            return true;
        }
        private static int StatusPenalty(BattlerManager battler)
        {
            ProfilePokemon pokemon = battler.SelectedPokemon;
            if (pokemon.Conditions != StatusConditions.NORMAL)
            {
                StatusConditions status = pokemon.Conditions;
                string messageSts = pokemon.Name + " is ";

                switch (status)
                {
                    case StatusConditions.PARALYZED:
                        BattleLog.AddLog(messageSts + "paralyzed!");
                        break;
                    case StatusConditions.FROZEN:
                        BattleLog.AddLog(messageSts + "frozen!");

                        break;
                    case StatusConditions.BURNED:
                        BattleLog.AddLog(messageSts + "burn!");
                        return battler.UsedMove.Power > 0 ? -1 : 0;

                    case StatusConditions.SLEEP:
                        BattleLog.AddLog(messageSts + "asleep!");
                        if (pokemon.ConditionCount > 0)
                        {
                            BattleLog.AddLog(pokemon.Name + " asleep and cannot attack!");
                            return 0;
                        }
                        break;
                    case StatusConditions.CONFUSED:
                        BattleLog.AddLog(messageSts + "confused!");
                        if (battler.Roll % 2 != 0)
                        {
                            BattleLog.AddLog(pokemon.Name + " is confused and hurt itself! Roll turns 0!");
                            battler.Roll = 0;
                        }
                        break;
                }
                
            }
            return 0;
        }
        private static int RollCombatDices(BattlerManager battler)
        {
            // Se for 0 → rola 1 dado
            // Se for positivo → rola 2 dados e pega o MAIOR
            // Se for negativo → rola 2 dados e pega o MENOR
            int moveDicesRoll = battler.NumberDices;
            string sumDices = "";

            int diceCount = (battler.NumberDices == 0) ? 1 : 2;
            int[] rolls = new int[diceCount];

            for (int i = 0; i < diceCount; i++)
            {
                int roll = DiceRollService.RollDice(1, battler.UsedMove.DiceSides);

                // Corrige valores fora do limite (7 → 5, 8 → 6)
                if (roll == 7){ BattleLog.AddLog($"ROLL {roll} CRÍTICAL, turn 5"); roll = 5; }
                else if (roll == 8) { BattleLog.AddLog($"ROLL {roll} CRÍTICAL, turn 6"); roll = 6; }

                rolls[i] = roll;
                sumDices = sumDices + $" {roll} +";

                BattleLog.AddLog($"ROLL:{roll} - {battler} move {battler.UsedMove.Name}.");
            }

            int result;
            if (moveDicesRoll < 0)
                result = rolls.Min(); // Desvantagem → pega o menor
            else if (moveDicesRoll > 0)
                if (battler.UsedMove.Effects.Any(p => p.EffectType == EffectType.SOMADICES))
                {
                    BattleLog.AddLog("ROLL RESULT: " + sumDices.Substring(0, sumDices.Length - 1) + $" = {rolls.Sum()}.\n");
                    return rolls.Sum();
                }
                else result = rolls.Max(); // Vantagem → pega o maior
            else
                result = rolls[0]; // Normal → único dado

            return result;
        }
        public static void EffectMove(BattlerManager user, BattlerManager targett) // aplica os efeito dos movimentos
        {
            if (user.UsedMove?.Effects == null || user.UsedMove.Effects.Count == 0)
                return;

            int redDice = DiceRollService.RollD6();

            if (user.UsedMove.EffectRoll > 1)
                BattleLog.AddLog($"\n**{user} {user.UsedMove.Name} roll effect: {redDice} - {user.UsedMove.EffectRoll} to activate.");

            if (redDice < user.UsedMove.EffectRoll)
                return;

            foreach (var effect in user.UsedMove.Effects)
            {
                ProfilePokemon targetEffects = effect.TargetEffect == 'B' ? targett.SelectedPokemon : user.SelectedPokemon;
                switch (effect.EffectType)
                {
                    case EffectType.TWODICES:
                        MoreDicesEffect(effect, user, targett, 1);
                        break;
                    case EffectType.SOMADICES:
                        user.NumberDices++;
                        break;
                    case EffectType.THREEDICES: // ainda nao funcional
                        MoreDicesEffect(effect, user, targett, 2);
                        break;

                    case EffectType.BURN:
                        StatusConditionsTrigger(targetEffects, StatusConditions.BURNED);
                        break;
                    case EffectType.CONFUSION:
                        StatusConditionsTrigger(targetEffects, StatusConditions.CONFUSED);
                        break;
                    case EffectType.PARALYZE:
                        StatusConditionsTrigger(targetEffects, StatusConditions.PARALYZED);
                        break;
                    case EffectType.SLEEP:
                        StatusConditionsTrigger(targetEffects, StatusConditions.SLEEP);
                        break;
                    case EffectType.FREEZE:
                        StatusConditionsTrigger(targetEffects, StatusConditions.FROZEN);
                        break;
                    case EffectType.POISON:
                        StatusConditionsTrigger(targetEffects, StatusConditions.POISONED);
                        break;


                    case EffectType.KO:
                        targetEffects.Conditions = StatusConditions.KNOCKED;
                        BattleLog.AddLog($"** {targetEffects.Name} will be knocked out at the end of the round!");
                        break;

                    case EffectType.RECHARGE:
                        user.UsedMove.RechargeMove();
                        BattleLog.AddLog($"** {user.UsedMove.Name} can only be used once per battle.");
                        break;
                    case EffectType.STATUS: //venoshock principalmente
                        if (targetEffects.Conditions != StatusConditions.NORMAL) user.RollBonus += 2; 
                        break;

                    case EffectType.RAIN: // RAIN DANCE
                        FieldControlService.ChangeWeather(FieldCards.RAIN);
                        BattleLog.AddLog(user + $" used {user.UsedMove.Name}, and it started to rain.\n");
                        break;
                    case EffectType.SUNNYDAY: // SUNNY DAY
                        FieldControlService.ChangeWeather(FieldCards.SUNNYDAY);
                        BattleLog.AddLog(user + $" used {user.UsedMove.Name}, and it started to rain.\n");
                        break;
                    case EffectType.SNOW: // 
                        FieldControlService.ChangeWeather(FieldCards.SNOW);
                        BattleLog.AddLog(user + $" used {user.UsedMove.Name}, and it started to rain.\n");
                        break;
                    case EffectType.SAND: // SANDSTORM
                        FieldControlService.ChangeWeather(FieldCards.SANDSTORM);
                        BattleLog.AddLog(user + $" used {user.UsedMove.Name}, and it started to rain.\n");
                        break;

                    case EffectType.ESPECIAL:
                        EspecicalMoves(user, targett, targetEffects);
                        break;
                }
            }
        }
        private static void MoreDicesEffect(EffectManager effect, BattlerManager user, BattlerManager targets, int n) // melhorar para receber threedices
        {
            bool targetIsEnemy = effect.TargetEffect == 'B';

            if (targetIsEnemy)
            {
                targets.NumberDices -= n;
            }
            else // 'W'
            {
                user.NumberDices += n;
            }
        }
        private static void ApplyHalfLevelEffect(BattlerManager user, BattlerManager targets)
        {
            var move = user.SelectedPokemon.MovesPokemon.FirstOrDefault(m => m.Effects.Any(e => e.EffectType == EffectType.HALFLEVEL));
            if (move != null)
            {
                var effect = move.Effects.FirstOrDefault(e => e.EffectType == EffectType.HALFLEVEL);

                if (effect != null)
                {
                    ProfilePokemon target = effect.TargetEffect == 'B' ? targets.SelectedPokemon : user.SelectedPokemon;
                    int baseLevel = target.LevelPokemon();
                    move.HalfLevelMove(ref baseLevel);
                }
            }
        }
        private static void StatusConditionsTrigger(ProfilePokemon target, StatusConditions status) // quando ganha o status
        {
            // Aplica a condição apenas se o Pokémon estiver em NORMAL
            if (target.Conditions == StatusConditions.NORMAL)
            {
                target.Conditions = status;
                if (status == StatusConditions.SLEEP)
                {
                    BattleLog.AddLog(target.Name + " was put to sleep!");
                    target.ConditionCount = BattleConditions.SleepRoll();
                    target.CanAttack = false;

                    target.ConditionCount--;
                    if (target.ConditionCount == 0)
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        BattleLog.AddLog(target.Name + " woke up!");
                        target.CanAttack = true;
                    }
                }
                else if (status == StatusConditions.PARALYZED)
                {
                    BattleLog.AddLog(target.Name + " was paralyzed!");
                    if (!BattleConditions.ParalyzedRoll())
                    {
                        BattleLog.AddLog(target.Name + " is paralyzed and cannot attack!");
                        target.CanAttack = false;
                    }
                }
                else if (status == StatusConditions.FROZEN)
                {
                    BattleLog.AddLog(target.Name + " was frozen!");
                    if (!BattleConditions.FrozenRoll())
                    {
                        BattleLog.AddLog(target.Name + " is still frozen and cannot attack!");
                        target.CanAttack = false;
                    }
                    else
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        BattleLog.AddLog(target.Name + " has been unfrozen and can attack!");
                        target.CanAttack = true;
                    }
                }
            }
        }
        private static void EspecicalMoves(BattlerManager user, BattlerManager opponent, ProfilePokemon targetEffect)
        {
            Move move = user.UsedMove;
            switch (move.MoveID)
            {
                case 29: // Defog
                    FieldControlService.ChangeWeather(FieldCards.NORMAL);
                    FieldControlService.ChangeField(FieldCards.NORMAL);
                    FieldControlService.ChangeTrap(FieldCards.NORMAL);
                    BattleLog.AddLog($"{move.Name} cleared the battlefield!");
                    break;
                case 73: // Disable
                    List<Move> moveList = targetEffect.MovesPokemon.Where(p => p.CanUse).ToList();
                    if (moveList.Count > 1)
                    {
                        int index = ConsoleMenu.ShowMenu(ConsoleColor.Cyan, targetEffect.MovesPokemon.Select(m => m.ToString()).ToList(),
                            "Select a move to disable. The move can’t be used until the end of the battle.");
                        targetEffect.MovesPokemon[index].RechargeMove();
                        BattleLog.AddLog(targetEffect.MovesPokemon[index].Name + " it's disabled!");
                    }
                    else BattleLog.AddLog("Disable didn't work correctly!");
                    break;
                case 54: // Metronome
                    Random random = new Random();
                    int n;
                    do{n = random.Next(0, DataLists.AllMoves.Count);} while (n == 54);
                    Move randomMove = DataLists.AllMoves[n];
                    Move newMove = new Move(54, randomMove.Type, randomMove.Name, randomMove.Power,
                        randomMove.Effects, randomMove.DiceSides, randomMove.EffectRoll);
                    if (newMove.Type == user.SelectedPokemon.Pokemon.Type || newMove.Type == user.SelectedPokemon.Pokemon.StabType) newMove.StabMove();
                    user.UsedMove = newMove;
                    BattleLog.AddLog($"Metronome turned into the move {randomMove.Name}. \n{randomMove}");
                    EffectMove(user, opponent);
                    break;


            }
        }
        #endregion
    }
}
