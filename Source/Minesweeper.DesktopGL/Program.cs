using System;
using Minesweeper.Framework;

namespace Minesweeper.DesktopGL
{
    static class Program
    {
        static void Main(string[] args)
        {
            using var game = new GameRoot();
            game.Run();
        }
    }
}
