using CysmicEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CysmicSnake
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new SnakeGame((800, 600), "Snake");
            game.Play();
        }
    }
}
