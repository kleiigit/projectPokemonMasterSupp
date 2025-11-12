

using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Entities
{
    class Pokemon //classe perfil do pokémon
    {
        public int NumberID { get; } // ID do pokémon
        public string Name { get; } // nome do pokémon
        public TypePokemon Type { get; private set; } // tipo do pokémon
        public TypePokemon StabType { get; } // segundo tipo do pokémon
        public int Stage { get; } // estágio do pokémon
        public int[] EvolveID { get; } // lista de pokémons para evoluir
        public int LevelBase { get; } // nível básico do pokémon
        public int ExpToEvolve { get; } // experiência necessária para evoluir
        public int CathRate { get; } // taxa de captura do pokémon
        public int Generation { get; } // geração do pokémon
        public ColorToken Color { get; } // cor do token do pokémon
        public TokenBackGround Background { get; } // background do pokémon
        public List<Ability> Abilities = new List<Ability>(); // lista de habilidades do pokémon
        public List<Move> Moves = new List<Move>(); // lista de movimentos do pokémon

        public Pokemon(int numberID, string name, TypePokemon type, TypePokemon stabType, int stage, int[] toEvolvePokemon,
            int levelBase, int expToEvolve, int generation, ColorToken color,
             TokenBackGround background, List<Ability> abilities, List<Move> moves)
        {
            NumberID = numberID;
            Name = name;
            Type = type;
            StabType = stabType;
            Stage = stage;
            EvolveID = toEvolvePokemon;
            LevelBase = levelBase;
            ExpToEvolve = expToEvolve;
            Generation = generation;
            Color = color;
            CathRate = Catchrate();
            Background = background;
            Abilities = abilities;


            // Aplica o bônus de STAB automaticamente
            Moves = new List<Move>();
            foreach (var move in moves)
            {
                // Cria uma cópia do movimento com o bônus aplicado
                var moveCopy = new Move(move.MoveID, move.Type, move.Name, move.Power, move.Effects, move.DiceSides, move.EffectRoll);

                if (move.Type == Type || move.Type == StabType) moveCopy.StabMove();
                    

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
            LevelBase = other.LevelBase;
            ExpToEvolve = other.ExpToEvolve;
            Generation = other.Generation;
            Color = other.Color;
            Background = other.Background;

            // Copia das abilities (cada Ability deve ser clonável ou imutável)
            Abilities = new List<Ability>(other.Abilities);

            // Copia profunda dos movimentos
            Moves = new List<Move>();
            foreach (var move in other.Moves)
            {
                var moveCopy = new Move(move.MoveID, move.Type, move.Name, move.Power, move.Effects, move.DiceSides, move.EffectRoll);
                //if (move.Type == Type || move.Type == StabType) moveCopy.Power = Move.StabMove(move.Power);

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
        public void ChangeType(TypePokemon newType)
        {
            Type = newType;
        }
        public string GetStage()
        {
            switch(Stage)
            {
                case 1: return "Stage 1";
                case 2: return "Stage 2";
                default: return "Basic";
            }
        }
        public string GetStabType()
        {
            if (StabType != TypePokemon.None) return Type.ToString() + "/" + StabType.ToString();
            else return Type.ToString();
        }
        override public string ToString()
        {
            string pokemonDescription = $"{NumberID.ToString("D3")}# {Name}";
            pokemonDescription += " - ";
            for (int i = 0; i < Moves.Count; i++)
            {
                if (Moves[i].DiceSides != 6) pokemonDescription += $"(d{Moves[i].DiceSides}) ";
                pokemonDescription += $"{Moves[i].Name} {Moves[i].Power}";
                if (Moves[i].Effects.Count > 0) pokemonDescription += "*";
                if (i < Moves.Count - 1) pokemonDescription += ", ";
            }

            pokemonDescription += $"  [Level: {LevelBase}, {Type.ToString().ToUpper()}-Type, Color: {Color}]";
            return pokemonDescription;
        }
    }


}
