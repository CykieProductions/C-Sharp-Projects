using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CysmicEngine.Demo_Game;

namespace CysmicEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoGame demoGame = new DemoGame();
            demoGame.Play();
        }
    }
}

namespace CyTools
{
    public class Basic
    {
        /*public const string _UNDERLINE = "\x1B[4m";
        public const string _RESET = "\x1B[0m";*/
        public static Random r = new Random();

        public static T print<T>(T text, bool sameLine = false)//, int textColor = -1, int backgroundColor = -1)
        {
            /*var prevFC = Console.ForegroundColor;
            if (textColor == -1) textColor = (int)prevFC;
            var prevBC = Console.BackgroundColor;
            if (backgroundColor == -1) backgroundColor = (int)prevBC;

            Console.ForegroundColor = (ConsoleColor)textColor;
            Console.BackgroundColor = (ConsoleColor)backgroundColor;*/

            if (sameLine)
                Console.Write(text);
            else
                Console.WriteLine(text);

            //Console.ForegroundColor = prevFC;
            //Console.BackgroundColor = prevBC;

            return text;
        }
        /// <summary>
        /// Generate a random integer between and including the min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomInt(int min, int max)
        {
            Random r2 = new Random(r.Next(-999, 999));
            Random r3 = new Random(r.Next(-r2.Next(), 999));

            int randInt = r3.Next(0, 3);
            if (randInt == 0)
                return r.Next(min, max + 1);
            else if (randInt == 1)
                return r2.Next(min, max + 1);
            else
                return r3.Next(min, max + 1);
        }

        /// <summary>
        /// Returns true based on a "x" in "y" chance
        /// </summary>
        /// <param name="x">must be 1 or greater and less than "y"</param>
        /// <param name="y">can't be lower than "x"</param>
        /// <returns>the result of the dice roll</returns>
        public static bool Chance(int x, int y)
        {
            return RandomInt(1, y.Clamp(1, int.MaxValue)) <= x.Clamp(1, y);
        }
    }

    public static class Extentions
    {
        public static int ToInt(this ConsoleColor consoleColor) { return (int)consoleColor; }

        /// <summary>
        /// Clamps any valid *number* between the min and max *number*
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min">Inclusive minimum</param>
        /// <param name="max">Inclusive maximum</param>
        /// <returns></returns>
        public static T Clamp<T>(this T value, T min, T max) where T: struct
        {
            dynamic v = value;
            if (v.GetType() != typeof(int) && v.GetType() != typeof(float))
                return v;

            if (v > max)
                v = max;
            else if (v < min)
                v = min;

            return v;
        }

        //https://stackoverflow.com/questions/56692/random-weighted-choice
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);

        }
    }
}
