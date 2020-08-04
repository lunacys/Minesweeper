using System;

namespace Minesweeper.Framework.MinePutters
{
    public class MinePutterRandom : IMinePutter
    {
        private Random _random = new Random();
        
        public int PutMines(MineField mineField, int clickCellX, int clickCellY)
        {
            int generatedMines = 0;
            
            while (generatedMines < mineField.TotalMines)
            {
                for (int i = 0; i < mineField.Height; i++)
                {
                    for (int j = 0; j < mineField.Width; j++)
                    {
                        if (generatedMines >= mineField.TotalMines)
                            continue;

                        if (mineField.Cells[i, j].Type != FieldCellType.Mine 
                            && _random.Next(1, mineField.Height * mineField.Width) == 1)
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