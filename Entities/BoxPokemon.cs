using ProjetoPokemon.Entities.Enums;
using System.Text.RegularExpressions;
using System.Linq;
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon.Entities
{
    internal class BoxPokemon
    {
        public string Nickname { get;}
        public List<ProfilePokemon> ListBox = new List<ProfilePokemon>();
        public Dictionary<ItemCard,int> ListCards = new Dictionary<ItemCard,int>();

        public BoxPokemon(string nickname)
        {
            Nickname = nickname;
        }

        public BoxPokemon(string nickname, List<ProfilePokemon> listBox)
        {
            Nickname = nickname;
            ListBox = listBox;
        }
        public ItemCard? SelectItem(TypeItemCard type)
        {
            if (ListCards == null || ListCards.Count == 0)
                return null;

            // Filtra apenas os itens do tipo desejado
            var filtered = ListCards
                .Where(x => x.Key.Type == type)
                .ToDictionary(x => x.Key, x => x.Value);

            if (filtered.Count == 0)
            {
                return null;
            }

            // Cria lista de opções (somente visual)
            var itemList = filtered.Keys.ToList();
            var options = itemList.Select(k => k.ToString()).ToList();
            BattleLog.AddLog("Do not use any item card");

            // Exibe o menu
            int choice = ConsoleMenu.ShowMenu(ConsoleColor.Blue ,options, $"{Nickname}: Choose a Battle Item Card");

            // Se for a última opção, jogador optou por não usar item
            if (choice == options.Count - 1)
            {
                BattleLog.AddLog("No item card used.");
                return null;
            }

            // Obtém carta selecionada
            ItemCard selected = itemList[choice];

            // Reduz contador ou remove totalmente
            if (ListCards.ContainsKey(selected))
            {
                ListCards[selected]--;
                if (ListCards[selected] <= 0)
                    ListCards.Remove(selected);
            }

            return selected;
        }
        public static Pokemon ChoosePokemon(List<Pokemon> List, int[] number)
        {
            Pokemon pokemon = ConsoleMenu.ShowMenu(List.Where(p => number.Contains(p.NumberID)).ToDictionary(p => p, p => p.Name), "Choose a initial Pokemon");
            return pokemon;
        }

        public void CreateBox(List<Pokemon> allPokemons)
        {
                Console.WriteLine($"Choose your initial Pokémon:");
                var pokemon = ChoosePokemon(allPokemons, new int[] { 1, 4, 7 }); // Exemplo: escolher entre Bulbasaur, Charmander e Squirtle
                var initialPokemon = new ProfilePokemon(pokemon, pokemon.Name, 0);
                initialPokemon.NickName();
                ListBox.Add(initialPokemon);
                Console.WriteLine($"{initialPokemon.Name} added to your box!\n");
        }

        public static BoxPokemon FromText(string text, List<Pokemon> allPokemons, List<ItemCard> allItems)
        {
            string nickname = "";
            var pokemons = new List<ProfilePokemon>();
            Dictionary<ItemCard, int> items = new Dictionary<ItemCard, int>();

            var lines = text.Split('\n')
                            .Select(l => l.Trim())
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToList();

            bool readingPokemon = false;
            bool readingItem = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("profile"))
                {
                    var matchNickname = Regex.Match(line, @"profile\s*=\s*(.+?);");
                    if (matchNickname.Success)
                        nickname = matchNickname.Groups[1].Value.Trim();
                }
                else if (line.StartsWith("pokemon"))
                {
                    readingPokemon = true;
                    readingItem = false;
                }
                else if (line.StartsWith("item"))
                {
                    readingPokemon = false;
                    readingItem = true;
                }
                else if (line.StartsWith("}"))
                {
                    readingPokemon = false;
                    readingItem = false;
                }
                else if (readingPokemon)
                {
                    // Regex captura: ID, Level, Nickname (entre aspas), CardID
                    var match = Regex.Match(line, @"(\d+)\s*=\s*(\d+)\s*""([^""]*)""\s*,\s*(\d+)");
                    if (match.Success)
                    {
                        int id = int.Parse(match.Groups[1].Value);
                        int level = int.Parse(match.Groups[2].Value);
                        string pNickname = match.Groups[3].Value;
                        int cardID = int.Parse(match.Groups[4].Value);

                        // Busca Pokémon existente e cria cópia
                        var originalPokemon = allPokemons.FirstOrDefault(p => p.NumberID == id);
                        if (originalPokemon != null)
                        {
                            var pokemonCopy = new Pokemon(originalPokemon); // construtor de cópia
                            var profile = new ProfilePokemon(
                                pokemonCopy,
                                string.IsNullOrWhiteSpace(pNickname) ? pokemonCopy.Name : pNickname,
                                level
                            );

                            if (cardID != 0)
                                profile.AttachCard = allItems.FirstOrDefault(c => c.Id == cardID);

                            pokemons.Add(profile);
                            Console.WriteLine(profile.Name + " adicionado!");
                        }
                    }
                }
                else if (readingItem)
                {
                    var cleanLine = line.Replace(",", "").Replace(";", "");
                    var parts = cleanLine.Split('=');
                    if (parts.Length == 2)
                    {
                        int id = int.Parse(parts[0].Trim());
                        int quantity = int.Parse(parts[1].Trim());

                        var card = allItems.FirstOrDefault(c => c.Id == id);
                        if (card != null)
                        {
                            if (items.ContainsKey(card))
                                items[card] += quantity;
                            else
                                items[card] = quantity;
                        }
                    }
                }
            }

            var box = new BoxPokemon(nickname, pokemons);
            box.ListCards = items;
            return box;
        }
        public string SaveProfile()
        {
            
            string nl = Environment.NewLine; // quebra de linha correta para o SO
            string result = $"profile = {Nickname};{nl}";

            // Bloco de Pokémons
            result += "pokemon {" + nl;
            foreach (var p in ListBox)
            {
                int cardID = p.AttachCard?.Id ?? 0;
                string pName = string.IsNullOrWhiteSpace(p.Name) ? "" : p.Name;
                result += $"{p.Pokemon.NumberID} = {p.Level} \"{pName}\", {cardID},{nl}";
            }
            result += "}" + nl;

            // Bloco de Itens
            result += "item {" + nl;
            foreach (var kvp in ListCards)
            {
                result += $"{kvp.Key.Id} = {kvp.Value},{nl}";
            }
            result += "};" + nl;
            return result;
        }
        public override string ToString()
        {
            string description = "Pokemon: \n";
            foreach(var pokemon in ListBox)
            {
                description += $"{pokemon.Name} ({pokemon.Pokemon.Name}) Level: " + pokemon.Level;
                if (pokemon.AttachCard != null)
                {
                    description += ", segurando: " + pokemon.AttachCard.Name;
                }
                description += "\n";
            }
            description += "Item: \n";
            foreach(var item in ListCards)
            {
                description += item.ToString();
                description += "\n";
            }

            return Nickname + "\n" + description;
        }
    }
}
