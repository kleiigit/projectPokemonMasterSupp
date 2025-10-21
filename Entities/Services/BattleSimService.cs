using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities.Services
{
    static class BattleSimService // classe que controla as simulações de combate
    {
        private static string battleLog = "";
        private static Move nullMove = new Move(0, TypePokemon.None, "No Move", 0, 6);
        private static int attackerDicesRoll = 0;
        private static int defenderDicesRoll = 0;
        private static ProfilePokemon pokemonAtk;
        private static ProfilePokemon pokemonDef;

        public static void SelectPokemon(BoxPokemon profileAttacker, BoxPokemon profileDefender)
        {
            // Choose Pokemon
            int index = ConsoleMenu.ShowMenu(profileAttacker.ListBox.Select(m => m.ToString()).ToList(), $"Choose {profileAttacker.Nickname}'s Attacking Pokémon");
            ProfilePokemon pokemonAttacker = profileAttacker.ListBox[index];
            BattleModifications profileAtk = new BattleModifications(profileAttacker, SetupBattle.Attacker, pokemonAttacker);

            index = ConsoleMenu.ShowMenu(profileDefender.ListBox.Select(m => m.ToString()).ToList(), $"Choose {profileDefender.Nickname}'s Defending Pokémon");
            ProfilePokemon pokemonDefender = profileDefender.ListBox[index];
            BattleModifications profileDef = new BattleModifications(profileDefender, SetupBattle.Defender, pokemonDefender);


            BattleControl(profileAtk, profileDef);
        }
        public static void BattleControl(BattleModifications attacker, BattleModifications defender)
        {
            pokemonAtk = attacker.Pokemon;
            pokemonDef = defender.Pokemon;

            //pokemonAtk.Conditions = StatusConditions.BURNED;
            //pokemonDef.Conditions = StatusConditions.CONFUSED;

            if (MoveStatusCheck(pokemonAtk))
            {
                attacker.UsedMove = pokemonAtk.Pokemon.SelectMove(attacker.ToString());
                battleLog += $"{attacker} used {attacker.UsedMove.Name}.\n";
            }

            if (MoveStatusCheck(pokemonDef))
            {
                defender.UsedMove = pokemonDef.Pokemon.SelectMove(defender.ToString());
                battleLog += $"{defender} used {defender.UsedMove.Name}.\n";
            }
            battleLog += "\n";

            attacker.UsedCard = attacker.TrainerBox.SelectItem(TypeItemCard.Battle);
            defender.UsedCard = defender.TrainerBox.SelectItem(TypeItemCard.Battle);

            if (attacker.UsedCard != null)
            {
                battleLog += $"{attacker.TrainerName} used the item card {attacker.UsedCard.Name}!\n";
                CardEffectBattle(attacker.UsedCard.BattleCard(), attacker);

            }
            if (defender.UsedCard != null)
            {
                battleLog += $"{defender.TrainerName} used the item card {defender.UsedCard.Name}!\n";
                CardEffectBattle(defender.UsedCard.BattleCard(), defender);

            }
            battleLog += "\n";

            EffectMove(attacker, true);
            EffectMove(defender, false);

            do
            {
                attacker.NumberDices = attackerDicesRoll;
                defender.NumberDices = defenderDicesRoll;
                // roll
                attacker.Roll = RollCombatDices(attacker.UsedMove, attacker.NumberDices);
                defender.Roll = RollCombatDices(defender.UsedMove, defender.NumberDices);
                battleLog += "\n";

                // bonus status
                attacker.StatusBonus = CheckStats(pokemonAtk, attacker.UsedMove, attacker.Roll, out int confusionModA);
                defender.StatusBonus = CheckStats(pokemonDef, defender.UsedMove, defender.Roll, out int confusionModB);
                battleLog += "\n";

                if (attacker.Pokemon.CanAttack == false) attacker.UsedMove = nullMove;
                if (defender.Pokemon.CanAttack == false) defender.UsedMove = nullMove;

                // effective bonus
                attacker.EffectiveBonus = EffectiveTypeService.GetTypeModifier(attacker.UsedMove.Type, pokemonDef.Pokemon.Type);
                defender.EffectiveBonus = EffectiveTypeService.GetTypeModifier(defender.UsedMove.Type, pokemonAtk.Pokemon.Type);
                
                // confusion bonus
                attacker.StatusBonus += confusionModB;
                defender.StatusBonus += confusionModA;

                attacker.CalculateTotalPower();
                defender.CalculateTotalPower();
                Console.WriteLine();

                // battle log
                battleLog += attacker.DisplayBattleStatus();
                battleLog += "\n";
                battleLog += defender.DisplayBattleStatus();
                Console.WriteLine(battleLog);

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
            Console.WriteLine(victoryPokemon + " won the battle!");
            if (attacker.TotalResult > defender.TotalResult && pokemonAtk.LevelPokemon() <= pokemonDef.LevelPokemon()) pokemonAtk.LevelUp();
            else if (attacker.TotalResult < defender.TotalResult && pokemonDef.LevelPokemon() <= pokemonAtk.LevelPokemon()) pokemonDef.LevelUp();
            Console.ResetColor();
            battleLog = "";
        }

        private static bool MoveStatusCheck(ProfilePokemon pokemon)
        {
            if (pokemon.Conditions == StatusConditions.PARALYZED)
            {
                if (!BattleConditions.ParalyzedRoll())
                {
                    battleLog += pokemon.Name + " está paralisado e não pode atacar!\n";
                    return false;
                }
            }
            else if (pokemon.Conditions == StatusConditions.SLEEP)
            {
                pokemon.ConditionCount--;
                if (pokemon.ConditionCount == 0) { battleLog += pokemon.Name + " acordou!\n"; pokemon.Conditions = StatusConditions.NORMAL; return true; }
                else
                {
                    battleLog += pokemon.Name + " continua dormindo...\n";
                    return false;
                }
            }
            else if (pokemon.Conditions == StatusConditions.FROZEN)
            {
                if (BattleConditions.FrozenRoll())
                {
                    pokemon.Conditions = StatusConditions.NORMAL;
                    battleLog += pokemon.Name + " descongelou e pode atacar!\n";
                    return true;
                }
                else battleLog += pokemon.Name + " continua congelado.\n"; return false;
            }
            return true;
        }
        private static int CheckStats(ProfilePokemon pokemon, Move move, int roll, out int rollConfusion)
        {
            rollConfusion = 0;
            if (pokemon.Conditions != StatusConditions.NORMAL)
            {
                StatusConditions status = pokemon.Conditions;
                string messageSts = pokemon.Name + " está ";

                switch (status)
                {
                    case StatusConditions.PARALYZED:
                        battleLog += messageSts + "paralisado!\n";
                        break;
                    case StatusConditions.FROZEN:
                        battleLog += messageSts + "congelado!\n";

                        break;
                    case StatusConditions.BURNED:
                        battleLog += messageSts + "queimado!\n";
                        return move.Power > 0 ? -1 : 0;

                    case StatusConditions.SLEEP:
                        battleLog += messageSts + "dormindo!\n";
                        if (pokemon.ConditionCount > 0)
                        {
                            battleLog += pokemon.Name + " está dormindo e não pode atacar!\n";
                            return 0;
                        }
                        break;
                    case StatusConditions.CONFUSED:
                        battleLog += messageSts + "confuso!\n";
                        if (roll % 2 != 0)
                        {
                            battleLog += pokemon.Name + " está confuso e se machucou!\n";
                            rollConfusion = roll;
                        }
                        break;
                }
                
            }
            return 0;
        }
        private static int RollCombatDices(Move move, int moveDicesRoll)
        {
            // Se for 0 → rola 1 dado
            // Se for positivo → rola 2 dados e pega o MAIOR
            // Se for negativo → rola 2 dados e pega o MENOR

            int diceCount = (moveDicesRoll == 0) ? 1 : 2;
            int[] rolls = new int[diceCount];

            for (int i = 0; i < diceCount; i++)
            {
                int roll = DiceRollService.RollDice(1, move.DiceSides);

                // Corrige valores fora do limite (7 → 5, 8 → 6)
                if (roll == 7) roll = 5;
                else if (roll == 8) roll = 6;

                rolls[i] = roll;
                battleLog += $"{move.Name} - Roll {i + 1}: {roll}\n";
            }

            int result;
            if (moveDicesRoll < 0)
                result = rolls.Min(); // Desvantagem → pega o menor
            else if (moveDicesRoll > 0)
                if (move.Effects.Any(p => p.EffectType == EffectType.SOMADICES))
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
                Console.WriteLine($"{user.ToString()} {user.UsedMove.Name} roll effect: {redDice} - need {user.UsedMove.EffectRoll} to activate.");

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

                    case EffectType.HALFLEVEL:
                        ApplyHalfLevelEffect(effect, user.UsedMove, isAttacker);
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

        private static void ApplyHalfLevelEffect(EffectMove effect, Move userMove, bool isAttacker)
        {
            bool targetIsEnemy = effect.TargetEffect == 'B';

            int baseLevel = 1;

            if (targetIsEnemy)
                baseLevel = isAttacker ? pokemonDef.LevelPokemon() : pokemonAtk.LevelPokemon();
            else
                baseLevel = isAttacker ? pokemonAtk.LevelPokemon() : pokemonDef.LevelPokemon();

            userMove.Power = Math.Max(1, (int)Math.Floor(baseLevel / 2.0));
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
                    battleLog += target.Name + " foi colocado para dormir!\n";
                    target.ConditionCount = BattleConditions.SleepRoll();
                    if (!isAttacker)
                        pokemonAtk.CanAttack = false;
                    else
                        pokemonDef.CanAttack = false;

                    target.ConditionCount--;
                    if (target.ConditionCount == 0)
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        Console.WriteLine(target.Name + " acordou!");
                        if (!isAttacker)
                            pokemonAtk.CanAttack = true;
                        else
                            pokemonDef.CanAttack = true;
                    }
                }

                else if (status == StatusConditions.PARALYZED)
                {
                    battleLog += target.Name + " foi paralisado!\n";
                    if (!BattleConditions.ParalyzedRoll())
                    {
                        battleLog += target.Name + " está paralisado e não pode atacar!\n";
                        if (isAttacker)
                            pokemonDef.CanAttack = false;
                        else
                            pokemonAtk.CanAttack = false;
                    }
                }

                else if (status == StatusConditions.FROZEN)
                {
                    battleLog += target.Name + " foi congelado!\n";
                    if (!BattleConditions.FrozenRoll())
                    {
                        battleLog += target.Name + " está congelado e não pode atacar!\n";
                        if (isAttacker)
                            pokemonDef.CanAttack = false;
                        else
                            pokemonAtk.CanAttack = false;
                    }
                    else
                    {
                        target.Conditions = StatusConditions.NORMAL;
                        battleLog += target.Name + " descongelou e pode atacar!\n";
                        if (isAttacker)
                            pokemonDef.CanAttack = true;
                        else
                            pokemonAtk.CanAttack = true;
                    }
                }
            }
        }

        private static void CardEffectBattle(string card, BattleModifications trainer)
        {
            string[] cardsEffects = card.Split(';');
            foreach (string effect in cardsEffects)
            {
                string[] effectSplit = effect.Split('.');
                switch (effectSplit[0])
                { 
                    case "roll":
                        trainer.CardBonus += int.Parse(effectSplit[1]);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
