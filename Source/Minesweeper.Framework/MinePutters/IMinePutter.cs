namespace Minesweeper.Framework.MinePutters
{
    public interface IMinePutter
    {
        void PutMines(MineField mineField, int clickCellX, int clickCellY);
    }
}