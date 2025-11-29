
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;
using System.Data;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Managers;

namespace ProjetoPokemon.Services
{
    static class BattleConfiguration // classe que controla as simulações de combate
    {
        private static readonly ConsoleColor atkColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor defColor = ConsoleColor.Magenta;

        public static void BattleWildPokemon(BoxPokemon profile)
        {
            bool falseSwipe = false;
            CombatManager newCombat = new CombatManager();
            do
            {
                newCombat = new CombatManager(profile, ColorToken.Blue);
                Console.WriteLine($"Um {newCombat.BattlerDefender.Pokemon.Name} selvagem apareceu! \n{newCombat.BattlerDefender.Pokemon}");
                newCombat.Setup(newCombat.BattlerDefender.SelectMove(newCombat.BattlerAttacker));
                falseSwipe = newCombat.BattlerAttacker.GetUsedMoveID() == 196 ? true : false; // false swipe bonus

            } while (profile.ListPokemon.Any(p => p.Conditions != StatusConditions.KNOCKED) && newCombat.BattlerDefender.Pokemon.Conditions != StatusConditions.KNOCKED);

            if (ConsoleMenu.ShowYesNo("Do you want catch " + newCombat.BattlerDefender.Pokemon.GetName() + "? - Catch Rate: +" + newCombat.BattlerDefender.Pokemon.Pokemon.CathRate))
            {
                int bonusCard = falseSwipe == true ? 1 : 0;
                if (ConsoleMenu.ShowYesNo("Do you want to use a capture item card?"))
                {
                    ItemCard? catchItem = profile.UseItemCard(TypeItemCard.Catch);
                    if (catchItem == null) Console.WriteLine("No item card can be selected.");
                    else
                    {
                        Console.WriteLine($"\n{profile.Nickname} used the item card {catchItem.Name}!");
                        bonusCard += catchItem.CatchCard(newCombat.BattlerDefender.Pokemon.Pokemon);
                    }

                }
                if (bonusCard >= 50) // masterball
                {
                    Console.WriteLine("You caught the wild Pokémon!");
                    Console.WriteLine($"{newCombat.BattlerDefender} add in your party!");
                    Console.ReadLine();
                    newCombat.BattlerDefender.Pokemon.PutNicknamePokemon();
                    profile.AddPokemon(new ProfilePokemon(newCombat.BattlerDefender.Pokemon.Pokemon, newCombat.BattlerDefender.Pokemon.Name, 0));
                    return;
                }

                if (WildGeneratorService.CatchWildPokemon(newCombat.BattlerDefender.Pokemon.Pokemon.CathRate, bonusCard))
                {
                    Console.WriteLine($"{newCombat.BattlerDefender} add in your party!");
                    Console.ReadLine();
                    newCombat.BattlerDefender.Pokemon.PutNicknamePokemon();
                    profile.AddPokemon(new ProfilePokemon(newCombat.BattlerDefender.Pokemon.Pokemon, newCombat.BattlerDefender.Pokemon.Name, 0));
                }
                else Console.ReadLine();
            }
        }
        public static void BattleTrainer(bool isNPC)
        {
            Move? typeMove = null;

            // Select Profiles
            int index = ConsoleMenu.ShowMenu(atkColor, DataLists.AllProfiles.Select(m => m.Nickname).ToList(), "Choose a Attacker trainer Profile");
            BoxPokemon ProfileA = DataLists.AllProfiles[index]; // Attacker Profile
            List<BoxPokemon> selectProfile = DataLists.AllProfiles.Where(s => s.Nickname != ProfileA.Nickname).ToList();
            index = ConsoleMenu.ShowMenu(defColor, selectProfile.Select(m => m.Nickname).ToList(), "Choose a Defender trainer Profile");
            BoxPokemon ProfileB = DataLists.AllProfiles[index]; // Defender Profile

            // istance
            

            do
            {
                CombatManager Combat = new CombatManager(ProfileA, ProfileB);

                if (Combat.BattlerAttacker.Pokemon.Conditions == StatusConditions.KNOCKED) 
                    Combat.BattlerAttacker = BattlerPlayer.CreateTrainer(ProfileA, atkColor, SetupBattle.Attacker);

                if (Combat.BattlerDefender.Pokemon.Conditions == StatusConditions.KNOCKED) 
                    Combat.BattlerDefender = BattlerPlayer.CreateTrainer(ProfileB, defColor, SetupBattle.Defender);

                if (Combat.BattlerDefender is BattlerGym) 
                    typeMove = Combat.BattlerDefender.SelectMove(Combat.BattlerAttacker);


                Combat.Setup(Combat.BattlerDefender.GetUsedMove());

            } while (ProfileA.ListPokemon.Any(p => p.Conditions != StatusConditions.KNOCKED) && ProfileB.ListPokemon.Any(p => p.Conditions != StatusConditions.KNOCKED));
            ProfileA.RecoverPokémon();
            ProfileB.RecoverPokémon();
        }
    }
}
