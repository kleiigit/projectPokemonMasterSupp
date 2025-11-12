

using DocumentFormat.OpenXml.Drawing;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Entities
{
    internal class EffectManager
    {
        // TargetEffect Chat: (Battle: W - user, B - opponent) A - Attach item, F - Berry attach, C - Catch item, S - Selection in Team
        public char TargetEffect { get; } 
        public EffectType EffectType { get; }
        public string? MoveDescription { get; private set; }
        public int BonusEffect { get; }
        public string? EffectCond { get; }

        public EffectManager(EffectType effectType, int n)
        {
            EffectType = effectType;
            MoveDescription = EffectDescription(n);
        }

        public EffectManager(char targetEffect, EffectType effectType, int n)
        {
            TargetEffect = targetEffect;
            EffectType = effectType;
            MoveDescription = EffectDescription(n);
        }
        public EffectManager(char targetEffect, EffectType effectType, int bonus, string effectCond, int n)
        {
            TargetEffect = targetEffect;
            EffectType = effectType;
            BonusEffect = bonus;
            EffectCond = effectCond;
            MoveDescription = EffectDescription(n);
        }

        private string EffectDescription(int n)
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
                case EffectType.BOOST:
                    description = "This move boosts its power after use.";
                    break;
                case EffectType.NERF:
                    description += " has a disadvantage on its next rolls until the end of the battle.";
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
                case EffectType.SNOW:
                    description = "Play a Snow weather card";
                    break;
                case EffectType.SUNNYDAY:
                    description = "Play a Sunny Day weather card";
                    break;
                case EffectType.SAND:
                    description = "Play a Sandstorm weather card";
                    break;
                case EffectType.SAFEGUARD:
                    description = "Play a Safeguard card";
                    break;


                case EffectType.ESPECIAL:
                    switch (n)
                    {
                        case 54: //metronome
                            description = "A random move will occur!";
                            break;
                        case 22: // venoshock
                            description = "+2 attack bonus if the opposing Pokémon has a status condition.";
                            break;
                        case 88: // smack down
                            description = "the opposing Pokémon cannot resist Ground-type attacks.";
                            break;
                        case 98: // tri attack
                            description = "Inflicts a random effect on the opposing Pokémon.";
                            break;
                        case 36: // mirror move
                            description = "Copy the opponent’s move.";
                            break;
                        case 157: // dream eater
                            description = "2x attack power if the opponent is asleep.";
                            break;
                        case 173: // hidden power
                            description = "This move has a random type.";
                            break;
                        case 176: // torment
                            description += " cannot use the same move in consecutive turns.";
                            break;
                        case 177: // taunt
                            description += " cannot use move with 0 power.";
                                break;
                        case 179: // protect
                            description += " move has 0 power, and its effects cannot affect your Pokémon.";
                                break;
                        case 180: // roost
                            description += " may lose its Flying-type";
                                break;
                        case 168: // brick break.
                            description += " cannot be affected by the opponent’s move disadvantages.";
                                break;
                        case 186: //facade
                            description = "If the opponent is affected by a status condition, it gains +2.";
                                break;
                        case 189: // thief
                            description = "This move can steal the opponent’s attached card.";
                                break;
                        case 196: // false swipe
                            description = "If this move defeats a wild Pokémon, it grants a +1 capture bonus.";
                                break;
                        case 199: // incinerate
                            description = "This move disable opponent's attach card.";
                                break;
                        case 202: // acrobatics
                            description = "This move has a bonus if no item is attached.";
                               break;
                        case 203: // embargo
                            description = "Disable all item card opponent.";
                            break;

                            // 206 retaliate
                        case 218: // sleep talk
                            description += " can attack while asleep. Cannot be used while awake.";
                            break;

                        // retaliation
                        case 223: // nature power
                            description = "This move becomes a new move.";
                            break;
                        default:
                            description = "No Effect Description.";
                            return description;
                    }
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
            if (MoveDescription == null) MoveDescription = "No Description";
            return MoveDescription;
        }
    }
}
