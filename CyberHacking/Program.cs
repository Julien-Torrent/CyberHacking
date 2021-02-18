using System;
using System.Collections.Generic;

namespace CyberHacking
{
    class Program
    {
        static void Main()
        {
            var ch = new CyberHacking()
            {
                BufferSize = 5,
                Sequence = new List<int> { 233, 28, 85, 189 },
            };

            ch.Start();

            do
            {
                ch.PrintSequence();
                ch.PrintBuffer();
                ch.PrintGrid();
                ch.Play(int.Parse(Console.ReadLine()));
                Console.Clear();
            } while (!ch.IsGameOver);

            if (ch.GameState == State.Won)
                Console.WriteLine("You won");
            else
                Console.WriteLine("Game Over");
        }
    }
}
