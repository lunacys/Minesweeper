using System;

namespace Minesweeper.Framework.MinePutters
{
    public interface IMinePutter
    {
        int PutMines(MineField mineField, int clickCellX, int clickCellY, Random random);
    }
}