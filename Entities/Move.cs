using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class Move
    {
        public int MoveID { get; set; }
        public TypePokemon Type { get; set; } // tipo do movimento
        public string Name { get; set; } // nome do movimento
        public int Power { get; set; } // poder do movimento
        public List<EffectMove> Effects = new List<EffectMove>(); // efeitos do movimento
        public string DiceSides { get; set; } // lados do dado do movimento
        public int EffectRoll { get; set; } // rolagem de efeito

        public Move(int moveID, TypePokemon type, string name, int power, string diceSides)
        {
            MoveID = moveID;
            Type = type;
            Name = name;
            Power = power;
            DiceSides = diceSides;
        }
        public Move(int moveID, TypePokemon type, string name, int power, List<EffectMove> effects, string diceSides, int efRoll)
        {
            MoveID = moveID;
            Type = type;
            Name = name;
            Power = power;
            Effects = effects;
            DiceSides = diceSides;
            EffectRoll = efRoll;
        }

        override public string ToString()
        {
            string EffectsDescription = "";
            foreach (var effect in Effects)
            {
                EffectsDescription += effect.ToString();
                if (effect != Effects[^1]) EffectsDescription += " | ";
            }
            if (Effects.Count > 0) return $"{Name} - {Type} - Power: {Power} | Roll:{EffectRoll} ({EffectsDescription})";
            else return $"{Name} - {Type} - Power: {Power}";
        }
    }
}
