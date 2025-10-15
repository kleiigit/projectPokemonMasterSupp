using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Pokemon> pokemons = new List<Pokemon>();
            List<Move> movesList = new List<Move>();
            DataBaseControl.DataBase(pokemons, movesList);

            Console.Write("Escolha o ID do pokemon atacante: "); int pokemonA = int.Parse(Console.ReadLine());
            var pokeDisplay = pokemons.Where(x => x.NumberID == pokemonA).Select(p => p.Name).First();
            Console.WriteLine(pokeDisplay + " selecionado!");
            Console.Write("Escolha o ID do pokemon defensor: "); int pokemonB = int.Parse(Console.ReadLine());
            pokeDisplay = pokemons.Where(x => x.NumberID == pokemonB).Select(p => p.Name).First();
            Console.WriteLine(pokeDisplay + " selecionado!");
            BattleSimService.Simulate(pokemons, pokemonA, pokemonB);

        }
    }
}