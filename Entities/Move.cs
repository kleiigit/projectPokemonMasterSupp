using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class Move
    {
        public TypePokemon Type; // tipo do movimento
        public string Name; // nome do movimento
        public int Power; // poder do movimento
        public List<EffectMove> Effects; // efeitos do movimento
        public int DiceSides; // lados do dado do movimento
    }
}
