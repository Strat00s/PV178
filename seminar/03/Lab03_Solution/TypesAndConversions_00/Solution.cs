using System;

namespace TypesAndConversions_00
{
    public static class Solution
    {
        public enum Color { Red, Green, Blue }

        public static void Task01()
        {
            var color = Color.Green;

            var colorString = color.ToString();

            color = (Color)Enum.Parse(typeof(Color), colorString);         
            color = (Color)Enum.Parse(typeof(Color), colorString, true);    // case insensitive variant
        }

        public static void Task02()
        {
            var intValue = 1234;
            var hexValue = "4D2";
            
            // from int to HEX
            var convertedHex = intValue.ToString("X");

            // from HEX to int
            var convertedInt = Convert.ToInt32(hexValue, 16);  

            Console.ReadKey();
        }
    }
}
