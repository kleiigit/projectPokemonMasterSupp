using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuxiliarCampanha.Entities.Services
{
    internal class DiceRollService
    {
        private static Random random = new Random();

        public static int RollDice(int numberOfDice, int sidesPerDie)
        {
            int total = 0;
            for (int i = 0; i < numberOfDice; i++)
            {
                total += random.Next(1, sidesPerDie + 1);
            }
            return total;
        }

        public static int RollD4() => RollDice(1, 4);
        public static int RollD6() => RollDice(1, 6);
        public static int RollD8() => RollDice(1, 8);
        public static int RollD10() => RollDice(1, 10);
        public static int RollD12() => RollDice(1, 12);
        public static int RollD20() => RollDice(1, 20);
        public static int RollD100() => RollDice(1, 100);
    }
}
