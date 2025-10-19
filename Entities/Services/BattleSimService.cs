using AuxiliarCampanha.Entities.Services;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Services;
using System.Linq;

namespace ProjetoPokemon.Entities.Services
{
    static class BattleSimService // classe que controla as simulações de combate
    {
        private static int attackerDicesRoll = 0;
        private static int defenderDicesRoll = 0;
        private static ProfilePokemon pokemonAttacker;
        private static ProfilePokemon pokemonDefender;
        

        public static void SelectPokemon(List<ProfilePokemon> pokemons)
        {
            int index = ConsoleMenu.ShowMenu(pokemons.Select(m => m.ToString()).ToList(), "Escolha o Pokémon atacante");
            pokemonAttacker = pokemons[index];
            index = ConsoleMenu.ShowMenu(pokemons.Select(m => m.ToString()).ToList(), "Escolha o Pokémon defensor");
            pokemonDefender = pokemons[index];

            BattleControl();
        }
        public static void BattleControl()
        {
            int totalAttacker = 0;
            int totalDefender = 0;

            Move attackerMove = new Move(0, TypePokemon.None, "Sem ataque", 0, 6);
            Move defenderMove = new Move(0, TypePokemon.None, "Sem ataque", 0, 6);

            if (MoveStatusCheck(pokemonAttacker))
            {
                attackerMove = pokemonAttacker.Pokemon.SelectMove();
            }

            if (MoveStatusCheck(pokemonDefender))
            {
                defenderMove = pokemonDefender.Pokemon.SelectMove();
            }
            

            // Efeitos de cartas deveriam ser colocados aqui.

            EffectMove(attackerMove, true);
            EffectMove(defenderMove, false);
            // Effeito do move é ativado aqui

            CheckStats(pokemonAttacker, attackerMove);
            CheckStats(pokemonDefender, defenderMove);

            while(totalDefender == totalAttacker)
            {
                
                int rollA = RollCombatDices(attackerMove, attackerDicesRoll);
                int effvA = EffectiveTypeService.GetTypeModifier(attackerMove.Type, pokemonDefender.Pokemon.Type);
                int rollB = RollCombatDices(defenderMove, defenderDicesRoll);
                int effvB = EffectiveTypeService.GetTypeModifier(defenderMove.Type, pokemonAttacker.Pokemon.Type);
                string statusDisplayAtk = pokemonAttacker.Name;
                string statusDisplayDef = pokemonDefender.Name;
                if (pokemonAttacker.Conditions != StatusConditions.NORMAL)
                {
                    statusDisplayAtk = pokemonAttacker.Name + $" ({pokemonAttacker.Conditions})";
                }

                totalAttacker = (rollA + effvA) + attackerMove.Power + pokemonAttacker.LevelPokemon(); // falta outros modificadores
                Console.WriteLine();
                Console.WriteLine($"{statusDisplayAtk} usou {attackerMove.Name} com total de " + totalAttacker);
                Console.WriteLine($"lv: {pokemonAttacker.LevelPokemon()}, roll ({rollA}/{effvA}), power: {attackerMove.Power}");

                totalDefender = (rollB + effvB) + defenderMove.Power + pokemonDefender.LevelPokemon(); // falta outros modificadores
                Console.WriteLine($"{statusDisplayDef} usou {defenderMove.Name} com total de " + totalDefender);
                Console.WriteLine($"lv: {pokemonDefender.LevelPokemon()}, roll ({rollB}/{effvB}), power: {defenderMove.Power}");

                if (totalAttacker == totalDefender)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Empate! Role novamente os dados.\n");
                    Console.ResetColor();
                    Console.ReadLine();
                }
                else
                {
                    string victoryPokemon = totalAttacker > totalDefender ? "Atacante " + pokemonAttacker.Name : "Defensor " + pokemonAttacker.Name;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(victoryPokemon + " venceu a batalha!");
                    Console.ResetColor();
                }
            }
            

        }
        private static bool MoveStatusCheck(ProfilePokemon pokemon)
        {
            if (pokemon.Conditions == StatusConditions.PARALYZED)
            {
                if (!BattleConditions.ParalyzedRoll())
                {
                    return false;
                }
            }
            else if (pokemon.Conditions == StatusConditions.SLEEP)
            {
                pokemon.ConditionCount--;
                if (pokemon.ConditionCount == 0) { Console.WriteLine(pokemon.Name + " acordou!"); pokemon.Conditions = StatusConditions.NORMAL; return true; }
                else return false;
            }
            else if (pokemon.Conditions == StatusConditions.FROZEN)
            {
                if (!BattleConditions.FrozenRoll())
                {
                    pokemon.Conditions = StatusConditions.NORMAL;
                    Console.WriteLine(pokemon.Name + " saiu do congelamento!");
                    return true;
                }
                else return false;
            }
            return true;
        }
        private static void CheckStats(ProfilePokemon pokemon, Move move)
        {
            if (pokemon.Conditions != StatusConditions.NORMAL)
            {
                StatusConditions status = pokemon.Conditions;
                string messageSts = pokemon.Name + " está ";
                bool attack = true;
                switch (status)
                {
                    case StatusConditions.PARALYZED:
                            Console.WriteLine("paralizado!");
                            attack = false;
                        break;
                    case StatusConditions.FROZEN:
                        Console.WriteLine("congelado!");
                        attack = false;
                        break;
                    case StatusConditions.BURNED:
                        Console.WriteLine("queimado!");
                        move.Power = move.Power > 0 ? move.Power-- : 0;
                        break;
                    case StatusConditions.SLEEP:
                        Console.WriteLine("dormindo!");
                        attack = false;
                        break;
                }
                if(!attack)
                {
                    move = new Move(0, TypePokemon.None, "Sem ataque", 0, 6);
                }
            }
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
                Console.WriteLine($"{move.Name} - Rolagem {i + 1}: {roll}");
            }

            int result;
            if (moveDicesRoll < 0)
                result = rolls.Min(); // Desvantagem → pega o menor
            else if (moveDicesRoll > 0)
                if (move.Effects.Any(p => p.EffectType == EffectType.SOMADICES))
                {
                    Console.WriteLine("soma");
                    return rolls.Sum();
                }
                else result = rolls.Max(); // Vantagem → pega o maior
            else
                result = rolls[0]; // Normal → único dado

            return result;
        }

        private static void EffectMove(Move userMove, bool isAttacker)
        {
            if (userMove?.Effects == null || userMove.Effects.Count == 0)
                return;

            int redDice = DiceRollService.RollD6();

            if (userMove.EffectRoll > 1)
                Console.WriteLine($"Efeito de {userMove.Name}: ({redDice}/{userMove.EffectRoll})");

            if (redDice < userMove.EffectRoll)
                return;
            StatusConditions effectCondition = StatusConditions.NORMAL;
            foreach (var effect in userMove.Effects)
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
                        ApplyHalfLevelEffect(effect, userMove, isAttacker);
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
                baseLevel = isAttacker ? pokemonDefender.LevelPokemon() : pokemonAttacker.LevelPokemon();
            else
                baseLevel = isAttacker ? pokemonAttacker.LevelPokemon() : pokemonDefender.LevelPokemon();

            userMove.Power = Math.Max(1, (int)Math.Floor(baseLevel / 2.0));
        }

        private static void StatusConditionsTrigger(EffectMove effect, StatusConditions status, bool isAttacker)
        {
            bool targetIsEnemy = effect.TargetEffect == 'B';
            ProfilePokemon target;

            // Determina o alvo correto
            if (targetIsEnemy)
                target = isAttacker ? pokemonDefender : pokemonAttacker;
            else // 'W' ou aliado
                target = isAttacker ? pokemonAttacker : pokemonDefender;

            // Aplica a condição apenas se o Pokémon estiver em NORMAL
            if (target.Conditions == StatusConditions.NORMAL)
            {
                target.Conditions = status;
                if (status == StatusConditions.SLEEP)
                {
                    target.ConditionCount = BattleConditions.SleepRoll();
                }
            }
        }

    }
}
