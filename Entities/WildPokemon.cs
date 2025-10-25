using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoPokemon.Entities
{
    internal class WildPokemon : ProfilePokemon
    {
        public WildPokemon(Pokemon pokemon, string name, int level) : base(pokemon, name, level)
        {
        }
        public Move RandomMove()
        {
            if (Pokemon.Moves == null || Pokemon.Moves.Count == 0)
                return null;
            Random rnd = new Random();
            return Pokemon.Moves[rnd.Next(Pokemon.Moves.Count)];
        }
    }
}
