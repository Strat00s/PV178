using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//TODO add option to exit
namespace IS_VOD_Downloader
{
    public static class Menu
    {
        public static int Draw(List<string> options, string prompt)
        {
            int i;
            while (true)
            {
                i = 1;
                foreach (string option in options)
                {
                    Console.WriteLine($"{i++}. {option}");
                }
                Console.Write($"{prompt} (1-{options.Count}): ");
                var selection = Console.ReadLine();
                
                //check selection
                if (int.TryParse(selection, out i ))
                {
                    if (i > 0 && i <= options.Count)
                        return i - 1;
                }
                Console.WriteLine($"Invalid option {selection}");
            }

            //return await Task.Run(() => 
            //    {
            //        return 0;
            //    });
        }
    }
}
