using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class Move
    {
        public int MoveID { get; }
        public TypePokemon Type { get; } // tipo do movimento
        public string Name { get; } // nome do movimento
        public int Power { get; set; } // poder do movimento
        public List<EffectMove> Effects = new List<EffectMove>(); // efeitos do movimento
        public int DiceSides { get; } // lados do dado do movimento
        public int EffectRoll { get; } // rolagem de efeito

        public Move(int moveID, TypePokemon type, string name, int power, int diceSides)
        {
            MoveID = moveID;
            Type = type;
            Name = name;
            Power = power;
            DiceSides = diceSides;
        }
        public Move(int moveID, TypePokemon type, string name, int power, List<EffectMove> effects, int diceSides, int efRoll)
        {
            MoveID = moveID;
            Type = type;
            Name = name;
            Power = power;
            Effects = effects;
            DiceSides = DiceSidesMove(diceSides);
            EffectRoll = efRoll;
        }
        public static int DiceSidesMove(int diceSides)
        {
            if (diceSides == 0)
                diceSides = 6;
            return diceSides;
        }

        public static int StabMove(int powerM)
        {
            if (powerM != 0) powerM += 1;
            return powerM;
        }
        override public string ToString()
        {
            string EffectsDescription = "";
            foreach (var effect in Effects)
            {
                EffectsDescription += effect.ToString();
                if (effect != Effects[^1]) EffectsDescription += " | ";
            }
            string moveStr = $"{Name} - {Type} - Power: {Power}";
            if (DiceSides != 6) moveStr += $" (D{DiceSides})";
            if (Effects.Count > 0) return moveStr + $" | Roll:{ EffectRoll} ({ EffectsDescription})";
            else return moveStr;
        }
       
    }
}
