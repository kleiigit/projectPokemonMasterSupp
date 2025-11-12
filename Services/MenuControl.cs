
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
        static List<string> _optionMenu = new List<string>() { "Tall Grass", "Battle", "Podedex", "Party Pokemon", "Display Profile", "Select Profile", "Simulacro", "Preferences", "Close" };



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
                        BattleSimService.BattleTrainer(true);
                        break;
                    case 2: // Pokedex
                        List<string> _list = new List<string>() { "Pokémon", "Move"};
                        int menu = ConsoleMenu.ShowMenu(ConsoleColor.Yellow, _list, "Select Pokédex");
                        if (menu == 0) PokedexService.PokedexByType();
                        else PokedexService.MovePokedexByType();
                            break;
                    case 3: // PartyPokemon
                        int pkmIndex = ConsoleMenu.ShowMenu(ConsoleColor.White, activeProfile.ListPokemon.Select(m => m.NickPokemon).ToList(), "Party Pokémon of " + activeProfile.Nickname);
                        PokemonMenu(activeProfile.ListPokemon[pkmIndex]);
                        break;
                    case 4: // Display Profile
                        Console.WriteLine(activeProfile.DescriptionBox());
                        Console.ReadLine();
                        break;
                    case 5: // Change Profile
                        activeProfile = BoxPokemon.ChooseProfileTrainer();
                        break;
                    case 6: // simulacro
                        VictoryChanceService.ComparePokemon();
                        Console.ReadLine();
                        break;
                    case 7: // preferences
                        break;
                    case 8: // close
                        break;

                    default: break;
                }

            } while (index != 8);

        }
        public static void PokemonMenu(ProfilePokemon pokemon)
        {
            List<string> _optionPokemon = new List<string>() { "Summary", "Attach Item", "Use Item", "Release", "Cancel"};
            if (pokemon.LevelExp >= pokemon.Pokemon.ExpToEvolve && pokemon.Pokemon.ExpToEvolve != 0) _optionPokemon.Add("*Evolve");
            int index = ConsoleMenu.ShowMenu(ConsoleColor.Magenta, _optionPokemon, "Menu Pokemon " + pokemon.NickPokemon);
            switch (index)
            {
                case 0:
                    Console.WriteLine(pokemon.SummaryProfile());
                    break;
                case 1:
                    pokemon.AttachCard = activeProfile.SelectItem(TypeItemCard.Attach);
                    if (pokemon.AttachCard != null) Console.WriteLine($"{pokemon.AttachCard.Name} was attached to " + pokemon.NickPokemon);
                    break;
                case 2:
                    ItemCard? cardAction = activeProfile.SelectItem(TypeItemCard.Action);
                    break;
                case 3:
                    if (ConsoleMenu.ShowYesNo("Do you want release " + pokemon.NickPokemon))
                    {
                        activeProfile.ListPokemon.Remove(pokemon);
                        Console.WriteLine(pokemon.NickPokemon + " was released!");
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
