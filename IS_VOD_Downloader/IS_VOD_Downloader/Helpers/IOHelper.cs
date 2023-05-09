using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IS_VOD_Downloader.Helpers
{
    static class IOHelper
    {
        private static int _cursorPosition { get; set; }
        private static int _dotPosition{ get; set; }
        private static int _direction { get; set; }


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

        private static void DrawOptions(List<string> options)
        {
            int i = 1;
            foreach (string option in options)
            {
                Console.WriteLine($"{i++}. {option}");
            }
        }

        public static int Select(List<string> options, string prompt)
        {
            if (options.Count == 1)
                return 0;

            DrawOptions(options);
            
            while (true)
            {
                Console.Write($"{prompt} (1-{options.Count}): ");
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

            DrawOptions(options);

            while (true)
            {
                Console.Write($"{prompt} (1-{options.Count}): ");
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
                bool parseOk = true;
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
                            parseOk = false;
                            break;
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
                            parseOk = false;
                            break;
                        }
                    }
                }

                if (!parseOk)
                    continue;

                return result.Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public static bool BoolSelect(string accept, string decline, string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} [{accept}/{decline}]: ");
                var option = Console.ReadLine();

                if (option == string.Empty || option == null) 
                {
                    if (char.IsUpper(accept[0]))
                        return true;
                    if (char.IsUpper(decline[0]))
                        return false;
                    continue;
                }

                option = option.ToLower();
                if (option == accept.ToLower() || option[0] == accept.ToLower()[0])
                    return true;
                if (option == decline.ToLower() || option[0] == decline.ToLower()[0])
                    return false;
                Console.WriteLine($"Invalid option '{option}'");
            }
        }

        //Animations
        private static void DrawProgressBar(int cursorTop, int progressBarWidth, string prefix, int progress, int max)
        {
            double progressFraction = (double)progress / max;

            Console.SetCursorPosition(0, cursorTop);
            Console.Write($"{prefix}[");

            int completedBars = (int)(progressBarWidth * progressFraction);

            for (int i = 0; i < progressBarWidth; i++)
            {
                Console.Write(i < completedBars ? "█" : " ");
            }

            Console.Write($"] {(int)(progressFraction * 100), 3}%");
            if (progress == max)
                Console.Write(" Done");
        }

        //TODO make it asynchronous
        public static async Task AnimateProgressAsync(ThreadSafeInt downloadProg, int downloadMax, ThreadSafeInt decryptProg, int decryptMax)
        {
            int progressBarWidth = Console.WindowWidth - 30;

            Console.CursorVisible = false;

            int cursorTop1 = Console.CursorTop - 1;
            int cursorTop2 = Console.CursorTop;

            int lastDownload = 0;
            int lastDecrypt = 0;

            while (lastDownload < downloadMax || lastDecrypt < decryptMax)
            {
                lastDownload = downloadProg.Value;
                lastDecrypt = decryptProg.Value;
                DrawProgressBar(cursorTop1, progressBarWidth, "Downloaded: ", lastDownload, downloadMax);
                DrawProgressBar(cursorTop2, progressBarWidth, "Decrypted:  ", lastDecrypt, decryptMax);
                await Task.Delay(10);   //make it "async"
            }

            //TODO async wait for progress to change

            Console.CursorVisible = true;
            Console.WriteLine("\n");
        }


        public static async Task<T> AnimateAwaitAsync<T>(Task<T> task, string message, bool continuous = false)
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
                if (task.IsFaulted)
                    Console.WriteLine($"[FAIL] {message}");
                else
                    Console.WriteLine($"[ OK ] {message}");
                Console.CursorVisible = true;
            }

            return await task;
        }

        public static void FinishContinuous(bool failed = false)
        {
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            if (failed)
                Console.WriteLine($"[FAIL]");
            else
                Console.WriteLine($"[ OK ]");
            Console.CursorVisible = true;
        }
    }
}
