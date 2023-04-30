using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//TODO add option to exit
namespace IS_VOD_Downloader
{
    public static class Menu
    {
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
            while(true)
            {
                DrawOptions(options, prompt);
                var selection = Console.ReadLine();

                //Valid input: number, numbers seperated by commas, numbers seperated by dash (range), any combination of those
                if (selection == null || !Regex.IsMatch(selection, "^\\s*\\d+(\\s*-\\s*\\d+)?(\\s*,\\s*\\d+(\\s*-\\s*\\d+)?)*\\s*$"))
                {
                    Console.WriteLine($"Invalid option '{selection}'");
                    continue;
                }

                var result = new List<int>();

                //parse the input
                var parts = selection.Replace(" ", String.Empty).Split(",");
                foreach (var part in parts)
                {
                    var range = part.Split("-");
                    if (range.Length == 1) {
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
                            result.AddRange(Enumerable.Range(start - 1, end - 1));
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
