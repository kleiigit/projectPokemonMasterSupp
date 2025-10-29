
using ProjetoPokemon.Entities.Profiles;
using System.Collections.ObjectModel;

namespace ProjetoPokemon.Entities.Data
{
    internal static class DataLists
    {
        private static readonly List<Pokemon> _allPokemons = new List<Pokemon>();
        private static readonly List<Move> _allMoves = new List<Move>();
        private static readonly List<ItemCard> _allItemCards = new List<ItemCard>();
        private static readonly List<BoxPokemon> _allProfiles = new List<BoxPokemon>();

        // Exposição apenas para leitura (não permite Add/Remove externos)
        public static ReadOnlyCollection<Pokemon> AllPokemons => _allPokemons.AsReadOnly();
        public static ReadOnlyCollection<Move> AllMoves => _allMoves.AsReadOnly();
        public static ReadOnlyCollection<ItemCard> AllItemCards => _allItemCards.AsReadOnly();
        public static ReadOnlyCollection<BoxPokemon> AllProfiles => _allProfiles.AsReadOnly();

        public static void AddPokemon(Pokemon pokemon) { _allPokemons.Add(pokemon); }
        public static Pokemon GetPokemonID(int id) { return _allPokemons.First(p => p.NumberID == id); }

        public static void AddMove(Move move) { _allMoves.Add(move); }
        public static Move GetMoveID(int id) { return _allMoves.First(m => m.MoveID == id); }

        public static void AddItemCard(ItemCard itemCard) { _allItemCards.Add(itemCard); }
        public static ItemCard GetItemCardID(int id) { return _allItemCards.First(i => i.Id == id); }
        public static void AddProfile(BoxPokemon profile) { _allProfiles.Add(profile); }
    }
}
