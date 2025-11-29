
using ProjetoPokemon.Entities.Battlers;

namespace ProjetoPokemon.Services
{
    internal class TotalCalculationService
    {
        public TotalCalculationService()
        {
        }

        public int NumberDices { get; set; }
        public int RollBonus { get; set; }
        public int EffectiveBonus { get; set; }
        public int Roll { get; set; }
        public int[] Rolls { get; set; } = [];
        public int CardBonus { get; set; }
        public int StatusBonus { get; set; }
        public int WeatherBonus { get; set; }
        public int TotalResult { get; set; }

        public int RedRoll { get; set; }

        public void TotalPower(Battler Battler)
        {
            TotalResult = (RollBonus + Roll) + EffectiveBonus + Battler.GetUsedMovePower() + Battler.Pokemon.LevelPokemon() + StatusBonus + CardBonus + WeatherBonus;
            if (Battler is BattlerTest) { }
            else
                BattleLog.LogCalculation(Display(Battler));
        }
        public string Display(Battler Battler)
        {
            string logCalculation = $"\n{Battler} used {Battler.GetUsedMoveName()} with total of {TotalResult}.\n";
            logCalculation += $"{Roll} (Attack Roll) + {Battler.GetUsedMovePower()} (Attack Strength) + {Battler.Pokemon.LevelPokemon()}  (Level)";
            if (RollBonus != 0) logCalculation += $" + {RollBonus} (Bonus)";
            if (EffectiveBonus != 0)
            {
                if (EffectiveBonus > 0) logCalculation += $" + {EffectiveBonus} (Effective Move)";
                else logCalculation += $" {EffectiveBonus} (Weak Move)";
            }

            logCalculation += $" ";

            if (StatusBonus != 0) logCalculation += $" + {StatusBonus} (Status)";
            if (CardBonus != 0) logCalculation += $" + {CardBonus} (Battle Card)";
            if (WeatherBonus != 0) logCalculation += $" + {WeatherBonus} (Weather))";

            return logCalculation;
        }
    }
}
