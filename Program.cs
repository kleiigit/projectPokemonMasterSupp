
using ProjetoPokemon.Data;
using ProjetoPokemon.Services;

namespace ProjetoPokemon
{
    class Program
    {
        static void Main(string[] args)
        {

            DataBaseControl.DataBase();
            Console.ReadLine();

            MasterGame.CreateGameItemDeck();
            // if (ConsoleMenu.ShowYesNo("Do you want create a new Trainer?"))  BoxPokemon.CreateTrainer();


            MenuControl.ShowMenuOptions();
            DataBaseControl.SaveProfiles();
        }
    }
}