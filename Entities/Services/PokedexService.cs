using ProjetoPokemon.Entities.Data;
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoPokemon.Entities.Services
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
                .Where(p => p.Type == chosenType || p.StabType == chosenType)
                .ToList();
            Console.Write("---- Filtrados: "); Console.WriteLine(filteredPokemons.Count + " Pokémon."); ;
            foreach (var pokemon in filteredPokemons) Console.WriteLine(pokemon);
        }
    }
}
