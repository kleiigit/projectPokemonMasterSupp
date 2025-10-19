using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    class Pokemon //classe perfil do pokémon
    {
        public int NumberID { get; } // ID do pokémon
        public string Name { get; } // nome do pokémon
        public TypePokemon Type { get; } // tipo do pokémon
        public TypePokemon StabType { get; } // segundo tipo do pokémon
        public int Stage { get; } // estágio do pokémon
        public int EvolveID { get; } // lista de pokémons para evoluir
        public string? Form { get; } // forma do pokémon
        //
        public int LevelBase { get; } // nível básico do pokémon
        public int ExpToEvolve { get; } // experiência necessária para evoluir
        public int CathRate { get; } // taxa de captura do pokémon
        public int Generation { get; } // geração do pokémon
        public ColorToken Color { get; } // cor do token do pokémon
        public TokenBackGround Background { get; } // background do pokémon
        public List<Ability> Abilities = new List<Ability>(); // lista de habilidades do pokémon
        //
        public bool Shiny { get; } // se o pokémon é shiny ou não

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


            // Aplica o bônus de STAB automaticamente
            Moves = new List<Move>();
            foreach (var move in moves)
            {
                // Cria uma cópia do movimento com o bônus aplicado
                var moveCopy = new Move(move.MoveID, move.Type, move.Name, move.Power, move.Effects, move.DiceSides, move.EffectRoll);

                if (move.Type == Type || move.Type == StabType) moveCopy.Power = Move.StabMove(move.Power);
                    

                Moves.Add(moveCopy);
            }
        }
        public Pokemon(Pokemon other)
        {
            NumberID = other.NumberID;
            Name = other.Name;
            Type = other.Type;
            StabType = other.StabType;
            Stage = other.Stage;
            EvolveID = other.EvolveID;
            Form = other.Form;
            LevelBase = other.LevelBase;
            ExpToEvolve = other.ExpToEvolve;
            Generation = other.Generation;
            Color = other.Color;
            Background = other.Background;
            Shiny = other.Shiny;

            // Copia das abilities (cada Ability deve ser clonável ou imutável)
            Abilities = new List<Ability>(other.Abilities);

            // Copia profunda dos movimentos
            Moves = new List<Move>();
            foreach (var move in other.Moves)
            {
                var moveCopy = new Move(move.MoveID, move.Type, move.Name, move.Power, move.Effects, move.DiceSides, move.EffectRoll);
                if (move.Type == Type || move.Type == StabType)
                    moveCopy.Power = Move.StabMove(move.Power);

                Moves.Add(moveCopy);
            }

            CathRate = Catchrate(); // recalcula taxa de captura se necessário
        }

        private int Catchrate()
        {
            if (LevelBase >= 7)
                return LevelBase + 1;
            else
                return LevelBase + 2;
        }

        public Move RandomMove()
        {
            if (Moves == null || Moves.Count == 0)
                return null;
            Random rnd = new Random();
            return Moves[rnd.Next(Moves.Count)];
        }

        public Move SelectMove()
        {
            if (Moves == null || Moves.Count == 0)
                return null;
            int moveIndex = ConsoleMenu.ShowMenu(Moves.Select(m => m.ToString()).ToList(), "Escolha o ataque de " + ToString());
            Move moveSelected = Moves[moveIndex];

            return moveSelected;
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
