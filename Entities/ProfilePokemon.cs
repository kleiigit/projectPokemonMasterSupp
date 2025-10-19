using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class ProfilePokemon
    {
        public string Name { get; set; }
        public Pokemon Pokemon { get; set; }
        public int Level { get; set; }
        public ItemCard? AttachCard { get; set; }
        public StatusConditions Conditions { get; set; }
        public int ConditionCount { get; set; }

        // Construtor principal
        public ProfilePokemon(Pokemon pokemon, string name, int level)
        {
            Pokemon = pokemon;
            Name = Name = string.IsNullOrWhiteSpace(name) ? pokemon.Name : name;
            Level = level;
            AttachCard = null;
            Conditions = StatusConditions.NORMAL;
        }

        public int LevelPokemon()
        {
            return Level + Pokemon.LevelBase;
        }
        public override string ToString()
        {
            return Name + $" {Pokemon.ToString()} Level: {Level}";
        }
    }
}

