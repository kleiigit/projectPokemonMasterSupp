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
            string description = TargetEffect == 'W' ? "This Pokemon" : "The opponent Pokémon";
            switch(EffectType)
            {
                case EffectType.POISON:
                    description += " can be poisoned.";
                    break;
                case EffectType.PARALYZE:
                    description += " can be paralyzed.";
                    break;
                case EffectType.SLEEP:
                    description += " can be put to sleep.";
                    break;
                case EffectType.BURN:
                    description += " can be burned.";
                    break;
                case EffectType.FREEZE:
                    description += "  can be frozen.";
                    break;
                case EffectType.CONFUSION:
                    description += " may get confused.";
                    break;


                case EffectType.RECHARGE:
                    description = "This move can only be used once per battle.";
                    break;
                case EffectType.KO:
                    description += " can be knocked out.";
                    break;
                case EffectType.FIRST:
                    description += " ignores the effects of the opponent's Pokémon's move.";
                    break;
                case EffectType.HALFLEVEL:
                    description = "The power of this attack is equal to half of the";
                    description += TargetEffect == 'W' ? " this Pokémon's level." : " opponent's Pokémon's level.";
                    break ;
                case EffectType.PRECISION:
                    description = "If the roll is less than 3, it becomes 3.";
                    break;


                case EffectType.TWODICES:
                    description += " can use two dice and choose the";
                    description += TargetEffect == 'W' ? " best result." : " worst result.";
                    break;
                case EffectType.SOMADICES:
                    description += " can add the attack dice roll.";
                    break;


                default:
                    description = "Effect Description.";
                    return description;
            }
            return description;
        }

        override public string ToString()
        {
            return DescriptionEffect;
        }
    }
}
