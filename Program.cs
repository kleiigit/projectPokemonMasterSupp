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

            //PokedexService.FilterPokedexByType(pokemons);

            Console.ReadLine();
            if (ConsoleMenu.ShowYesNo("Do you want create a new Trainer?"))
            {
                Console.Write("Write a new trainer name: ");
                string newTrainerName = Console.ReadLine();
                if(string.IsNullOrEmpty(newTrainerName))
                    { newTrainerName = "Trainer"; }
                BoxPokemon newProfile = new BoxPokemon(newTrainerName);
                newProfile.CreateBox(pokemons);
                profiles.Add(newProfile);
            }

            BattleSimService.BattlePokemonSetup(profiles);

            DataBaseControl.SaveProfiles(profiles);

        }
    }
}