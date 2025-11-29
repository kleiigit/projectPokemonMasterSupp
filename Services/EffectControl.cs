using ProjetoPokemon.Data;
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using ProjetoPokemon.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoPokemon.Services
{
    internal static class EffectControl
    {
        public static void HiddenPower(ProfilePokemon profile)
        {
            if (profile.Pokemon.Moves.Any(p => p.MoveID == 173))
            {
                int typeCount = DiceRollService.RollDice(1, Enum.GetValues(typeof(TypePokemon)).Length - 1);
                Move filtredMove = profile.Pokemon.Moves.Where(p => p.MoveID == 173).First();
                filtredMove.ChangeType((TypePokemon)typeCount);
            }
        }
        public static bool SleepTalk(Battler battler)
        {
            if (battler.MovesPokemon.Any(p => p.MoveID == 218) && battler.Pokemon.Conditions == StatusConditions.SLEEP)
            {
                battler.SetUsedMove(battler.RandomUsedMove()); // forçar o randomico
                BattleLog.AddLog(battler + $" is still sleeping and use {battler.GetUsedMoveName()} with {DataLists.GetMoveID(218).Name}"); // sleep talk effect

                return false;
            }
            return false;
        }
    }
}
