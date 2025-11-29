
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetoPokemon.Data;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;

namespace ProjetoPokemon.Entities
{
    internal class ProfilePokemon
    {
        public string Name { get; set; }
        public Pokemon Pokemon { get; set; }
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
            Name = Name = string.IsNullOrWhiteSpace(name) ? pokemon.Name : name;
            LevelExp = level;
            AttachCard = null;
            Conditions = StatusConditions.NORMAL;
        }
        public ProfilePokemon(Pokemon pokemon) // wild pokemon
        {
            Pokemon = pokemon;
            Name = pokemon.Name;
            LevelExp = 0;
            AttachCard = null;
            Conditions = StatusConditions.NORMAL;
        }
        public ProfilePokemon Clone()
        {
            // Cria a nova instância usando o construtor básico
            ProfilePokemon clone = new ProfilePokemon(this.Pokemon, this.Name, this.LevelExp);

            // Copia propriedades simples
            clone.Conditions = this.Conditions;
            clone.ConditionCount = this.ConditionCount;
            clone.CanAttack = this.CanAttack;
            clone.Forms = this.Forms;

            // Copia Shiny via método, já que a propriedade é privada set
            if (this.Shiny)
                clone.SetShiny(true);

            // Copia AttachCard (deep copy se existir)
            if (this.AttachCard != null)
                clone.AttachCard = this.AttachCard.Copy(); // assumindo que exista Copy()
            return clone;
        }
        // Métodos de Pokemon Profile
        public int LevelPokemon()
        {
            return LevelExp + Pokemon.LevelBase;
        }
        public void PutNicknamePokemon()
        {
            string? newName = null;
            if (ConsoleMenu.ShowYesNo($"Do you want put a nickname in {Pokemon.Name}?"))
            {
                Console.Write("Write a nickname: ");
                newName = Console.ReadLine();
            }
            Name = string.IsNullOrWhiteSpace(newName) ? Pokemon.Name : newName;
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
                        Pokemon.Moves = evoPokemon[0].Moves;
                    }
                    else
                    {
                        int selectedEvoIndex = ConsoleMenu.ShowMenu(ConsoleColor.Green,
                            evoPokemon.Select(evo => evo.ToString()).ToList(),
                            $"Select the evolution for {Pokemon.Name}:");
                        Console.WriteLine($"{Pokemon.Name} evolved into {evoPokemon[selectedEvoIndex].Name}");
                        if (Pokemon.Name == Name) Name = evoPokemon[selectedEvoIndex].Name;
                        Pokemon = evoPokemon[selectedEvoIndex];
                        Pokemon.Moves = evoPokemon[0].Moves;
                    }
                    LevelExp = LevelExp - Pokemon.ExpToEvolve;
                }
            }
        }
        public void SetShiny(bool chance)
        {
            if (chance) Shiny = true;
        }
        public string GetName()
        {
            string nickname = Name;
            if (nickname != Pokemon.Name)
            {
                nickname += $" ({Pokemon.Name})";
            }
            return nickname;
        }
        public string SummaryProfile()
        {
            string conditionsText = Conditions.ToString();
            string pokemonEvo = string.Empty;
            string movesString = string.Empty;
            if(ConditionCount > 0)
            {
                conditionsText += $" ({ConditionCount})";
            }
            if (Pokemon.ExpToEvolve > 0)
            {
                string namesEvo = string.Empty;
                pokemonEvo = "- To Evo: " + (Pokemon.LevelBase + Pokemon.ExpToEvolve) + $" (+{Pokemon.ExpToEvolve})";
                foreach (int numberId in Pokemon.EvolveID)
                {
                    namesEvo += DataLists.GetPokemonID(numberId).Name + " ";
                }
                pokemonEvo += $" ( {namesEvo})";
            }
            foreach (Move move in Pokemon.Moves) movesString += move.ToString() + "\n";

            return $"Summary:\n" +
                $"{Pokemon.NumberID}# " +
                $"{GetName()} " +
                $"{Pokemon.GetStabType().ToUpper()}-TYPE " +
                $"{Pokemon.GetStage()} " +
                $"Gen {Pokemon.Generation}\n" +
                $"Status: {conditionsText}\n" +
                $"Level: {LevelPokemon()} {pokemonEvo}\n" +
                $"Attached Item: {AttachCard?.Name}\n" +
                $"[Token: {Pokemon.Color.ToString()}, Level Base: {Pokemon.LevelBase}, Catch: +{Pokemon.CathRate}]\n" +
                $"Moves: \n{movesString}";
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

