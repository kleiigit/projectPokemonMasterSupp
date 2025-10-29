using ProjetoPokemon.Entities.Data;
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Profiles;

namespace ProjetoPokemon.Entities.Services
{
    static class WildGeneratorService
    {
        static Random random = new Random();
        static int shinyChance = 10; // 10 em 100
        public static ProfilePokemon WildByColor(ColorToken color)
        {
            List<Pokemon> pokemonColor = DataLists.AllPokemons.Where(p => p.Color == color).ToList();
            int n = random.Next(0, pokemonColor.Count);
            ProfilePokemon randomPokemon = new ProfilePokemon(pokemonColor[n], "Wild " + pokemonColor[n].Name, 0);
            randomPokemon.SetShiny(ShinyChance());
            return randomPokemon;
        }
        public static bool ShinyChance()
        {
            int n = random.Next(1, 100);
            return n <= shinyChance;
        }
        public static bool CatchWildPokemon(int rate, int bonus)
        {
            int n = DiceRollService.RollD6();
            if (n + bonus >= rate)
            {
                Console.Write($"CATCH ROLL: {n}! You caught the wild Pokémon!");
                Console.ReadLine();
                return true;
            }
            else
            {
                Console.Write($"CATCH ROLL: {n}! The wild Pokémon broke free!");
                Console.ReadLine();
                return false;
            }
        }

    }
}
