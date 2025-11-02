using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities
{
    internal class ProfilePokemon
    {
        public string Name { get; set; }
        public Pokemon Pokemon { get; set; }
        public List<Move> MovesPokemon { get; private set; } = new List<Move>();
        public int LevelExp { get; set; }
        public ItemCard? AttachCard { get; set; }
        public StatusConditions Conditions { get; set; }
        public int ConditionCount { get; set; }
        public bool CanAttack { get; set; } = true;
        public string? Forms { get; set; }
        public bool Shiny { get; private set; } = false;

        // Construtor principal
        public ProfilePokemon(Pokemon pokemon, string name, int level)
        {
            Pokemon = pokemon;
            CopyMoves(pokemon);
            Name = Name = string.IsNullOrWhiteSpace(name) ? pokemon.Name : name;
            LevelExp = level;
            AttachCard = null;
            Conditions = StatusConditions.NORMAL;
        }
        public void CopyMoves(Pokemon pokemon)
        {
            MovesPokemon.Clear();
            foreach( Move move in pokemon.Moves)
            {
                MovesPokemon.Add(move.Copy());
            }
        }
        // Métodos de Pokemon Profile
        public int LevelPokemon()
        {
            return LevelExp + Pokemon.LevelBase;
        }
        public void NickNamePokemon()
        {
            string? newName = null;
            if (ConsoleMenu.ShowYesNo($"Do you want put a nickname in {Pokemon.Name}?"))
            {
                Console.Write("Write a nickname: ");
                newName = Console.ReadLine();
            }
            Name = string.IsNullOrWhiteSpace(newName) ? Pokemon.Name : newName;
        }
        public Move RandomMovePokemon()
        {
            if (MovesPokemon == null || MovesPokemon.Count == 0)
                return new Move(0, TypePokemon.None, "No Move", 0, 6);
            Random rnd = new Random();
            return MovesPokemon[rnd.Next(MovesPokemon.Count)];
        }
        public Move SelectMovePokemon(BattlerManager trainerA, BattlerManager trainerB)
        {
            if (MovesPokemon == null || MovesPokemon.Count == 0) return new Move(0, TypePokemon.None, "No Move", 0, 6);
            List<Move> moveList = MovesPokemon.Where(p => p.CanUse == true).ToList();
            
            foreach (Move move in MovesPokemon)
            { 
                move.RateWin(VictoryChanceService.ChanceSimulator(move, trainerA, trainerB));
            }
            
            int moveIndex = ConsoleMenu.ShowMenu(ConsoleColor.Yellow,
                moveList.Select(m => m.ToString()).ToList(), $"Choose move: {trainerA} Lv.{LevelPokemon()} ({Pokemon.Type}-type) \nVS " +
                $"{trainerB} Lv.{trainerB.SelectedPokemon.LevelPokemon()} ({trainerB.SelectedPokemon.Pokemon.Type}-type)");
            Move moveSelected = moveList[moveIndex];

            return moveSelected;
        }
        public void LevelUpPokemon()
        {
            if (LevelExp < 6) 
            { 
                LevelExp++; Console.WriteLine($"{Name} leveled up to {LevelPokemon()}!"); 
            }
            else Console.WriteLine($"O {Name} has already reached the maximum possible level. Level: {LevelPokemon()}");

            EvolutionPokemon();
        }
        public void EvolutionPokemon()
        {
            if (Pokemon.ExpToEvolve > 0 && LevelExp >= Pokemon.ExpToEvolve)
            {
                Console.WriteLine(Name+" can evolve...");
                List<Pokemon> evoPokemon = new List<Pokemon>();
                string evolveOptions = "";
                foreach (int evolveID in Pokemon.EvolveID)
                {
                    Pokemon evolvedForm = Data.DataLists.GetPokemonID(evolveID);
                    if (evolvedForm != null)
                    {
                        evoPokemon.Add(evolvedForm);
                        evolveOptions += $"{evolvedForm}\n";
                    }
                    else { Console.WriteLine($"No evolution found for {Pokemon.Name} with ID {evolveID}."); }
                }
                Console.ReadLine();
                if (ConsoleMenu.ShowYesNo($"Do you want evolve {Name}\n" + evolveOptions))
                {
                    if (evoPokemon.Count == 1)
                    {
                        Console.WriteLine($"{Pokemon.Name} evolved into {evoPokemon[0]}");
                        if (Pokemon.Name == Name) Name = evoPokemon[0].Name;
                        Pokemon = evoPokemon[0];
                        CopyMoves(Pokemon);
                    }
                    else
                    {
                        int selectedEvoIndex = ConsoleMenu.ShowMenu(ConsoleColor.Green,
                            evoPokemon.Select(evo => evo.ToString()).ToList(),
                            $"Select the evolution for {Pokemon.Name}:");
                        Console.WriteLine($"{Pokemon.Name} evolved into {evoPokemon[selectedEvoIndex].Name}");
                        if (Pokemon.Name == Name) Name = evoPokemon[selectedEvoIndex].Name;
                        Pokemon = evoPokemon[selectedEvoIndex];
                        CopyMoves(Pokemon);
                    }
                    LevelExp = LevelExp - Pokemon.ExpToEvolve;
                }
            }
        }
        public void SetShiny(bool chance)
        {
            if (chance) Shiny = true;
        }

        public override string ToString()
        {
            string status = "";
            string attachItem = "";
            string shinyTag = "";
            if (Shiny) shinyTag = "*";
            if (Conditions != StatusConditions.NORMAL) status = $"[{Conditions.ToString()}]";
            if (AttachCard != null) attachItem = $" - Attached Item: {AttachCard.Name}";
            return Name + shinyTag + $" {status} Level: {LevelPokemon()} - Info: {Pokemon.ToString()}" + attachItem;
        }
    }
}

