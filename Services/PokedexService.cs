using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
{
    static class PokedexService
    {
        private static TypePokemon ByTypeFilter(string text)
        {
            int selectType = ConsoleMenu.ShowMenu(ConsoleColor.Cyan, Enum.GetValues<TypePokemon>()
                .Cast<TypePokemon>().Where(t => t != TypePokemon.None) // Exclui o tipo "None"
                .Select(t => t.ToString()).ToList(), text);
            var enumValues = Enum.GetValues(typeof(TypePokemon)).Cast<TypePokemon>().ToArray();

            TypePokemon chosenType = TypePokemon.None;
            if (selectType >= 0 && selectType < enumValues.Length)
                chosenType = enumValues[selectType];
            else if (selectType - 1 >= 0 && selectType - 1 < enumValues.Length)
                chosenType = enumValues[selectType - 1];
            return chosenType;
        }
        public static Pokemon SelectPokemon(string text)
        {
            TypePokemon chosenType = ByTypeFilter("Select a Pokemon Type to filter:");
            var filteredPokemons = DataLists.AllPokemons
                .Where(p => p.Type == chosenType)
                .ToList();
            int n = ConsoleMenu.ShowMenu(ConsoleColor.DarkGreen, filteredPokemons.Select(p => p.ToString()).ToList(), $"Select Pokemon {text}:");
            return filteredPokemons[n];
        }
        public static void PokedexByType()
        {
            TypePokemon chosenType = ByTypeFilter("Select a Pokemon Type to filter:");
            var filteredPokemons = DataLists.AllPokemons
                .Where(p => p.Type == chosenType)
                .ToList();
            Console.Write("---- Filtrados: "); Console.WriteLine(filteredPokemons.Count + $" Pokémon {chosenType.ToString().ToUpper()}-Type.");
            foreach (var pokemon in filteredPokemons) Console.WriteLine(pokemon);
            Console.ReadLine();
        }
        public static void MovePokedexByType()
        {
            TypePokemon chosenType = ByTypeFilter("Select a Move Type to filter:");
            List<Move> filteredMoves = DataLists.AllMoves.Where(p => p.Type == chosenType).OrderBy(p => p.Name).ToList();
            int moveSelect = ConsoleMenu.ShowMenu(ConsoleColor.Cyan, filteredMoves.Select(p => p.Name).ToList(), $"List {chosenType.ToString().ToUpper()}-type moves");
            if (moveSelect >= 0 && moveSelect < filteredMoves.Count)
            {
                Move selectedMove = filteredMoves[moveSelect];
                Console.WriteLine(selectedMove.SummaryMove());
                Console.ReadLine();
            }

        }
    }
}
