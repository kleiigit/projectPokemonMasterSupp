using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class EffectMove
    {
        public char TargetEffect { get; }
        public EffectType EffectType { get; }
        public string DescriptionEffect { get; }

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
                    description += " ignores the effects of the opponent's move.";
                    break;
                case EffectType.HALFLEVEL:
                    description = "The power equals half";
                    description += TargetEffect == 'W' ? " this Pokémon's level." : " the opponent’s level.";
                    break ;
                case EffectType.PRECISION:
                    description = "If the roll is less than 3, it becomes 3.";
                    break;
                case EffectType.LIFE:
                    description = "If this Digimon loses, you may reroll the dice.";
                    break;
                case EffectType.FURY:
                    description = "If the opponent’s Power is 0, you have the advantage.";
                    break;
                case EffectType.CARD:
                    description = "You may draw an Item Card.";
                    break;
                case EffectType.IMMUNE:
                    description = "This move is not affected by the opponent’s Pokémon’s type.";
                    break;
                case EffectType.CHANGE:
                    description = "You may change ";
                    description += TargetEffect == 'B' ? " opponent's active Pokémon." : " your active Pokémon.";
                    break;



                case EffectType.TWODICES:
                    description += " use two dice and choose the";
                    description += TargetEffect == 'W' ? " best result." : " worst result.";
                    break;
                case EffectType.SOMADICES:
                    description += " can add two rolled dice.";
                    break;
                case EffectType.THREEDICES:
                    description += " use three dice and choose the";
                    description += TargetEffect == 'W' ? " best result." : " worst result.";
                    break;

                case EffectType.RAIN:
                    description = "Play a Rain weather card";
                    break;
                

                default:
                    description = "Effect Description.";
                    return description;
            }

            // especial
            return description;
        }

        override public string ToString()
        {
            return DescriptionEffect;
        }
    }
}
