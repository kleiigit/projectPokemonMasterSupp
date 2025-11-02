
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

            DataLists.AllProfiles[0].DrawItemCard(20);
            // if (ConsoleMenu.ShowYesNo("Do you want create a new Trainer?"))  BoxPokemon.CreateTrainer();


            MenuControl.ShowMenuOptions();
            DataBaseControl.SaveProfiles();
        }
    }
}