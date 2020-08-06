using System;

namespace Minesweeper.Framework.MinePutters
{
    public class MinePutterHard : IMinePutter
    {
        public MinePutterDifficulty Difficulty => MinePutterDifficulty.Hard;

        public int PutMines(MineField mineField, int clickCellX, int clickCellY, Random random)
        {
            var generatedMines = 0;
            
            while (generatedMines < mineField.TotalMines)
            {
                for (int i = 0; i < mineField.Height; i++)
                {
                    for (int j = 0; j < mineField.Width; j++)
                    {
                        if (generatedMines >= mineField.TotalMines)
                            continue;
                        
                        if (j != clickCellX && i != clickCellY && 
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