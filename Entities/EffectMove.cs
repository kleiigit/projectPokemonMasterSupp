
namespace ProjetoPokemon.Entities
{
    internal class EffectMove
    {
        public string? TargetEffect { get; set; }
        public string EffectName { get; set; }
        public string DescriptionEffect { get; set; }

        public EffectMove(char targetEffect, string effectName)
        {
            string targetColor = "Alvo";
            if (targetEffect == 'B') targetColor = "the Opponent";
            else targetColor = "himself"; 

            TargetEffect = targetColor;
            EffectName = effectName;
            DescriptionEffect = EffectDescription();
        }

        public EffectMove(string effectName) // sem alvo
        {
            EffectName = effectName;
            DescriptionEffect = EffectDescription();
        }

        private string EffectDescription()
        {
            string description = "";
            switch (EffectName)
            {
                case "POISON":
                    description = "Poison " + TargetEffect;
                    break;
                case "BURN":
                    description = "Burn " + TargetEffect;
                    break;
                case "PARALYZE":
                    description = "Paralyze " + TargetEffect;
                    break;
                case "SLEEP":
                    description = "Put to Sleep " + TargetEffect;
                    break;
                case "FREEZE":
                    description = "Freeze " + TargetEffect;
                    break;
                case "CONFUSION":
                    description = "Confuse " + TargetEffect;
                    break;

                case "TWODICES":
                    description = TargetEffect + " roll two dices";
                    if (TargetEffect == "himself")
                        description += " and choose the highest result";
                    else
                        description += " and choose the worst result";
                    break;

                case "RECHARGE":
                    description = "this move needs to recharge";
                    break;
                case "KO":
                    description = "this move can cause instant KO in user";
                    break;
                case "RAIN":
                    description = "Put Rain Card in battlefield";
                    break;


                default:
                    description = "No description available.";
                    break;
            }

            return description;
        }

        override public string ToString()
        {
            return DescriptionEffect;
        }
    }
}
