using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Entities.Enums;
using ProjetoPokemon.Entities.Profiles;

namespace ProjetoPokemon.Entities
{
    enum RarityCard
    {
        Common,
        Uncommon,
        Rare,
        Epic,
    }
    internal class ItemCard
    {
        public int Id { get; }
        public string Name { get;}
        public RarityCard Rarity { get; } = RarityCard.Common;
        public TypeItemCard Type { get;}
        public List<EffectManager> Effects = new List<EffectManager>();
        public string Description { get;}

        public ItemCard(int id, string name, RarityCard rarity, TypeItemCard type, List<EffectManager> effects, string description)
        {
            Id = id;
            Name = name;
            Rarity = rarity;
            Type = type;
            Effects = effects;
            Description = description;
        }

        public string BattleCard()
        {
            string effectCard = "";
            if (Type == TypeItemCard.Battle)
            {
                foreach (EffectManager effect in Effects)
                {
                    switch(effect.EffectType)
                    {
                        case EffectType.ADDROLL:
                            effectCard += $"roll.{effect.BonusEffect};";
                            break;
                        default:
                            effectCard += "";
                            break;
                    }
                }
            }
            if (effectCard == "")
                return "";
            else
                return effectCard;
        }

        public string AttachCard()
        {
            string effectCard = "";
            if (Type == TypeItemCard.Attach)
            {
                foreach (EffectManager effect in Effects)
                {
                    string[] effectCond;
                    if (effect.EffectCond != null) effectCond = effect.EffectCond.Split(',');
                    switch (effect.EffectType)
                    {
                        // weather
                        case EffectType.SNOW:
                            effectCard += "snowcard;";
                            break;
                        case EffectType.RAIN:
                            effectCard += "raincard;";
                            break;
                        case EffectType.SAND:
                            effectCard += "sandcard;";
                            break;
                        case EffectType.SUNNYDAY:
                            effectCard += "suncard;";
                            break;
                        // trigger
                        case EffectType.ROLL:
                            effectCard += "rollTime;";
                            break;
                        case EffectType.STATUS:
                            effectCard += "statusTime;";
                            break;
                        // mod
                        case EffectType.ADDPOWER:
                            effectCard += $"addPower.{effect.BonusEffect};";
                            break;
                        case EffectType.IMMUNE:
                            if(effect.EffectCond != null)
                            {
                                if(effect.EffectCond == EffectType.STATUS.ToString())
                                {

                                }
                                else
                                {
                                    if (!Enum.TryParse(effect.EffectCond, out EffectType typeImmune))
                                    {
                                        effectCard += $"imune.{typeImmune}";
                                    }  
                                }
                            }
                            break;
                        
                            
                    }
                }   
            }
            if (effectCard == "")
                return "";
                else
                return effectCard;
        }




        public override string ToString()
        {
            return Name + $" - {Rarity.ToString()} {Type.ToString()} Card ({Description})";
        }
    }
}
