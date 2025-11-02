
using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;

namespace ProjetoPokemon.Services
{
    static class MasterGame
    {
        private static readonly List<ItemCard> ItemCardDeck = new List<ItemCard>();




        public static void CreateGameItemDeck()
        {
            foreach (var item in DataLists.AllItemCards)
            {
                switch(item.Rarity)
                {
                    case RarityCard.Common: AddItemDeck(item, 6); break;
                    case RarityCard.Uncommon: AddItemDeck(item, 4); break;
                    case RarityCard.Rare: AddItemDeck(item, 2); break;
                    case RarityCard.Epic: AddItemDeck(item, 1); break;
                    case RarityCard.Unique: // No add 
                        break;
                }
            }
            Shuffle(ItemCardDeck);
        }
        public static void AddItemDeck(ItemCard itemCard, int count)
        {
            for (var i = 0; i < count; i++)
            {
                ItemCardDeck.Add(itemCard);
            }
        }
        public static ItemCard? DrawItemCard()
        {
            if (ItemCardDeck.Count == 0 || ItemCardDeck == null) 
                return null;

                ItemCard drawCard = ItemCardDeck[0];
                ItemCardDeck.RemoveAt(0);
                Console.WriteLine("Draw item " + drawCard.Name + $"[{ItemCardDeck.Count}]");
                return drawCard;
            
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rnd = new Random();
            if (list == null || list.Count <= 1)
                return;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
