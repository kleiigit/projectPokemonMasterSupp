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
        public Move SelectMove(BattleModifications trainerA, BattleModifications trainerB)
        {
            if (Pokemon.Moves == null || Pokemon.Moves.Count == 0)
                return null;
            int moveIndex = ConsoleMenu.ShowMenu(ConsoleColor.Yellow,
                Pokemon.Moves.Select(m => m.ToString()).ToList(), $"Choose move: {trainerA} Lv.{LevelPokemon()} ({Pokemon.StabString().ToUpper()}-type) VS " +
                $"{trainerB} Lv.{trainerB.SelectedPokemon.LevelPokemon()} ({trainerB.SelectedPokemon.Pokemon.StabString().ToUpper()}-type)");
            Move moveSelected = Pokemon.Moves[moveIndex];

            return moveSelected;
        }
        public void LevelUp()
        {
            if (Level < 6) 
            { 
                Level++; Console.WriteLine($"{Name} leveled up to {LevelPokemon()}!"); 
            }
            else Console.WriteLine($"O {Name} has already reached the maximum possible level. Level: {LevelPokemon()}");

            // inserir mecanica de evolução aqui no futuro
        }
        public override string ToString()
        {
            return Name + $" Level: {LevelPokemon()} - Info: {Pokemon.ToString()}";
        }
    }
}

