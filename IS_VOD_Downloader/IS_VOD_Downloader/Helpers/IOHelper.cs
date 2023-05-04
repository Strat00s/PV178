using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Helpers
{
    static class IOHelper
    {
        private static int _cursorPosition { get; set; }
        private static int _dotPosition{ get; set; }
        private static int _direction { get; set; }

        public static void FinishContinuous()
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.WriteLine($"[ OK ]");
            Console.CursorVisible = true;
        }
        public static string GetInput(string prompt)
        {
            string? result = null;
            while (result == null || result.Length == 0)
            {
                Console.Write(prompt);
                result = Console.ReadLine();
            }
            return result;
        }

        public static async Task<T> AnimateAwait<T>(Task<T> task, string message, bool continuous = false)
        {
            Console.Write($"[    ] {message}");
            Console.CursorVisible = false;
            Console.CursorLeft = 0;

            if (!continuous)
            {
                _cursorPosition = Console.CursorLeft;
                _direction = 1;
                _dotPosition = 1;
            }
            
            while (!task.IsCompleted)
            {
                Console.SetCursorPosition(_cursorPosition + _dotPosition, Console.CursorTop);
                Console.Write(".");

                await Task.Delay(200);

                Console.SetCursorPosition(_cursorPosition + _dotPosition, Console.CursorTop);
                Console.Write(" ");

                _dotPosition += _direction;
                if (_dotPosition == 4 || _dotPosition == 1)
                    _direction *= -1;
            }

            Console.SetCursorPosition(_cursorPosition, Console.CursorTop);
            //TODO propagate status???
            if (!continuous)
            {
                Console.WriteLine($"[ OK ] {message}");
                Console.CursorVisible = true;
            }

            return await task;
        }

        private static void DrawOptions(List<string> options, string prompt)
        {
            int i = 1;
            foreach (string option in options)
            {
                Console.WriteLine($"{i++}. {option}");
            }
            Console.Write($"{prompt} (1-{options.Count}): ");
        }

        public static int Select(List<string> options, string prompt)
        {
            if (options.Count == 1)
                return 0;

            while (true)
            {
                DrawOptions(options, prompt);
                var selection = Console.ReadLine();

                //check selection
                if (int.TryParse(selection, out var i))
                {
                    if (i > 0 && i <= options.Count)
                        return i - 1;
                }
                Console.WriteLine($"Invalid option '{selection}'");
            }
        }


        //return list of selected items
        public static List<int> MultiSelect(List<string> options, string prompt)
        {
            if (options.Count == 1)
                return new List<int>() { 0 };

            while (true)
            {
                DrawOptions(options, prompt);
                var selection = Console.ReadLine();

                //Valid input: number, numbers seperated by commas, numbers seperated by dash (range), any combination of those
                if (selection == null || !Regex.IsMatch(selection, "^\\s*\\d+(\\s*-\\s*\\d+)?(\\s*,\\s*\\d+(\\s*-\\s*\\d+)?)*\\s*$"))
                {
                    Console.WriteLine($"Invalid option '{selection}'");
                    continue;
                }

                Console.WriteLine(selection);

                var result = new List<int>();

                //parse the input
                var parts = selection.Replace(" ", String.Empty).Split(",");
                foreach (var part in parts)
                {
                    var range = part.Split("-");
                    if (range.Length == 1)
                    {
                        var index = int.Parse(range[0]);
                        if (index < 1 || index > options.Count)
                        {
                            Console.WriteLine($"Invalid input {index}");
                        }
                        else
                        {
                            result.Add(index - 1);
                        }
                    }
                    else
                    {
                        var start = int.Parse(range[0]);
                        var end = int.Parse(range[1]);
                        if (start < end && start > 0 && end <= options.Count)
                        {
                            result.AddRange(Enumerable.Range(start - 1, end - start + 1));
                        }
                        else
                        {
                            Console.WriteLine($"Invalid input {start} - {end}");
                        }
                    }
                }

                return result.Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }
    }
}
