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
        public int DiceSides { get; set; } // lados do dado do movimento
    }
}
