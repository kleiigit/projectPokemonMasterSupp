
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Entities
{
    internal class Move
    {
        public int MoveID { get; }
        public TypePokemon Type { get; } // tipo do movimento
        public string Name { get; } // nome do movimento
        public int Power { get; private set; } // poder do movimento
        public List<EffectManager> Effects = new List<EffectManager>(); // efeitos do movimento
        public int DiceSides { get; private set; } // lados do dado do movimento
        public int EffectRoll { get; } // rolagem de efeito
        public bool CanUse { get; private set; } = true;
        public double Rate { get; private set; }

        public Move(int moveID, TypePokemon type, string name, int power, int diceSides)
        {
            MoveID = moveID;
            Type = type;
            Name = name;
            Power = power;
            DiceSides = diceSides;
        }
        public Move(int moveID, TypePokemon type, string name, int power, List<EffectManager> effects, int diceSides, int efRoll)
        {
            MoveID = moveID;
            Type = type;
            Name = name;
            Power = power;
            Effects = effects;
            DiceSides = DiceSidesMove(diceSides);
            EffectRoll = efRoll;
        }
        
        public static int DiceSidesMove(int diceSides) { if (diceSides == 0) diceSides = 6; return diceSides;}
        public void StabMove() { if (Power != 0 && Power < 4) Power += 1;}
        public void RateWin(double rate)
        {
            Rate = rate;
        }
        public void RechargeMove() { CanUse = false; }
        public void HalfLevelPower(ref int powerM) { Power = Math.Max(1, (int)Math.Floor(powerM / 2.0)); }
        public void BoostPower()
        {
            Power++;
        }
        public void ChangePower(int newPower)
        {
            Power = newPower;
        }
        public void ChangeSide(int newSide)
        {
            DiceSides = newSide;
        }
        public Move Copy()
        {
            Move copiedMove = new Move(
                MoveID,
                Type,
                Name,
                Power,
                Effects,
                DiceSides,
                EffectRoll
            );

            // Copia propriedades adicionais
            copiedMove.RateWin(Rate);
            if (!CanUse) copiedMove.RechargeMove();

            return copiedMove;
        }
        override public string ToString()
        {
            string EffectsDescription = "";
            foreach (var effect in Effects)
            {
                EffectsDescription += effect.ToString();
                if (effect != Effects[^1]) EffectsDescription += " | ";
            }
            string moveStr = $"({Rate.ToString("F0")}%) {Name} - {Type} - Power: {Power}";
            if (DiceSides != 6) moveStr += $" (D{DiceSides})";
            if (Effects.Count > 0)
            {
                if (EffectRoll > 0) return moveStr + $" | Effect Roll:{EffectRoll} ({EffectsDescription})";
                else return moveStr + $" | Effect ({EffectsDescription})";
            }
            else return moveStr;
        }
       
    }
}
