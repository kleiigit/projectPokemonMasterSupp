
using DocumentFormat.OpenXml.Drawing.Diagrams;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Managers;

namespace ProjetoPokemon.Services
{
    internal static class VictoryChanceService
    {
        static int simulationsPerMove = 100;
        public static List<string> resultString = new List<string>();
        private static FieldCards weatherCard = FieldCards.NORMAL;
        private static FieldCards fieldCard = FieldCards.NORMAL;
        private static FieldCards trapCard = FieldCards.NORMAL;

        public static void AddResult(String strings)
        {
            resultString.Add(strings);
        }
        public static void ShowResult()
        {
            foreach (string text in resultString)
            {
                Console.WriteLine(text);
            }
            resultString.Clear();
        }
        public static List<double[]> ChanceSimulator(Move usedMove, Battler battlerA, Battler battlerB)
        {
            List<double[]> winRates = new List<double[]>();
            List<FieldCards> saveField = FieldControlService.SaveFieldConfig();
            if (battlerB.MovesPokemon.Count == 0)
                battlerB.MovesPokemon.Add(Move.Null());

            foreach (var moveB in battlerB.MovesPokemon)
            {
                int winsA = 0;
                int tieA = 0;
                int lostA = 0;

                for (int i = 0; i < simulationsPerMove; i++)                
                {

                    Battler cloneA = BattlerTest.CloneBattler(battlerA);
                    Battler cloneB = BattlerTest.CloneBattler(battlerB);

                    CombatManager newCombat = new CombatManager(cloneA, cloneB);
                    newCombat.BattlerAttacker.SetUsedMove(usedMove);
                    newCombat.BattlerDefender.SetUsedMove(moveB);


                    CombatManager.Calculation(newCombat.BattlerAttacker, newCombat.BattlerDefender);
                    CombatManager.Calculation(newCombat.BattlerDefender, newCombat.BattlerAttacker);

                    // --- Verificação de nocaute imediato ---
                    bool aKnocked = newCombat.BattlerAttacker.Pokemon.Conditions == StatusConditions.KNOCKED;
                    bool bKnocked = newCombat.BattlerDefender.Pokemon.Conditions == StatusConditions.KNOCKED;

                    // --- Lógica principal ---
                    if (aKnocked && bKnocked) {                       
                        tieA++;
                    } // ambos foram nocauteados
                    else if (aKnocked && !bKnocked) lostA++; // A foi nocauteado, perde automaticamente
                    else if (!aKnocked && bKnocked) winsA++; // B foi nocauteado, A vence automaticamente
                    else
                    {
                        //Console.WriteLine(newCombat.BattlerAttacker.Total.TotalResult + " "+ newCombat.BattlerDefender.Total.TotalResult);
                        // Nenhum foi nocauteado, decidir pelo resultado numérico
                        if (newCombat.BattlerAttacker.Total.TotalResult > newCombat.BattlerDefender.Total.TotalResult)
                            winsA++;
                        else if (newCombat.BattlerAttacker.Total.TotalResult < newCombat.BattlerDefender.Total.TotalResult)
                            lostA++;
                        else
                            { tieA++;
                           
                        }
                    }
                    FieldControlService.LoadFieldConfig(saveField);
                }

                double winRate = (double)winsA / simulationsPerMove * 100;
                double tieRate = (double)tieA / simulationsPerMove * 100;
                double lostRate = (double)lostA / simulationsPerMove * 100;
                double[] rates = new double[]
                    {
                        Math.Round(winRate, 2),
                        Math.Round(tieRate, 2),
                        Math.Round(lostRate, 2)
                    };
                rates = rates.Select(r => Math.Round(r, 2)).ToArray();
                winRates.Add(rates);

                // Log opcional por movimento
                AddResult($"{usedMove.Name} vs {moveB?.Name ?? "No Move"} (W-{winRate:F0}%, L-{lostRate:F0}%, T-{tieRate:F0}%)");
            }
            
            BattleLog.ClearLogs();

            // Retorna o menor percentual (pior caso)
            return winRates;
        }
        public static void ComparePokemon()
        {
            Pokemon pokemonA = PokedexService.SelectPokemon("A");
            Pokemon pokemonB = PokedexService.SelectPokemon("B");
            TierSimulator.ComparePokemon(pokemonA, pokemonB);
        }
        public static void FieldControl()
        {
          weatherCard = FieldCards.NORMAL;
          fieldCard = FieldCards.NORMAL;
          trapCard = FieldCards.NORMAL;
        }
        
    }
}
