using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

namespace ProjetoPokemon.Entities
{
    internal class BoxPokemon
    {
        public string Nickname { get; set; }
        public List<ProfilePokemon> ListBox = new List<ProfilePokemon>();
        public Dictionary<ItemCard,int> ListCards = new Dictionary<ItemCard,int>();

        public BoxPokemon(string nickname, List<ProfilePokemon> listBox)
        {
            Nickname = nickname;
            ListBox = listBox;
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
            }

            return Nickname + "\n" + description;
        }
    }
}
