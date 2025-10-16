using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            List<Move> movesList = new List<Move>();
            DataBaseControl.DataBase(pokemons, movesList);

            int index = ConsoleMenu.ShowMenu(pokemons.Select(m => m.Name).ToList(), "Escolha o Pokémon atacante");
            Pokemon pokemonAttacker = pokemons[index];
            index = ConsoleMenu.ShowMenu(pokemons.Select(m => m.Name).ToList(), "Escolha o Pokémon defensor");
            Pokemon pokemonDefender = pokemons[index];

            BattleSimService.BattleControl(pokemonAttacker, pokemonDefender);

        }
    }
}