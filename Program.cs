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
            List<BoxPokemon> profiles = new List<BoxPokemon>();
            List<ItemCard> itemCards = new List<ItemCard>();

            DataBaseControl.DataBase(pokemons, movesList, itemCards, profiles);
            Console.ReadLine();
            int index = ConsoleMenu.ShowMenu(profiles.Select(m => m.Nickname).ToList(), "Escolha o perfil de Treinador");
            BoxPokemon profile = profiles[index];
            Console.WriteLine(profile);

            BattleSimService.SelectPokemon(profile.ListBox);

        }
    }
}