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
        public bool CanAttack { get; set; } = true;

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

        public void NickName()
        {
            string? newName = null;
            if (ConsoleMenu.ShowYesNo($"Do you want put a nickname in {Pokemon.Name}?"))
            {
                Console.Write("Write a nickname: ");
                newName = Console.ReadLine();
            }
            Name = string.IsNullOrWhiteSpace(newName) ? Pokemon.Name : newName;
        }

        public void LevelUp()
        {
            if (Level < 6) 
            { 
                Level++; Console.WriteLine($"{Name} subiu para o nível {LevelPokemon()}!"); 
            }
            else Console.WriteLine($"O {Name} já atingiu o nível máximo possível. Nível: {LevelPokemon()}");

            // inserir mecanica de evolução aqui no futuro
        }
        public override string ToString()
        {
            return Name + $" {Pokemon.ToString()} Level: {Level}";
        }
    }
}

