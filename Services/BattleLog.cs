
using ProjetoPokemon.Entities;
using ProjetoPokemon.Entities.Battlers;
using System;

namespace ProjetoPokemon.Services
{
    internal class BattleLog
    {
        public static List<string> Logs { get; private set; } = new List<string>();
        public static List<string> setupPokemon { get; private set; } = new List<string>();
        public static List<string> setupItem { get; private set; } = new List<string>();
        public static List<string> setupMove { get; private set; } = new List<string>();
        public static List<string> calculationLog { get; private set; } = new List<string>();

        public BattleLog()
        {
            Logs = new List<string>();
        }
        public static void AddLog(string log)
        {
            Logs.Add(log);
        }
        public static void ClearLogs()
        {
            Logs.Clear();
        }
        public static void ShowSetupLogs(Battler battlerA, Battler battlerB)
        {
            
            LogPokemon(battlerA);
            LogPokemon(battlerB);
            Console.WriteLine();
            foreach (var log in setupPokemon)
            {
                Console.WriteLine(log);
            }
            
            LogCardItem(battlerA);
            LogCardItem(battlerB);
            Console.WriteLine();
            foreach (var log in setupItem)
            {
                Console.WriteLine(log);
            }
            
            LogMove(battlerA);
            LogMove(battlerB);
            Console.WriteLine();
            foreach (var log in setupMove)
            {
                Console.WriteLine(log);
            }

            Console.WriteLine();
            foreach (var log in calculationLog)
            {
                Console.WriteLine(log);
            }
        }
        private static void LogPokemon(Battler battler)
        {
            // select pokemon setup
            if (battler is BattlerPlayer player)
                setupPokemon.Add($"## {player.TrainerBox.Nickname} selected {player.Pokemon.GetName()} as their Pokémon.\n" + player.Pokemon.Pokemon.ToString());
            else if (battler is BattlerGym trainer)
                setupPokemon.Add($"## {trainer.Trainer.Name} selected {trainer.Pokemon.GetName()} as their Pokémon.\n" + trainer.Pokemon.Pokemon.ToString());
            else
                setupPokemon.Add($"## A {battler.Pokemon.GetName()} appeared!\n" + battler.Pokemon.Pokemon.ToString() + "");
        }
        private static void LogCardItem(Battler battler)
        {
            if (battler is BattlerPlayer playerCard && battler.UsedCard != null)
                setupItem.Add($"{playerCard.TrainerName} used the item card {battler.UsedCard.Name}!");
        }
        private static void LogMove(Battler battler)
        {
            setupMove.Add($"{battler.Pokemon.GetName()} used {battler.GetUsedMove().Name}.\n -> {battler.GetUsedMove()}");
        }

        public static void LogCalculation(string strings)
        {
            calculationLog.Add(strings);
        }
    }
}
