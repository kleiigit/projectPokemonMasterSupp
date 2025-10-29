
using ProjetoPokemon.Entities.Profiles;
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
            BattleLog.AddLog("\nRoll to avoid paralysis (d4 > 1): " + rollCondition);
            if (rollCondition == 1)
            {
                return false;
            }
            return true;
        }
        public static bool FrozenRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            BattleLog.AddLog("\nRoll to avoid frozen (d4 = 4): " + rollCondition);
            if (rollCondition == 4)
            {
                return true;
            }
            return false;
        }
        public static int SleepRoll()
        {
            int rollCondition = DiceRollService.RollD4();
            BattleLog.AddLog("\nRoll to define sleeping level: " + rollCondition);
            return rollCondition;

        }

        public static void PoisonRoll(BattlerManager pokemon) // custom poison effect
        {
            int damage = DiceRollService.RollD6();
            BattleLog.AddLog("\nRoll to resist the poison (d6 < 6): " + damage);
            if (damage == 6) {
                BattleLog.AddLog($"{pokemon} was knocked out by poison!"); 
                pokemon.SelectedPokemon.Conditions = StatusConditions.KNOCKED;
            }
        }

    }
}
