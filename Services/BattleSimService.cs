using DocumentFormat.OpenXml.Drawing.Diagrams;
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
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
                int bonusCard = 0;
                if (ConsoleMenu.ShowYesNo("Do you want to use a capture item card?"))
                {
                    ItemCard? catchItem = profile.SelectItem(TypeItemCard.Catch);
                    if (catchItem == null) Console.WriteLine("No item card can be selected.");
                    else
                    {
                        Console.WriteLine($"\n{profile.Nickname} used the item card {catchItem.Name}!");
                        bonusCard = catchItem.CatchCard(wildPokemonMod.SelectedPokemon.Pokemon);
                    }

                }
                if(bonusCard >= 50) // masterball
                {
                    Console.WriteLine("You caught the wild Pokémon!");
                    Console.WriteLine($"{wildPokemonMod} add in your party!");
                    Console.ReadLine();
                    wildPokemonMod.SelectedPokemon.NickNamePokemon();
                    profile.AddPokemon(new ProfilePokemon(wildPokemonMod.SelectedPokemon.Pokemon, wildPokemonMod.SelectedPokemon.Name, 0));
                    return;
                }
            
                if (WildGeneratorService.CatchWildPokemon(wildPokemonMod.SelectedPokemon.Pokemon.CathRate, bonusCard))
                {
                    Console.WriteLine($"{wildPokemonMod} add in your party!");
                    Console.ReadLine();
                    wildPokemonMod.SelectedPokemon.NickNamePokemon();
                    profile.AddPokemon(new ProfilePokemon(wildPokemonMod.SelectedPokemon.Pokemon, wildPokemonMod.SelectedPokemon.Name, 0));
                }
                else Console.ReadLine();
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

            //attacker.SelectedPokemon.MovesPokemon.Add(DataLists.GetMoveID(156)); // teste
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
                    attacker.UsedCard.BattleCard(attacker, defender);
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
                        playerD.UsedCard.BattleCard(defender, attacker);
                    }
                }
            }

            if (!defender.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST)) EffectMove(attacker, defender);
            if (!attacker.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST)) EffectMove(defender, attacker);

            BattleLog.ShowLogs();
            BattleLog.ClearLogs();

            BattleLog.AddLog("\n==== ROLLING DICES ====");
            if (defender.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST)) // se defensor tiver first
            {
                BattleLog.AddLog($"{defender} can strike first with {defender.UsedMove.Name}!");
                BattleCalculation(defender, attacker);
                EffectMove(attacker, defender);
                BattleCalculation(attacker, defender);

            }
            else if (attacker.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST))
            {
                BattleLog.AddLog($"{attacker} can strike first with {attacker.UsedMove.Name}!");
                BattleCalculation(attacker, defender);
                EffectMove(defender, attacker);
                BattleCalculation(defender, attacker);
            }
            else
            {
                BattleCalculation(attacker, defender);
                BattleCalculation(defender, attacker);
            }
            // battle log
            BattleLog.AddLog("\n==== RESULT COMBAT ====");
            attacker.DisplayBattleStatus();
            defender.DisplayBattleStatus();
            BattleLog.ShowLogs();
            VictoryChanceService.ShowResult();

            if (attacker.TotalResult == defender.TotalResult) // empate
            {
                if(defender.TrainerName != null) // se for trainer
                {
                    do
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Tie! Roll the dice again.\n");
                        Console.ResetColor();
                        Console.ReadLine();
                        attacker.Roll = RollCombatDices(attacker);
                        defender.Roll = RollCombatDices(defender);
                        attacker.CalculateTotalPower();
                        defender.CalculateTotalPower();
                        // battle log
                        BattleLog.ClearLogs();
                        BattleLog.AddLog("\n==== RESULT COMBAT ====");
                        attacker.DisplayBattleStatus();
                        defender.DisplayBattleStatus();
                        BattleLog.ShowLogs();
                    }
                    while (attacker.TotalResult == defender.TotalResult);
                    VictoryChanceService.ShowResult();
                }
                else defender.SelectedPokemon.Conditions = StatusConditions.KNOCKED; // selvagem
            }

                ResultBattle(attacker, defender);

            BattleLog.ClearLogs();
        }
        public static void BattleCalculation(BattlerManager attacker, BattlerManager defender)
        {
            attacker.Roll = RollCombatDices(attacker); // rolagem do dado do move, de acordo com o efeito
            attacker.EffectiveBonus = EffectiveTypeService.GetTypeModifier(attacker.UsedMove.Type, defender.SelectedPokemon.Pokemon.Type); // effetivo de tipo
            attacker.StatusBonus = StatusPenalty(attacker); // penalidade de status

            attacker.WeatherBonus = FieldControlService.WeatherBonus(attacker); // efeito do clima
            if (!attacker.SelectedPokemon.CanAttack) attacker.UsedMove = nullMove;
            

            // downed effect
            if (attacker.UsedMove.Type == TypePokemon.Ground && defender.HasEffectType(EffectType.DOWNED) && attacker.EffectiveBonus < 0)
            {
                BattleLog.AddLog($"{defender.SelectedPokemon.Name} downed and can’t resist Ground-type attacks.");
                attacker.EffectiveBonus = 0; 
            }
            // dragon rage, imune
            if (attacker.UsedMove.Effects.Any(p => p.EffectType == EffectType.IMMUNE))
            {
                attacker.EffectiveBonus = 0;
            }
            //dream eater
            if (defender.SelectedPokemon.Conditions == StatusConditions.SLEEP && attacker.UsedMove.MoveID == 157)
            {
                BattleLog.AddLog(attacker + " doubled the attack’s power by devouring the opponent’s dreams!");
                attacker.RollBonus += attacker.Roll + attacker.RollBonus;
            }
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

            if (attacker.TotalResult > defender.TotalResult) defender.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            else if (attacker.TotalResult < defender.TotalResult) attacker.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            if (pokemonAtk.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(attacker);
            if (pokemonDef.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(defender);
            if (pokemonAtk.Conditions == StatusConditions.KNOCKED) Console.WriteLine(attacker + " has been knocked out!");
            else if (attacker.UsedMove.Effects.Any(p => p.EffectType == EffectType.BOOST))
            {
                Console.WriteLine(attacker.UsedMove.Name + " got a boost!");
                attacker.UsedMove.BoostPower();
            }
            if (pokemonDef.Conditions == StatusConditions.KNOCKED) Console.WriteLine(defender + " has been knocked out!");
            else if (defender.UsedMove.Effects.Any(p => p.EffectType == EffectType.BOOST))
            {
                Console.WriteLine(defender.UsedMove.Name + " got a boost!");
                defender.UsedMove.BoostPower();
            }
            if (pokemonAtk.LevelPokemon() <= pokemonDef.LevelPokemon() && pokemonAtk.Conditions != StatusConditions.KNOCKED)
                pokemonAtk.LevelUpPokemon();
            if (pokemonDef.LevelPokemon() <= pokemonAtk.LevelPokemon() && pokemonDef.Conditions != StatusConditions.KNOCKED && defender.TrainerName != null)
                pokemonDef.LevelUpPokemon();
            Console.ReadLine();
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
            int bonusDice = battler.NumberDices;
            int diceCount = Math.Max(1, 1 + Math.Abs(bonusDice)); // 1 padrão + adicionais

            int[] rolls = new int[diceCount];
            string sumDices = "";

            for (int i = 0; i < diceCount; i++)
            {
                int roll = DiceRollService.RollDice(1, battler.UsedMove.DiceSides);

                // Corrige valores fora do limite (7 → 5, 8 → 6)
                if (roll == 7)
                {
                    BattleLog.AddLog($"ROLL {roll} CRÍTICAL, turn 5");
                    roll = 5;
                }
                else if (roll == 8)
                {
                    BattleLog.AddLog($"ROLL {roll} CRÍTICAL, turn 6");
                    roll = 6;
                }

                rolls[i] = roll;
                sumDices += $" {roll} +";
                BattleLog.AddLog($"ROLL:{roll} - {battler} move {battler.UsedMove.Name}.");
            }

            int result;

            // Se o movimento possui SOMADICES, a soma prevalece, sem interferência do NumberDices
            if (battler.UsedMove.Effects.Any(p => p.EffectType == EffectType.SOMADICES))
            {
                if(rolls.Length > 1) BattleLog.AddLog("ROLL RESULT: " + sumDices[..^1] + $" = {rolls.Sum()}.\n");
                result = rolls.Sum();
            }
            else
            {
                // Caso contrário, vantagem/desvantagem afeta o melhor/pior resultado
                if (bonusDice < 0)
                    result = rolls.Min(); // Desvantagem
                else if (bonusDice > 0)
                    result = rolls.Max(); // Vantagem
                else
                    result = rolls[0];    // Normal
            }

            // Efeito de precisão
            if (battler.UsedMove.Effects.Any(p => p.EffectType == EffectType.PRECISION) && result < 3 && battler.UsedMove.DiceSides > 5)
            {
                BattleLog.AddLog($"** Effect Precision of {battler.UsedMove.Name}. Roll result {result} modified to 3.");
                result = 3;
            }

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
                BattlerManager targetEffects = effect.TargetEffect == 'B' ? targett : user;
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
                        StatusConditionsTrigger(targetEffects.SelectedPokemon, StatusConditions.BURNED);
                        break;
                    case EffectType.CONFUSION:
                        StatusConditionsTrigger(targetEffects.SelectedPokemon, StatusConditions.CONFUSED);
                        break;
                    case EffectType.PARALYZE:
                        StatusConditionsTrigger(targetEffects.SelectedPokemon, StatusConditions.PARALYZED);
                        break;
                    case EffectType.SLEEP:
                        StatusConditionsTrigger(targetEffects.SelectedPokemon, StatusConditions.SLEEP);
                        break;
                    case EffectType.FREEZE:
                        StatusConditionsTrigger(targetEffects.SelectedPokemon, StatusConditions.FROZEN);
                        break;
                    case EffectType.POISON:
                        StatusConditionsTrigger(targetEffects.SelectedPokemon, StatusConditions.POISONED);
                        break;


                    case EffectType.KO:
                        targetEffects.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
                        BattleLog.AddLog($"** {targetEffects.SelectedPokemon.Name} will be knocked out at the end of the round!");
                        break;

                    case EffectType.RECHARGE:
                        user.UsedMove.RechargeMove();
                        BattleLog.AddLog($"** {user.UsedMove.Name} can only be used once per battle.");
                        break;
                    case EffectType.STATUS: //venoshock principalmente
                        if (targetEffects.SelectedPokemon.Conditions != StatusConditions.NORMAL) user.RollBonus += 2; 
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
                    case EffectType.CHANGE:
                        if (targetEffects.TrainerBox != null)
                        {
                            var availablePokemons = targetEffects.TrainerBox.ListPokemon
                                .Where(p => p.Conditions != StatusConditions.KNOCKED
                                            && p.Name != targetEffects.SelectedPokemon.Name).ToList();

                            if (availablePokemons.Count > 0)
                            {
                                int index = DiceRollService.RollDice(1, availablePokemons.Count - 1);
                                var newPokemon = availablePokemons[index];
                                BattleLog.AddLog($"Active Pokémon {targetEffects.SelectedPokemon.Name} is changed to " + newPokemon.Name);
                                Console.ReadLine();
                                targetEffects.SelectedPokemon = newPokemon;
                                // selectmove again
                                if (MoveStatusCheck(targetEffects))
                                {
                                    if (targetEffects.TrainerName == null)
                                    {
                                        targetEffects.UsedMove = targetEffects.SelectedPokemon.RandomMovePokemon();
                                        BattleLog.AddLog($"=> {targetEffects} used {targetEffects.UsedMove.Name}.\n" + targetEffects.UsedMove + "\n");
                                    }
                                    else
                                    {
                                        BattlerManager battlerDefender = effect.TargetEffect == 'B' ? user : targett;
                                        targetEffects.UsedMove = targetEffects.SelectedPokemon.SelectMovePokemon(targetEffects, battlerDefender);
                                        BattleLog.AddLog($"=> {targetEffects} used {targetEffects.UsedMove.Name}.\n" + targetEffects.UsedMove + "\n");
                                    }

                                }
                            }
                        }
                        break;


                    case EffectType.ESPECIAL:
                        EspecicalMoves(user, targett);
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
                    move.HalfLevelPower(ref baseLevel);
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
                else if (status == StatusConditions.POISONED)
                {
                    BattleLog.AddLog(target.Name + " was poisoned!");
                }
            }
        }
        private static void EspecicalMoves(BattlerManager user, BattlerManager opponent)
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
                    List<Move> moveList = opponent.SelectedPokemon.MovesPokemon.Where(p => p.CanUse).ToList();
                    if (moveList.Count > 1)
                    {
                        int index = 0;
                        if (user.TrainerName == "TESTE") index = DiceRollService.RollDice(1, opponent.SelectedPokemon.MovesPokemon.Count) - 1;
                        else
                        {
                            index = ConsoleMenu.ShowMenu(ConsoleColor.Cyan, opponent.SelectedPokemon.MovesPokemon.Select(m => m.ToString()).ToList(),
                                "Select a move to disable. The move can’t be used until the end of the battle.");
                        }
                        opponent.SelectedPokemon.MovesPokemon[index].RechargeMove();
                        BattleLog.AddLog(opponent.SelectedPokemon.MovesPokemon[index].Name + " it's disabled!");
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
                case 88: //smack down
                    opponent.BuffsAndDebuffs.Add(EffectType.DOWNED);
                    BattleLog.AddLog($"{user.SelectedPokemon.Name} knocked out {opponent.SelectedPokemon.Name} with {user.UsedMove}!");
                    break;
                case 98: // tri attack
                    int t = DiceRollService.RollD6();
                    StatusConditions[] effects =
                        { StatusConditions.PARALYZED, StatusConditions.PARALYZED,
                          StatusConditions.BURNED, StatusConditions.BURNED,
                          StatusConditions.FROZEN, StatusConditions.FROZEN };

                    StatusConditionsTrigger(opponent.SelectedPokemon, effects[t - 1]);
                    break;
                case 36: // mirror move
                    BattleLog.AddLog($"** The move {opponent.UsedMove.Name} has been copied!");
                    user.UsedMove = opponent.UsedMove.Copy();
                    EffectMove(user, opponent);
                    break;
                case 124: // counter
                    BattleLog.AddLog($"** COUNTER! The power of move has been changed to " + opponent.UsedMove.Power);
                    user.UsedMove.ChangePower(opponent.UsedMove.Power);
                    break;
            }
        }
        #endregion
    }
}
