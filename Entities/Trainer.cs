
using ProjetoPokemon.Enums;

namespace ProjetoPokemon.Entities
{
    internal class Trainer
    {
        public int TrainerID { get;}
        public TrainerClass Class { get;}
        public string Name { get;}
        public List<ProfilePokemon> ListPokemon { get;}
        public ColorToken? TrainerColor { get;}
        public ItemCard? Reward { get;}

        public Trainer(int trainerID, TrainerClass trainerClass, string name, List<ProfilePokemon> listPokemon)
        {
            TrainerID = trainerID;
            Class = trainerClass;
            Name = name;
            ListPokemon = listPokemon;
        }
    }
}
