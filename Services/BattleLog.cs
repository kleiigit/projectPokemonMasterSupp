
namespace ProjetoPokemon.Services
{
    internal class BattleLog
    {
        public static List<string> Logs { get; private set; } = new List<string>();
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
        public static void ShowLogs()
        {
            foreach (var log in Logs)
            {
                Console.WriteLine(log);
            }
        }
    }
}
