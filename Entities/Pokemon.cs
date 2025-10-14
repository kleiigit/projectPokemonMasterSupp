using DocumentFormat.OpenXml.Drawing.Diagrams;
using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    class Pokemon //classe perfil do pokémon
    {
        public int NumberID { get; set; } // ID do pokémon
        public string Name { get; set; } // nome do pokémon
        public TypePokemon Type { get; set; } // tipo do pokémon
        public TypePokemon StabType { get; set; } // segundo tipo do pokémon
        public int Stage { get; set; } // estágio do pokémon
        public int EvolveID { get; set; } // lista de pokémons para evoluir
        public string? Form { get; set; } // forma do pokémon
        //
        public int LevelBase { get; set; } // nível básico do pokémon
        public int ExpToEvolve { get; set; } // experiência necessária para evoluir
        public int CathRate { get; set; } // taxa de captura do pokémon
        public int Generation { get; set; } // geração do pokémon
        public ColorToken Color { get; set; } // cor do token do pokémon
        public TokenBackGround Background { get; set; } // background do pokémon
        public List<Ability> Abilities = new List<Ability>(); // lista de habilidades do pokémon
        //
        public bool Shiny { get; set; } // se o pokémon é shiny ou não

        public List<Move> Moves = new List<Move>(); // lista de movimentos do pokémon

        public Pokemon(int numberID, string name, TypePokemon type, TypePokemon stabType, int stage, int toEvolvePokemon, string? form,
            int levelBase, int expToEvolve, int generation, ColorToken color,
             TokenBackGround background, List<Ability> abilities, bool shiny, List<Move> moves)
        {
            NumberID = numberID;
            Name = name;
            Type = type;
            StabType = stabType;
            Stage = stage;
            EvolveID = toEvolvePokemon;
            Form = form;
            LevelBase = levelBase;
            ExpToEvolve = expToEvolve;
            Generation = generation;
            Color = color;
            CathRate = Catchrate();
            Background = background;
            Abilities = abilities;
            Shiny = shiny;
            Moves = moves;
        }

        private int Catchrate()
        {
            if (LevelBase >= 7)
                return LevelBase + 1;
            else
                return LevelBase + 2;
        }

        override public string ToString()
        {
            string dualType = Type.ToString();
            string pokemonDescription = $"{NumberID.ToString("D3")}# {Name}, {dualType} [Level: {LevelBase}, Color:{Color}] ";
            for (int i = 0; i < Moves.Count; i++)
            {
                pokemonDescription += $"{Moves[i].Name} {Moves[i].Power}";
                if (i < Moves.Count - 1)
                    pokemonDescription += ", ";
            }
            if (StabType != TypePokemon.None)
            {
                dualType += $"/{StabType}";
            }


            return pokemonDescription;
        }
    }


}
