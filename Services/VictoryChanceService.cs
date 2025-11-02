
using ProjetoPokemon.Entities;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Services
{
    internal static class VictoryChanceService
    {
        static int simulationsPerMove = 1000;
        private static readonly Move nullMove = new Move(0, TypePokemon.None, "No Move", 0, 6);
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
        public static double ChanceSimulator(Move usedMove, BattlerManager battlerA, BattlerManager battlerB)
        {
            BattlerManager tempA = CloneBattler(battlerA);
            BattlerManager tempB = CloneBattler(battlerB);
            if (tempB.SelectedPokemon.MovesPokemon.Count == 0)
                tempB.SelectedPokemon.MovesPokemon.Add(nullMove);

            List<double> winRates = new List<double>();
            List<FieldCards> saveField = FieldControlService.SaveFieldConfig();
            foreach (var moveB in tempB.SelectedPokemon.MovesPokemon)
            {
                int winsA = 0;

                for (int i = 0; i < simulationsPerMove; i++)
                {
                    tempA = CloneBattler(battlerA);
                    tempB = CloneBattler(battlerB);

                    tempA.UsedMove = usedMove;
                    tempB.UsedMove = moveB;

                    // Aplica efeitos iniciais
                    if (!tempB.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST)) BattleSimService.EffectMove(tempA, tempB);
                    if (!tempA.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST)) BattleSimService.EffectMove(tempB, tempA);

                    // Executa simulação rápida
                    if (tempB.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST)) // se defensor tiver first
                    {
                        BattleSimService.BattleCalculation(tempB, tempA);
                        BattleSimService.EffectMove(tempA, tempB);
                        BattleSimService.BattleCalculation(tempA, tempB);

                    }
                    else if (tempA.UsedMove.Effects.Any(p => p.EffectType == EffectType.FIRST))
                    {
                        BattleSimService.BattleCalculation(tempA, tempB);
                        BattleSimService.EffectMove(tempB, tempA);
                        BattleSimService.BattleCalculation(tempB, tempA);
                    }
                    else
                    {
                        BattleSimService.BattleCalculation(tempA, tempB);
                        BattleSimService.BattleCalculation(tempB, tempA);
                    }

                    if (tempA.TotalResult > tempB.TotalResult && tempA.SelectedPokemon.Conditions != StatusConditions.KNOCKED)
                        winsA++;
                    FieldControlService.LoadFieldConfig(saveField);
                }

                double winRate = (double)winsA / simulationsPerMove * 100;
                winRates.Add(Math.Round(winRate, 2));

                // Log opcional por movimento
                AddResult($"{usedMove.Name} vs {moveB?.Name ?? "No Move"} = {winRate:F0}%");
            }

            BattleLog.ClearLogs();

            // Retorna o menor percentual (pior caso)
            return winRates.Min();
        }

        public static void FieldControl()
        {
          weatherCard = FieldCards.NORMAL;
          fieldCard = FieldCards.NORMAL;
          trapCard = FieldCards.NORMAL;
        }
        private static BattlerManager CloneBattler(BattlerManager original)
        {
            // Clona o ProfilePokemon para evitar referência compartilhada
            ProfilePokemon clonedProfile = CloneProfilePokemon(original.SelectedPokemon);
            string name = "TESTE";
            BattlerManager clone = new BattlerManager(clonedProfile, original.Setup)
            {
                
                NumberDices = original.NumberDices,
                RollBonus = original.RollBonus,
                StatusBonus = original.StatusBonus,
                WeatherBonus = original.WeatherBonus
            };
            clone.SetTrainerName(name);
            return clone;
        }
        private static ProfilePokemon CloneProfilePokemon(ProfilePokemon original)
        {
            if (original == null)
                return null;

            // Cria cópia do Pokemon base
            Pokemon clonedPokemon = new Pokemon(original.Pokemon);
            // Cria o novo ProfilePokemon
            ProfilePokemon clone = new ProfilePokemon(clonedPokemon, original.Name, original.LevelExp)
            {
                AttachCard = original.AttachCard, // referência de item pode ser compartilhada, se for estática
                Conditions = original.Conditions,
                ConditionCount = original.ConditionCount,
                CanAttack = original.CanAttack
            };

            if (original.Shiny)
                clone.SetShiny(true);

            return clone;
        }
    }
}
