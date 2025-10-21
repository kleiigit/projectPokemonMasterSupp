using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class ItemCard
    {
        public int Id { get; }
        public string Name { get;}
        public TypeItemCard Type { get;}
        public List<EffectCard> Effects = new List<EffectCard>();
        public string Description { get;}

        public ItemCard(int id, string name, TypeItemCard type, List<EffectCard> effects, string description)
        {
            Id = id;
            Name = name;
            Type = type;
            Effects = effects;
            Description = description;
        }

        public string BattleCard()
        {
            string effectCard = "";
            if (Type == TypeItemCard.Battle)
            {
                foreach (EffectCard effect in Effects)
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




        public override string ToString()
        {
            return Name + " - " + Type.ToString() + $" Card ({Description})";
        }
    }
}
