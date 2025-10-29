
using ProjetoPokemon.Entities.Enums;

namespace ProjetoPokemon.Entities.Profiles
{
    internal class WildPokemon : ProfilePokemon
    {
        public WildPokemon(Pokemon pokemon, string name, int level) : base(pokemon, name, level)
        {
        }
        
        public override string ToString()
        {
            string status = "";
            string attachItem = "";
            if (Conditions != StatusConditions.NORMAL) status = $"[{Conditions.ToString()}]";
            if (AttachCard != null) attachItem = $" - Attached Item: {AttachCard.Name}";
            return Name + $" {status} Level: {LevelPokemon()} - Info: {Pokemon.ToString()}" + attachItem;
        }
    }
}
