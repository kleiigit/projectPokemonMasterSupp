
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

    }
}
