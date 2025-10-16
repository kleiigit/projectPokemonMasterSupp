
using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class EffectMove
    {
        public char TargetEffect { get; set; }
        public EffectType EffectType { get; set; }
        public string DescriptionEffect { get; set; }

        public EffectMove(char targetEffect, EffectType effectName)
        {
            string targetColor = "Alvo";
            if (targetEffect == 'B') targetColor = "the Opponent";
            else targetColor = "himself"; 

            TargetEffect = targetEffect;
            EffectType = effectName;
            DescriptionEffect = EffectDescription();
        }

        public EffectMove(EffectType effectName) // sem alvo
        {
            EffectType = effectName;
            DescriptionEffect = EffectDescription();
        }

        private string EffectDescription()
        {
            string description = "";
            switch (EffectType)
            {
                case EffectType.POISON:
                    description = "Poison " + TargetEffect;
                    break;
                case EffectType.BURN:
                    description = "Burn " + TargetEffect;
                    break;
                case EffectType.PARALYZE:
                    description = "Paralyze " + TargetEffect;
                    break;
                case EffectType.SLEEP:
                    description = "Put to Sleep " + TargetEffect;
                    break;
                case EffectType.FREEZE:
                    description = "Freeze " + TargetEffect;
                    break;
                case EffectType.CONFUSION:
                    description = "Confuse " + TargetEffect;
                    break;

                case EffectType.TWODICES:
                    description = TargetEffect + " roll two dices";
                    //if (TargetEffect == "himself") description += " and choose the highest result";
                    // else description += " and choose the worst result";
                    break;

                case EffectType.RECHARGE:
                    description = "this move needs to recharge";
                    break;
                case EffectType.KO:
                    description = "this move can cause instant KO in user";
                    break;
                case EffectType.RAIN:
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
