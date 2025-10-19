using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities
{
    internal class ItemCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TypeItemCard Type { get; set; }
        public string Description { get; set; }

        public ItemCard(int id, string name, TypeItemCard type, string description)
        {
            Id = id;
            Name = name;
            Type = type;
            Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
