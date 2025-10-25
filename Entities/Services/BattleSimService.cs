using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities.Services
{
    static class BattleSimService // classe que controla as simulações de combate
    {
        private static readonly Move nullMove = new Move(0, TypePokemon.None, "No Move", 0, 6);
        private static int attackerDicesRoll = 0;
        private static int defenderDicesRoll = 0;
        private static ProfilePokemon pokemonAtk;
        private static ProfilePokemon pokemonDef;
        private static readonly ConsoleColor atkColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor defColor = ConsoleColor.Magenta;

        public static void BattlePokemonSetup(List<BoxPokemon> profiles)
        {
            // Select Profiles
            int index = ConsoleMenu.ShowMenu(atkColor, profiles.Select(m => m.Nickname).ToList(), "Choose a Attacker trainer Profile");
            BoxPokemon ProfileA = profiles[index]; // Attacker Profile
            index = ConsoleMenu.ShowMenu(defColor, profiles.Select(m => m.Nickname).ToList(), "Choose a Defender trainer Profile");
            BoxPokemon ProfileB = profiles[index]; // Defender Profile

            // Choose Pokemon Attacker Pokemon
            index = ConsoleMenu.ShowMenu(atkColor, ProfileA.ListBox.Select(m => m.ToString()).ToList(), $"Choose {ProfileA.Nickname}'s Attacking Pokémon");
            ProfilePokemon pokemonAttacker = ProfileA.ListBox[index];
            BattleModifications profileAtk = new BattleModifications(ProfileA, SetupBattle.Attacker, pokemonAttacker);
            BattleLog.AddLog($"{ProfileA.Nickname} selected {pokemonAttacker.Name} as their attacking Pokémon.\n" + pokemonAttacker.Pokemon.ToString());
            // Choose Pokemon Defender Pokemon
            index = ConsoleMenu.ShowMenu(defColor, ProfileB.ListBox.Select(m => m.ToString()).ToList(), $"Choose {ProfileB.Nickname}'s Defending Pokémon");
            ProfilePokemon pokemonDefender = ProfileB.ListBox[index];
            BattleModifications profileDef = new BattleModifications(ProfileB, SetupBattle.Defender, pokemonDefender);
            BattleLog.AddLog($"{ProfileB.Nickname} selected {pokemonDefender.Name} as their defending Pokémon.\n" + pokemonDefender.Pokemon.ToString());


            BattleControl(profileAtk, profileDef);
        }
        public static void BattleControl(BattleModifications attacker, BattleModifications defender)
        {
            pokemonAtk = attacker.SelectedPokemon;
            pokemonDef = defender.SelectedPokemon;

            // move half level effect
            ApplyHalfLevelEffect(pokemonAtk, true);
            ApplyHalfLevelEffect(pokemonDef, false);

            // move selection
            if (MoveStatusCheck(pokemonAtk))
            {
                attacker.UsedMove = pokemonAtk.SelectMove(attacker, defender);
                BattleLog.AddLog($"{attacker} used {attacker.UsedMove.Name}.\n" + attacker.UsedMove.ToString());
            }
            if (MoveStatusCheck(pokemonDef))
            {
                defender.UsedMove = pokemonDef.SelectMove(defender, attacker);
                BattleLog.AddLog($"{defender} used {defender.UsedMove.Name}.\n" + defender.UsedMove.ToString());
            }

            // card selection
            attacker.UsedCard = attacker.TrainerBox.SelectItem(TypeItemCard.Battle);
            defender.UsedCard = defender.TrainerBox.SelectItem(TypeItemCard.Battle);
            if (attacker.UsedCard != null)
            {
                BattleLog.AddLog($"{attacker.TrainerName} used the item card {attacker.UsedCard.Name}!");
                EffectCard.CardEffectBattle(attacker.UsedCard.BattleCard(), attacker);

            }
            if (defender.UsedCard != null)
            {
                BattleLog.AddLog($"{defender.TrainerName} used the item card {defender.UsedCard.Name}!");
                EffectCard.CardEffectBattle(defender.UsedCard.BattleCard(), defender);

            }

            // effect
            EffectMove(attacker, true);
            EffectMove(defender, false);

            BattleLog.ShowLogs();
            BattleLog.ClearLogs();
            do
            {
                BattleLog.AddLog("\n==== ROLLING DICES ====");
                attacker.NumberDices = attackerDicesRoll;
                defender.NumberDices = defenderDicesRoll;

                // roll
                attacker.Roll = RollCombatDices(attacker);
                defender.Roll = RollCombatDices(defender);

                // bonus status
                attacker.StatusBonus = CheckStats(pokemonAtk, attacker.UsedMove, attacker.Roll, out int confusionModA);
                defender.StatusBonus = CheckStats(pokemonDef, defender.UsedMove, defender.Roll, out int confusionModB);

                if (attacker.SelectedPokemon.CanAttack == false) attacker.UsedMove = nullMove;
                if (defender.SelectedPokemon.CanAttack == false) defender.UsedMove = nullMove;

                // effective bonus
                attacker.EffectiveBonus = EffectiveTypeService.GetTypeModifier(attacker.UsedMove.Type, pokemonDef.Pokemon.Type);
                defender.EffectiveBonus = EffectiveTypeService.GetTypeModifier(defender.UsedMove.Type, pokemonAtk.Pokemon.Type);
                
                // confusion bonus
                attacker.StatusBonus += confusionModB;
                defender.StatusBonus += confusionModA;

                attacker.CalculateTotalPower();
                defender.CalculateTotalPower();

                // battle log
                attacker.DisplayBattleStatus();
                defender.DisplayBattleStatus();
                BattleLog.ShowLogs();

                if (attacker.TotalResult == defender.TotalResult)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Tie! Roll the dice again.\n");
                    Console.ResetColor();
                    Console.ReadLine();
                }

            } while (attacker.TotalResult == defender.TotalResult);




            string victoryPokemon = attacker.TotalResult > defender.TotalResult ? attacker.ToString() : defender.ToString();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + victoryPokemon + " won the battle!\n");
            Console.ResetColor();


            // KNOCKED OUT, POISONED check amd level up
            // poison roll
            if (pokemonAtk.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(attacker);
            if (pokemonDef.Conditions == StatusConditions.POISONED) BattleConditions.PoisonRoll(defender);
            if (attacker.TotalResult > defender.TotalResult)
            { 
                if(pokemonAtk.LevelPokemon() <= pokemonDef.LevelPokemon() && pokemonAtk.Conditions != StatusConditions.KNOCKED) pokemonAtk.LevelUp();
                defender.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            }
            else if(attacker.TotalResult < defender.TotalResult)
            {
                if(pokemonDef.LevelPokemon() <= pokemonAtk.LevelPokemon() && pokemonDef.Conditions != StatusConditions.KNOCKED) pokemonDef.LevelUp();
                attacker.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            } 
            if (pokemonAtk.Conditions == StatusConditions.KNOCKED) Console.WriteLine(pokemonAtk.Name + " has been knocked out!");
            if (pokemonDef.Conditions == StatusConditions.KNOCKED) Console.WriteLine(pokemonDef.Name + " has been knocked out!");


            BattleLog.ClearLogs();
        }


        private static bool MoveStatusCheck(ProfilePokemon pokemon)
        {
            if (pokemon.Conditions == StatusConditions.PARALYZED)
            {
                if (!BattleConditions.ParalyzedRoll())
                {
                    BattleLog.AddLog(pokemon.Name + " is paralyzed and cannot attack!");
                    return false;
                }
            }
            else if (pokemon.Conditions == StatusConditions.SLEEP)
            {
                pokemon.ConditionCount--;
                if (pokemon.ConditionCount == 0) { BattleLog.AddLog(pokemon.Name + " woke up!"); pokemon.Conditions = StatusConditions.NORMAL; return true; }
                else
                {
                    BattleLog.AddLog(pokemon.Name + " is still sleeping...");
                    return false;
                }
            }
            else if (pokemon.Conditions == StatusConditions.FROZEN)
            {
                if (BattleConditions.FrozenRoll())
                {
                    pokemon.Conditions = StatusConditions.NORMAL;
                    BattleLog.AddLog(pokemon.Name + " has been unfrozen and can attack!");
                    return true;
                }
                else BattleLog.AddLog(pokemon.Name + " is still frozen.)"); return false;
            }
            return true;
        }
        private static int CheckStats(ProfilePokemon pokemon, Move move, int roll, out int rollConfusion)
        {
            rollConfusion = 0;
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
                        return move.Power > 0 ? -1 : 0;

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
                        if (roll % 2 != 0)
                        {
                            BattleLog.AddLog(pokemon.Name + " is confused and hurt itself!");
                            rollConfusion = roll;
                        }
                        break;
                }
                
            }
            return 0;
        }
        private static int RollCombatDices(BattleModifications trainer)
        {
            // Se for 0 → rola 1 dado
            // Se for positivo → rola 2 dados e pega o MAIOR
            // Se for negativo → rola 2 dados e pega o MENOR
            int moveDicesRoll = trainer.NumberDices;

            int diceCount = (trainer.NumberDices == 0) ? 1 : 2;
            int[] rolls = new int[diceCount];

            for (int i = 0; i < diceCount; i++)
            {
                int roll = DiceRollService.RollDice(1, trainer.UsedMove.DiceSides);

                // Corrige valores fora do limite (7 → 5, 8 → 6)
                if (roll == 7) roll = 5;
                else if (roll == 8) roll = 6;

                rolls[i] = roll;
                BattleLog.AddLog($"{trainer} move {trainer.UsedMove.Name} - Roll: {roll}");
            }

            int result;
            if (moveDicesRoll < 0)
                result = rolls.Min(); // Desvantagem → pega o menor
            else if (moveDicesRoll > 0)
                if (trainer.UsedMove.Effects.Any(p => p.EffectType == EffectType.SOMADICES))
                {
                    return rolls.Sum();
                }
                else result = rolls.Max(); // Vantagem → pega o maior
            else
                result = rolls[0]; // Normal → único dado

            return result;
        }
        private static void EffectMove(BattleModifications user, bool isAttacker) // aplica os efeito dos movimentos
        {
            if (user.UsedMove?.Effects == null || user.UsedMove.Effects.Count == 0)
                return;

            int redDice = DiceRollService.RollD6();

            if (user.UsedMove.EffectRoll > 1)
                BattleLog.AddLog($"{user} {user.UsedMove.Name} roll effect: {redDice} - {user.UsedMove.EffectRoll} to activate.\n");

            if (redDice < user.UsedMove.EffectRoll)
                return;
            StatusConditions effectCondition = StatusConditions.NORMAL;
            foreach (var effect in user.UsedMove.Effects)
            {
                switch (effect.EffectType)
                {
                    case EffectType.TWODICES:
                        ApplyTwoDicesEffect(effect, isAttacker);
                        break;

                    case EffectType.SOMADICES:
                        ApplySumDicesEffect(isAttacker);
                        break;

                    case EffectType.BURN:
                        effectCondition = StatusConditions.BURNED;
                        StatusConditionsTrigger(effect, effectCondition, isAttacker);
                        break;
                    case EffectType.CONFUSION:
                        effectCondition = StatusConditions.CONFUSED;
                        StatusConditionsTrigger(effect, effectCondition, isAttacker);
                        break;
                    case EffectType.PARALYZE:
                        effectCondition = StatusConditions.PARALYZED;
                        StatusConditionsTrigger(effect, effectCondition, isAttacker);
                        break;
                    case EffectType.SLEEP:
                        effectCondition = StatusConditions.SLEEP;
                        StatusConditionsTrigger(effect, effectCondition, isAttacker);
                        break;
                    case EffectType.FREEZE:
                        effectCondition = StatusConditions.FROZEN;
                        StatusConditionsTrigger(effect, effectCondition, isAttacker);
                        break;
                    case EffectType.POISON:
                        effectCondition = StatusConditions.POISONED;
                        StatusConditionsTrigger(effect, effectCondition, isAttacker);
                        break;


                    case EffectType.KO:
                        bool targetIsEnemy = effect.TargetEffect == 'B';
                        ProfilePokemon target;
                        // Determina o alvo correto
                        if (targetIsEnemy)
                            target = isAttacker ? pokemonDef : pokemonAtk;
                        else // 'W' ou aliado
                            target = isAttacker ? pokemonAtk : pokemonDef;
                        target.Conditions = StatusConditions.KNOCKED;
                        BattleLog.AddLog(target.Name + " will be knocked out at the end of the round!");
                        break;

                }
            }
        }

        private static void ApplyTwoDicesEffect(EffectMove effect, bool isAttacker)
        {
            bool targetIsEnemy = effect.TargetEffect == 'B';

            if (targetIsEnemy)
            {
                if (isAttacker)
                    defenderDicesRoll -= 1;
                else
                    attackerDicesRoll -= 1;
            }
            else // 'W'
            {
                if (isAttacker)
                    attackerDicesRoll += 1;
                else
                    defenderDicesRoll += 1;
            }
        }
        private static void ApplySumDicesEffect(bool isAttacker)
        {
            if (isAttacker)
                attackerDicesRoll += 1;
            else
                defenderDicesRoll += 1;
        }

        private static void ApplyHalfLevelEffect(ProfilePokemon pokemon, bool isAttacker)
        {
            var move = pokemon.Pokemon.Moves.FirstOrDefault(m => m.Effects.Any(e => e.EffectType == EffectType.HALFLEVEL));
            if (move != null)
            {
                var effect = move.Effects.FirstOrDefault(e => e.EffectType == EffectType.HALFLEVEL);

                if (effect != null)
                {
                    bool targetIsEnemy = effect.TargetEffect == 'B';

                    int baseLevel;

                    if (targetIsEnemy)
                        baseLevel = isAttacker ? pokemonDef.LevelPokemon() : pokemonAtk.LevelPokemon();
                    else
                        baseLevel = isAttacker ? pokemonAtk.LevelPokemon() : pokemonDef.LevelPokemon();

                    move.HalfLevelMove(ref baseLevel);
                }
            }
        }

        private static void StatusConditionsTrigger(EffectMove effect, StatusConditions status, bool isAttacker) // quando ganha o status
        {
            bool targetIsEnemy = effect.TargetEffect == 'B';
            ProfilePokemon target;

            // Determina o alvo correto
            if (targetIsEnemy)
                target = isAttacker ? pokemonDef : pokemonAtk;
            else // 'W' ou aliado
                target = isAttacker ? pokemonAtk : pokemonDef;

            // Aplica a condição apenas se o Pokémon estiver em NORMAL
            if (target.Conditions == StatusConditions.NORMAL)
            {
                target.Conditions = status;
                if (status == StatusConditions.SLEEP)
                {
                    BattleLog.AddLog(target.Name + " was put to sleep!");
                    target.ConditionCount = BattleConditions.SleepRoll();
                    if (!isAttacker)
                        pokemonAtk.CanAttack = false;
                    else
                        pokemonDef.CanAttack = false;

                    target.ConditionCount--;
                    if (target.ConditionCount == 0)
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        BattleLog.AddLog(target.Name + " woke up!");
                        if (!isAttacker)
                            pokemonAtk.CanAttack = true;
                        else
                            pokemonDef.CanAttack = true;
                    }
                }

                else if (status == StatusConditions.PARALYZED)
                {
                    BattleLog.AddLog(target.Name + " was paralyzed!");
                    if (!BattleConditions.ParalyzedRoll())
                    {
                        BattleLog.AddLog(target.Name + " is paralyzed and cannot attack!");
                        if (isAttacker)
                            pokemonDef.CanAttack = false;
                        else
                            pokemonAtk.CanAttack = false;
                    }
                }

                else if (status == StatusConditions.FROZEN)
                {
                    BattleLog.AddLog(target.Name + " was frozen!");
                    if (!BattleConditions.FrozenRoll())
                    {
                        BattleLog.AddLog(target.Name + " is still frozen and cannot attack!");
                        if (isAttacker)
                            pokemonDef.CanAttack = false;
                        else
                            pokemonAtk.CanAttack = false;
                    }
                    else
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        BattleLog.AddLog(target.Name + " has been unfrozen and can attack!");
                        if (isAttacker)
                            pokemonDef.CanAttack = true;
                        else
                            pokemonAtk.CanAttack = true;
                    }
                }
            }
        }

        
    }
}
