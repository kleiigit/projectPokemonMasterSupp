using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Services;
using System.Xml.Linq;

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
            if(ConsoleMenu.ShowYesNo("Do you want create a new Trainer?"))
            {
                Console.Write("Write a new trainer name: ");
                string newTrainerName = Console.ReadLine();
                if(string.IsNullOrEmpty(newTrainerName))
                    { newTrainerName = "Trainer"; }
                BoxPokemon newProfile = new BoxPokemon(newTrainerName);
                newProfile.CreateBox(pokemons);
                profiles.Add(newProfile);
            }
            int index = ConsoleMenu.ShowMenu(profiles.Select(m => m.Nickname).ToList(), "Choose a Attacker trainer Profile");
            BoxPokemon profileAtk = profiles[index];

            index = ConsoleMenu.ShowMenu(profiles.Select(m => m.Nickname).ToList(), "Choose a Defender trainer Profile");
            BoxPokemon profileDef = profiles[index];
            Console.WriteLine($"{profileAtk}\n\n{profileDef}");
            BattleSimService.SelectPokemon(profileAtk, profileDef);
            DataBaseControl.SaveProfiles(profiles);

        }
    }
}