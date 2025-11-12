using ProjetoPokemon.Data;
using ProjetoPokemon.Enums;
using ProjetoPokemon.Services;
using System.Diagnostics.Tracing;

namespace ProjetoPokemon.Entities
{
    internal class ProfilePokemon
    {
        public string NickPokemon { get; set; }
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
            NickPokemon = NickPokemon = string.IsNullOrWhiteSpace(name) ? pokemon.Name : name;
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
        public void PutNicknamePokemon()
        {
            string? newName = null;
            if (ConsoleMenu.ShowYesNo($"Do you want put a nickname in {Pokemon.Name}?"))
            {
                Console.Write("Write a nickname: ");
                newName = Console.ReadLine();
            }
            NickPokemon = string.IsNullOrWhiteSpace(newName) ? Pokemon.Name : newName;
        }
        
        public List<Move> GetMoveList(BattlerManager trainer) // filtra os moves
        {
            // Garante que há movimentos
            if (MovesPokemon == null || MovesPokemon.Count == 0)
                return new List<Move>();

            // Remove Sleep Talk (ID 218)
            List<Move> moveList = MovesPokemon
                .Where(p => p.MoveID != 218 && p.CanUse)
                .ToList();

            // Se o Pokémon estiver sob efeito de Taunt, remove movimentos com poder 0
            if (trainer.BuffsAndDebuffs.Contains(EffectType.TAUNT))
                moveList = moveList.Where(p => p.Power > 0).ToList();
            return moveList;
        }
        public void LevelUpPokemon()
        {
            if (LevelExp < 6) 
            { 
                LevelExp++; Console.WriteLine($"{NickPokemon} leveled up to {LevelPokemon()}!"); 
            }
            else Console.WriteLine($"O {NickPokemon} has already reached the maximum possible level. Level: {LevelPokemon()}");

            EvolutionPokemon();
        }
        public void EvolutionPokemon()
        {
            if (Pokemon.ExpToEvolve > 0 && LevelExp >= Pokemon.ExpToEvolve)
            {
                Console.WriteLine(NickPokemon+" can evolve...");
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
                if (ConsoleMenu.ShowYesNo($"Do you want evolve {NickPokemon}\n" + evolveOptions))
                {
                    if (evoPokemon.Count == 1)
                    {
                        Console.WriteLine($"{Pokemon.Name} evolved into {evoPokemon[0]}");
                        if (Pokemon.Name == NickPokemon) NickPokemon = evoPokemon[0].Name;
                        Pokemon = evoPokemon[0];
                        CopyMoves(Pokemon);
                    }
                    else
                    {
                        int selectedEvoIndex = ConsoleMenu.ShowMenu(ConsoleColor.Green,
                            evoPokemon.Select(evo => evo.ToString()).ToList(),
                            $"Select the evolution for {Pokemon.Name}:");
                        Console.WriteLine($"{Pokemon.Name} evolved into {evoPokemon[selectedEvoIndex].Name}");
                        if (Pokemon.Name == NickPokemon) NickPokemon = evoPokemon[selectedEvoIndex].Name;
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

        public string NicknamePokemon()
        {
            string nickname = NickPokemon;
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
            foreach(Move move in MovesPokemon) movesString += move.ToString() + "\n";

            return $"Summary:\n" +
                $"{Pokemon.NumberID}# " +
                $"{NicknamePokemon()} " +
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
            return NickPokemon + shinyTag + $" {status} Level: {LevelPokemon()} - Info: {Pokemon.ToString()}" + attachItem;
        }
    }
}

