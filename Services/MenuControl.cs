
using DocumentFormat.OpenXml.Drawing;
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjetoPokemon.Services
{
    enum Languages
    {
        English,
        Portuguese,
        Spanish,
        Japanese,
    }
    internal class MenuControl
    {
        static BoxPokemon activeProfile = DataLists.AllProfiles[0];
        static List<string> _optionMenu = new List<string>() { "Tall Grass", "Battle", "Podedex", "Party Pokemon", "Display Profile", "Select Profile", "Preferences", "Close" };



        public static void ShowMenuOptions()
        {
            int index = 0;
            do
            {
                index = ConsoleMenu.ShowMenu(ConsoleColor.Magenta, _optionMenu, "Menu Options");
                switch (index)
                {
                    case 0: // Battle Wild Pokemon
                        BattleSimService.BattleWildPokemon(activeProfile);
                        break;
                    case 1: // Battle Trainer
                        BattleSimService.BattleTrainerPVP();
                        break;
                    case 2: // Pokedex
                        PokedexService.FilterPokedexByType();
                        break;
                    case 3: // PartyPokemon
                        int pkmIndex = ConsoleMenu.ShowMenu(ConsoleColor.White, activeProfile.ListPokemon.Select(m => m.Name).ToList(), "Party Pokémon of " + activeProfile.Nickname);
                        PokemonMenu(activeProfile.ListPokemon[pkmIndex]);
                        break;
                    case 4: // Display Profile
                        Console.WriteLine(activeProfile.DescriptionBox());
                        Console.ReadLine();
                        break;
                    case 5: // Change Profile
                        activeProfile = BoxPokemon.ChooseProfileTrainer();
                        break;
                    case 6: // preferences
                        break;
                    case 7: // close
                        break;

                    default: break;
                }

            } while (index != 7);

        }
        public static void PokemonMenu(ProfilePokemon pokemon)
        {
            List<string> _optionPokemon = new List<string>() { "Summary", "Attach Item", "Use Item", "Release", "Cancel"};
            if (pokemon.LevelExp >= pokemon.Pokemon.ExpToEvolve && pokemon.Pokemon.ExpToEvolve != 0) _optionPokemon.Add("*Evolve");
            int index = ConsoleMenu.ShowMenu(ConsoleColor.Magenta, _optionPokemon, "Menu Pokemon " + pokemon.Name);
            switch (index)
            {
                case 0:
                    break;
                case 1:
                    pokemon.AttachCard = activeProfile.SelectItem(TypeItemCard.Attach);
                    if (pokemon.AttachCard != null) Console.WriteLine($"{pokemon.AttachCard.Name} was attached to " + pokemon.Name);
                    break;
                case 2:
                    ItemCard? cardAction = activeProfile.SelectItem(TypeItemCard.Action);
                    break;
                case 3:
                    if (ConsoleMenu.ShowYesNo("Do you want release " + pokemon.Name))
                    {
                        activeProfile.ListPokemon.Remove(pokemon);
                        Console.WriteLine(pokemon.Name + " was released!");
                    }
                    break;
                case 4: break;
                case 5:
                    pokemon.EvolutionPokemon();
                    break;
            }
            Console.ReadLine();
        }
    }
}
