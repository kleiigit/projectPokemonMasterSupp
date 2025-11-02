using ProjetoPokemon.Data;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
{
    static class PokedexService
    {
        public static void FilterPokedexByType()
        {

            int selectType = ConsoleMenu.ShowMenu(ConsoleColor.Cyan, Enum.GetValues<TypePokemon>()
                .Cast<TypePokemon>().Where(t => t != TypePokemon.None) // Exclui o tipo "None"
                .Select(t => t.ToString()).ToList(), "Select a Pokemon Type to filter:");
            var enumValues = Enum.GetValues(typeof(TypePokemon)).Cast<TypePokemon>().ToArray(); TypePokemon chosenType;
            if (selectType >= 0 && selectType < enumValues.Length)
                chosenType = enumValues[selectType];
            else if (selectType - 1 >= 0 && selectType - 1 < enumValues.Length)
                chosenType = enumValues[selectType - 1];
            else return;

            var filteredPokemons = DataLists.AllPokemons
                .Where(p => p.Type == chosenType)
                .ToList();
            Console.Write("---- Filtrados: "); Console.WriteLine(filteredPokemons.Count + $" Pokémon {chosenType.ToString().ToUpper()}-Type."); ;
            foreach (var pokemon in filteredPokemons) Console.WriteLine(pokemon);
            Console.ReadLine();
        }
    }
}
