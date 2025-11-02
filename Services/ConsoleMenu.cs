namespace ProjetoPokemon.Services
{
    internal static class ConsoleMenu
    {
        public static int ShowMenu(ConsoleColor menuColor, List<string> options, string title)
        {
            int selectedIndex = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.ForegroundColor = menuColor; //definições de cores do menu
                Console.WriteLine($"=== {title} ===\n");
                Console.ResetColor();

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                    selectedIndex = selectedIndex == 0 ? options.Count - 1 : selectedIndex - 1;
                else if (key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex + 1) % options.Count;

            } while (key != ConsoleKey.Enter);

            Console.Clear();
            return selectedIndex;
        }

        public static TKey ShowMenu<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string title)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return default;
            }

            var keys = dictionary.Keys.ToList();

            // Cria as opções de exibição: ToString() da chave + quantidade
            var options = dictionary.Select(kv =>
                $"{kv.Key.ToString()} x{kv.Value}").ToList();

            int selectedIndex = ShowMenu(ConsoleColor.Cyan, options, title);

            // Retorna a chave (Key) selecionada
            return keys[selectedIndex];
        }

        public static bool ShowYesNo(string question)
        {
            var options = new List<string> { "Yes", "No" };
            int choice = ShowMenu(ConsoleColor.Cyan, options, question);
            return choice == 0; // 0 = Yes, 1 = No
        }
        public static string YesOrNoString(string question)
        {
            var options = new List<string> { "Yes", "No" };
            int choice = ShowMenu(ConsoleColor.Cyan, options, question);
            return options[choice];
        }
    }
}
