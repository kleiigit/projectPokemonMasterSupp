
using ProjetoPokemon.Entities.Services;

namespace ProjetoPokemon.Entities.Enums
{
    enum StatusConditions
    {
        NORMAL,
        POISONED,
        FROZEN,
        BURNED,
        SLEEP,
        PARALYZED,
        CONFUSED,
        KNOCKED,
    }

    class BattleConditions
    {
        public static bool ParalyzedRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            Console.WriteLine("Roll to avoid paralysis (d4 > 1): " + rollCondition);
            Console.ReadLine();
            if (rollCondition == 1)
            {
                return false;
            }
            return true;
        }
        public static bool FrozenRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            Console.WriteLine("Rolagem para sair do congelamento (d4 = 4): " + rollCondition);
            if (rollCondition == 4)
            {
                return true;
            }
            return false;
        }
        public static int SleepRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            Console.WriteLine("Rolagem definir o nível de sono: " + rollCondition);
            return rollCondition;

        }

        public static void PoisonRoll(BattleModifications pokemon) // custom poison effect
        {
            int damage = DiceRollService.RollD6();
            Console.WriteLine("Roll to resist the poison (d6 < 6): " + damage);
            if (damage == 6) { 
                Console.WriteLine($"{pokemon} was knocked out by poison!"); 
                pokemon.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            }
            Console.WriteLine();
        }

    }
}
