using AuxiliarCampanha.Entities.Services;
using ProjetoPokemon.Services;
using System.Globalization;

namespace ProjetoPokemon.Entities.Services
{
    static class BattleSimService // classe que controla as simulações de combate
    {
        public static void Simulate(List<Pokemon> pokemons, int pokemonA, int pokemonB)
        {
            Pokemon A = pokemons.First(p => p.NumberID == pokemonA);
            Pokemon B = pokemons.First(p => p.NumberID == pokemonB);
            Combat(A, B);
            Combat(B, A);   
        }
        private static void Combat(Pokemon attacker, Pokemon defender)
        {
            var move = attacker.SelectMove();
            if (move == null) return;

            // Efetividade do efeito
            int effectValue = 0;
            if (move.EffectRoll != 0)
            {
                int roll = DiceRollService.RollD6();
                if (roll >= move.EffectRoll)
                {
                    effectValue = roll; // por enquanto, só retorna algum número
                    // chamar método de efeito
                }
            }

            // Rolagem de ataque
            int rollMove = DiceRollService.RollDice(1, move.DiceSides);
            Console.WriteLine($"Roll = {rollMove}!");
            switch (rollMove)
            {
                case 7:
                    rollMove = 5;
                    break;
                case 8:
                    rollMove = 6;
                    break;
                default:
                    break;
            }
                
            int attackRoll = rollMove + effectValue;

            // Modificador de tipo
            int effective = EffectiveTypeService.GetTypeModifier(move.Type, defender.Type);
            // Adiciona nível do Pokémon
            int TotalRoll = attacker.LevelBase + effective + move.Power + attackRoll;

            Console.WriteLine($"{attacker.Name} usa {move.Name} em {defender.Name}, Attack Score: {TotalRoll}");
            Console.WriteLine($"level: {attacker.LevelBase}, roll: ({attackRoll}, {effective}), power: {move.Power}");
        }
    }
}
