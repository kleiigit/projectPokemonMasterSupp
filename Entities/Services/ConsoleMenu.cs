using System;
using System.Collections.Generic;

namespace ProjetoPokemon
{
    internal static class ConsoleMenu
    {
        public static int ShowMenu(List<string> options, string title)
        {
            int selectedIndex = 0;

            ConsoleKey key;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"=== {title} ===\n");
                Console.ResetColor();

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {options[i]} <");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    selectedIndex = (selectedIndex == 0) ? options.Count - 1 : selectedIndex - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selectedIndex = (selectedIndex + 1) % options.Count;
                }

            } while (key != ConsoleKey.Enter);

            Console.Clear();
            return selectedIndex;
        }
    }
}

