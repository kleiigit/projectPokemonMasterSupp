using AuxiliarCampanha.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine("Rolagem para evitar paralizia: " + rollCondition);
            if (rollCondition == 1)
            {
                return false;
            }
            return true;
        }
        public static bool FrozenRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            Console.WriteLine("Rolagem para sair do congelamento: " + rollCondition);
            if (rollCondition == 4)
            {
                return false;
            }
            return true;
        }
        public static int SleepRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            Console.WriteLine("Rolagem definir o nível de sono: " + rollCondition);
            return rollCondition;

        }

    }
}
