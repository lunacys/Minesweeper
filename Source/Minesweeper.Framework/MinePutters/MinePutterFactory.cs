using System;

namespace Minesweeper.Framework.MinePutters
{
    public class MinePutterFactory
    {
        public IMinePutter Generate(MinePutterDifficulty difficulty)
        {
            switch (difficulty)
            {
                case MinePutterDifficulty.Random: return new MinePutterRandom();
                case MinePutterDifficulty.Hard: return new MinePutterHard();
                case MinePutterDifficulty.Easy: return new MinePutterEasy();
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
        }
    }
}