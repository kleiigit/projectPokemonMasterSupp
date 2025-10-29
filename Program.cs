using ProjetoPokemon.Entities.Data;
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBaseControl.DataBase();
            Console.ReadLine();

            BattleSimService.BattleWildPokemon(DataLists.AllProfiles[0]);
            
            // PokedexService.FilterPokedexByType();
            // if (ConsoleMenu.ShowYesNo("Do you want create a new Trainer?"))  BoxPokemon.CreateTrainer();
            //BattleSimService.BattleTrainerPVP();
            DataBaseControl.SaveProfiles();

        }
    }
}