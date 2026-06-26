using System;
using System.Threading;

namespace PROG6221_V1
{
    /// <summary>
    /// Provides helper methods for console output formatting.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Sets the console foreground and background colors.
        /// </summary>
        public static void SetConsoleColor(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        /// <summary>
        /// Writes text in a specified color and then resets to default.
        /// </summary>
        public static void WriteColored(string text, ConsoleColor color)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = original;
        }

        /// <summary>
        /// Writes a line of text with a typing effect (character-by-character delay).
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="delayMilliseconds">Delay between characters (default 30ms).</param>
        public static void WriteLineWithDelay(string text, int delayMilliseconds = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMilliseconds);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Prints a decorative separator line using a specified character.
        /// </summary>
        public static void PrintSeparator(char ch = '-', int length = 70)
        {
            Console.WriteLine(new string(ch, length));
        }

        /// <summary>
        /// Prints a section header with decorative borders.
        /// </summary>
        public static void PrintHeader(string title, ConsoleColor color = ConsoleColor.Cyan)
        {
            WriteColored($"\n╔════════════════════════════════════════════════════════════╗\n", color);
            WriteColored($"║{title.PadLeft(40 + title.Length / 2).PadRight(58)}║\n", color);
            WriteColored($"╚════════════════════════════════════════════════════════════╝\n", color);
        }

        /// <summary>
        /// Prints a menu item with proper formatting.
        /// </summary>
        public static void PrintMenuItem(string number, string description)
        {
            WriteColored($"  {number}. ", ConsoleColor.Yellow);
            Console.WriteLine(description);
        }

        /// <summary>
        /// Clears the console and resets colors (optional).
        /// </summary>
        public static void ClearConsole()
        {
            Console.Clear();
            SetConsoleColor(ConsoleColor.Cyan, ConsoleColor.Black);
        }
    }
}