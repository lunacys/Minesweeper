using System;

namespace Minesweeper.Framework.MinePutters
{
    public class MinePutterEasy : IMinePutter
    {
        public int PutMines(MineField mineField, int clickCellX, int clickCellY, Random random)
        {
            var x = clickCellX;
            var y = clickCellY;
            var generatedMines = 0;

            bool CheckAround(int i, int j) => 
                j != x && i != y // Same as clicked
                       && j - 1 != x && j + 1 != x // X offsets
                       && i + 1 != y && j - 1 != y; // Y offsets

            while (generatedMines < mineField.TotalMines)
            {
                for (int i = 0; i < mineField.Height; i++)
                {
                    for (int j = 0; j < mineField.Width; j++)
                    {
                        if (generatedMines >= mineField.TotalMines)
                            continue;
                        
                        if (CheckAround(i, j) && 
                            mineField.Cells[i, j].Type != FieldCellType.Mine && random.Next(1, mineField.Height * mineField.Width) == 1)
                        {
                            mineField.Cells[i, j].Type = FieldCellType.Mine;
                            generatedMines++;
                        }
                    }
                }
            }

            return generatedMines;
        }
    }
}