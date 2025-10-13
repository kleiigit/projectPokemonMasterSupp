using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("pokemon.csv");
            List<Pokemon> pokemons = new List<Pokemon>();
            foreach (var line in sr.ReadToEnd().Split("\n").Skip(1))
            {
                var attributes = line.Split(",");
                if (attributes.Length < 15) continue;

                int numberID = int.Parse(attributes[0]);
                string name = attributes[1];
                TypePokemon type = (TypePokemon)Enum.Parse(typeof(TypePokemon), attributes[2]);
                TypePokemon stabType = string.IsNullOrEmpty(attributes[3]) ? TypePokemon.None : (TypePokemon)Enum.Parse(typeof(TypePokemon), attributes[3]);
                int stage = int.Parse(attributes[4]);
                int toEvolveID = string.IsNullOrEmpty(attributes[5]) ? 0 : int.Parse(attributes[5]);
                string? form = string.IsNullOrEmpty(attributes[15]) ? null : attributes[15];
                int levelBase = int.Parse(attributes[6]);
                int expToEvolve = int.Parse(attributes[7]);
                int generation = int.Parse(attributes[9]);
                ColorToken color = (ColorToken)Enum.Parse(typeof(ColorToken), attributes[10]);
                Background background = (Background)Enum.Parse(typeof(Background), attributes[11]);
                List<Ability> abilities = new List<Ability>(); // Simplified for this example
                bool shiny = bool.Parse(attributes[16]);
                List<Move> moves = new List<Move>(); // Simplified for this example

                Pokemon pokemon = new Pokemon(numberID, name, type, stabType, stage, toEvolveID, form,
                    levelBase, expToEvolve, generation,
                    color, background, abilities, shiny, moves);
                pokemons.Add(pokemon);
            }
        }
    }
}