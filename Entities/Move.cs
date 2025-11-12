
using ProjetoPokemon.Data;
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Entities
{
    internal class Move
    {
        public int MoveID { get; }
        public TypePokemon Type { get; private set; } // tipo do movimento
        public string Name { get; } // nome do movimento
        public int Power { get; private set; } // poder do movimento
        public List<EffectManager> Effects = new List<EffectManager>(); // efeitos do movimento
        public int DiceSides { get; private set; } // lados do dado do movimento
        public int EffectRoll { get; } // rolagem de efeito
        public bool CanUse { get; set; } = true;
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
        public void RateWin(List<double[]> rate)
        {
            Rate = rate.Min(r => r[0]);
        }
        public void RechargeMove() { CanUse = false; }
        public void HalfLevelPower(ref int powerM) { Power = Math.Max(1, (int)Math.Floor(powerM / 2.0)); }
        
        public void ChangePower(int n)
        {
            Power = n;
        }
        public void BoostPower()
        {
            Power++;
        }
        public void ChangeSide(int newSide)
        {
            DiceSides = newSide;
        }
        public void ChangeType(TypePokemon newType)
        {
            Type = newType;
        }

        public Move Copy()
        {
            // Cópia profunda e segura dos efeitos
            List<EffectManager> clonedEffects = new List<EffectManager>();
            foreach (var effect in Effects)
            {
                var clonedEffect = new EffectManager(
                    effect.TargetEffect,
                    effect.EffectType,
                    effect.BonusEffect,
                    effect.EffectCond ?? string.Empty,
                    MoveID
                );
                clonedEffects.Add(clonedEffect);
            }

            Move copiedMove = new Move(
                MoveID,
                Type,
                Name,
                Power,
                clonedEffects,
                DiceSides,
                EffectRoll
            );

            if (!CanUse)
                copiedMove.RechargeMove();

            return copiedMove;
        }
        public string SummaryMove()
        {
            string pokemonOwnes = string.Empty;
            List<Pokemon> moveOwner = DataLists.AllPokemons.Where(p => p.Moves.Any(m => m.Name == Name)).ToList();
            if (moveOwner.Count > 0)
            {
                foreach (var pokemon in moveOwner)
                {
                    pokemonOwnes += pokemon.Name.ToUpper() + ", ";
                }
            }
            else
            {
                pokemonOwnes = "None";
            }
            return ToString() + "\n\nPokemon: " + pokemonOwnes;
        }
        public string MoveMenu()
        {
            return $"({Rate:F0}%) " + ToString();
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
            if (Effects.Count > 0)
            {
                if (EffectRoll > 0) return moveStr + $" | Effect Roll:{EffectRoll} ({EffectsDescription})";
                else return moveStr + $" | Effect ({EffectsDescription})";
            }
            else return moveStr;
        }
       
    }
}
