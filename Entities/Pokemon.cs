using ProjetoPokemon.Entities.Enums;
using System.Linq;

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

            Console.WriteLine("\nSelect a move of " + Name);
            for (int i = 0; i < Moves.Count; i++)
            {
                Console.WriteLine($"{i}: {Moves[i]}");
            }

            // Solicita escolha do usuário
            Console.Write("Digite o índice do item desejado: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index < Moves.Count)
            {
                Move selected = Moves[index];
                Console.WriteLine($"Você escolheu: {selected.Name}");
                return selected;
            }

            Console.WriteLine("Índice inválido. Operação cancelada.");
            return default!;
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
