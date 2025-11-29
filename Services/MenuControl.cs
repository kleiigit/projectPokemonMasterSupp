
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
                        BattleConfiguration.BattleWildPokemon(activeProfile);
                        break;
                    case 1: // Battle Trainer
                        BattleConfiguration.BattleTrainer(true);
                        break;
                    case 2: // Pokedex
                        List<string> _list = new List<string>() { "Pokémon", "Move"};
                        int menu = ConsoleMenu.ShowMenu(ConsoleColor.Yellow, _list, "Select Pokédex");
                        if (menu == 0) PokedexService.PokedexByType();
                        else PokedexService.MovePokedexByType();
                            break;
                    case 3: // PartyPokemon
                        int pkmIndex = ConsoleMenu.ShowMenu(ConsoleColor.White, activeProfile.ListPokemon.Select(m => m.GetName()).ToList(), "Party Pokémon of " + activeProfile.Nickname);
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
                        SimulacroMenu();
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
            int index = ConsoleMenu.ShowMenu(ConsoleColor.Magenta, _optionPokemon, "Menu Pokemon " + pokemon.GetName());
            switch (index)
            {
                case 0:
                    Console.WriteLine(pokemon.SummaryProfile());
                    break;
                case 1:
                    pokemon.AttachCard = activeProfile.UseItemCard(TypeItemCard.Attach);
                    if (pokemon.AttachCard != null) Console.WriteLine($"{pokemon.AttachCard.Name} was attached to " + pokemon.GetName());
                    break;
                case 2:
                    ItemCard? cardAction = activeProfile.UseItemCard(TypeItemCard.Action);
                    break;
                case 3:
                    if (ConsoleMenu.ShowYesNo("Do you want release " + pokemon.Name))
                    {
                        activeProfile.ListPokemon.Remove(pokemon);
                        Console.WriteLine(pokemon.GetName() + " was released!");
                    }
                    break;
                case 4: break; // cancel
                case 5:
                    pokemon.EvolutionPokemon();
                    break;
            }
            Console.ReadLine();
        }
        public static void SimulacroMenu()
        {
            TierSimulator.ListTier();
            List<string> _optionSimulacro = new List<string>() { "Compare VS", "EspecificPokemon", "CheckPokemon", "Cancel" };
            int index = ConsoleMenu.ShowMenu(ConsoleColor.Magenta, _optionSimulacro, "Menu Simulacro ");
            switch (index)
            {
                case 0:
                    VictoryChanceService.ComparePokemon();
                    break;
                case 1:
                    Pokemon pokemon = PokedexService.SelectPokemon("Pokémon to Simulation");
                    TierSimulator.EspecificPokemon(pokemon);
                    break;
            }
        }
    }
}
