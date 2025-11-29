using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Managers;

namespace ProjetoPokemon.Services
{
    internal static class TierSimulator
    {
        static int simulations = 100;
        static List<BattlerTest> battlers = new List<BattlerTest>();
        private static double[] Simulator(Pokemon pokemonA, Pokemon pokemonB)
        {
            Battler attacker = new BattlerTest(new ProfilePokemon(pokemonA), SetupBattle.Attacker);
            Battler defender = new BattlerTest(new ProfilePokemon(pokemonB), SetupBattle.Defender);
            Move moveA = attacker.SelectMove(defender);
            Move moveB = defender.SelectMove(attacker);
            double[] kda = new double[3];
            int winA = 0;
            int loseA = 0;
            int tieA = 0;

            for (int i = 0; i < simulations; i++)
            {
                CombatManager newCombat = new CombatManager(BattlerTest.CloneBattler(attacker), BattlerTest.CloneBattler(defender));
                newCombat.BattlerAttacker.SetUsedMove(moveA);
                newCombat.BattlerDefender.SetUsedMove(moveB);
                CombatManager.Calculation(newCombat.BattlerAttacker, newCombat.BattlerDefender);
                CombatManager.Calculation(newCombat.BattlerDefender, newCombat.BattlerAttacker);

                /*
                Console.WriteLine(newCombat.BattlerAttacker + $" ({newCombat.BattlerAttacker.GetUsedMoveName()}) = " + newCombat.BattlerAttacker.Total.TotalResult + " vs " +
                    newCombat.BattlerDefender + $" ({newCombat.BattlerDefender.GetUsedMoveName()}) = " + newCombat.BattlerDefender.Total.TotalResult);
                */
                if (newCombat.BattlerAttacker.Total.TotalResult > newCombat.BattlerDefender.Total.TotalResult)
                {
                    winA++;
                }
                else if (newCombat.BattlerAttacker.Total.TotalResult == newCombat.BattlerDefender.Total.TotalResult)
                {
                    tieA++;
                }
                else
                {
                    loseA++;
                }
            }
            kda[0] = (double)winA / simulations * 100;
            kda[1] = (double)loseA / simulations * 100;
            kda[2] = (double)tieA / simulations * 100;
            return kda;
           } 

        public static void EspecificPokemon(Pokemon pokemonA)
        {
            BattlerTest find = battlers.First(p => p.Pokemon.Name == pokemonA.Name);
            Console.WriteLine(find.Pokemon.Name + " - Point: " + find.PointTier + $" (V: {find.winPokemon.Count} - D: {find.losePokemon.Count} - T: {find.tiePokemon.Count})\n");
            Console.WriteLine("Win");
            foreach (var win in find.winPokemon)
            {
                Console.WriteLine($"{win.Key.Name} - (V: {win.Value[0].ToString("F0")}% - D: {win.Value[1].ToString("F0")}% - T: {win.Value[2].ToString("F0")}%)");
            }

            Console.WriteLine("\nDefeat");
            foreach (var win in find.losePokemon)
            {
                Console.WriteLine($"{win.Key.Name} - (V: {win.Value[0].ToString("F0")}% - D: {win.Value[1].ToString("F0")}% - T: {win.Value[2].ToString("F0")}%)");
            }

            Console.WriteLine("\nTie");
            foreach (var win in find.tiePokemon)
            {
                Console.WriteLine($"{win.Key.Name} - (V: {win.Value[0].ToString("F0")}% - D: {win.Value[1].ToString("F0")}% - T: {win.Value[2].ToString("F0")}%)");
            }
        }
        public static void ComparePokemon(Pokemon A, Pokemon B)
        {
            double[] kda = Simulator(A, B);
            Console.WriteLine("Simulation: \n" + A.Name + " VS " + B.Name + $"\n " +
                $"V: {kda[0].ToString("F0")}% " +
                $"D: {kda[1].ToString("F0")}% " +
                $"T: {kda[2].ToString("F0")}% - ");
        }
        private static void AllPokemon()
        {
            Console.WriteLine("Loading Simulacro (ID:" + DataLists.AllPokemons.Count + ")");
            foreach (var pokemonA in DataLists.AllPokemons)
            {
                BattlerTest battler = new BattlerTest(new ProfilePokemon(pokemonA), SetupBattle.Attacker);
                double[] kda = new double[3];
                
                Console.Write(pokemonA.NumberID + ",");
                foreach (var pokemonB in DataLists.AllPokemons)
                {
                    kda = Simulator(pokemonA, pokemonB);
                    if (battler is BattlerTest test)
                    {
                        if (kda[0] > kda[1])
                        {
                            test.PointTier += 5;
                            test.winPokemon.Add(pokemonB, kda);
                        }
                        else if (kda[0] == kda[1])
                        {
                            test.PointTier += 1;
                            test.tiePokemon.Add(pokemonB, kda);
                        }
                        else
                        {
                            test.PointTier -= 1;
                            test.losePokemon.Add(pokemonB, kda);
                        }
                    }
                    
                }
                battlers.Add(battler);
            }
            Console.Beep();
            Console.WriteLine();
        }
        public static void ListTier()
        {
            if (battlers.Count != 0) { return; }
            battlers = new List<BattlerTest>();
            List<string> content = new List<string>();
            AllPokemon();
            List<BattlerTest> list = battlers.OrderByDescending(p => p.PointTier).ToList();
            foreach (var pokemon in list)
            {
                content.Add(pokemon.Pokemon.Name + " - Point: " + pokemon.PointTier + $" (V: {pokemon.winPokemon.Count} - D: {pokemon.losePokemon.Count} - T: {pokemon.tiePokemon.Count})");
            }
            DataBaseControl.SaveSimulations(content);
        }
    }
}
