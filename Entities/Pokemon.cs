using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    class Pokemon //classe perfil do pokémon
    {
        public int NumberID; // ID do pokémon
        public string Name; // nome do pokémon
        public TypePokemon Type; // tipo do pokémon
        public TypePokemon StabType; // segundo tipo do pokémon
        public int Stage; // estágio do pokémon
        public List<Pokemon>? ToEvolvePokemon; // lista de pokémons para evoluir
        public string? Form; // forma do pokémon

        public int LevelBase; // nível básico do pokémon
        public int ExpToEvolve; // experiência necessária para evoluir
        public int CathRate; // taxa de captura do pokémon
        public int Generation; // geração do pokémon
        public ColorToken Color; // cor do token do pokémon
        public Background Background; // background do pokémon
        public List<Ability> abilities = new List<Ability>(); // lista de habilidades do pokémon

        public bool shiny; // se o pokémon é shiny ou não

        public List<Move> moves = new List<Move>(); // lista de movimentos do pokémon

        public Pokemon(int numberID, string name, TypePokemon type, TypePokemon stabType, int stage, List<Pokemon>? toEvolvePokemon, string? form, 
            int levelBase, int expToEvolve, int cathRate, int generation, 
            ColorToken color, Background background, List<Ability> abilities, bool shiny, List<Move> moves)
        {
            NumberID = numberID;
            Name = name;
            Type = type;
            StabType = stabType;
            Stage = stage;
            ToEvolvePokemon = toEvolvePokemon;
            Form = form;
            LevelBase = levelBase;
            ExpToEvolve = expToEvolve;
            CathRate = cathRate;
            Generation = generation;
            Color = color;
            Background = background;
            this.abilities = abilities;
            this.shiny = shiny;
            this.moves = moves;
        }
    }


}
