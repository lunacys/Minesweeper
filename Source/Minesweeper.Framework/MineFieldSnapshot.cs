using Minesweeper.Framework.MinePutters;

namespace Minesweeper.Framework
{
    public class MineFieldSnapshot
    {
        public FieldCell[,] Cells { get; }
        public int MinesLeft { get; }
        public int FreeCellsLeft { get; }
        public int TotalOpenCells { get; }
        public int Width { get; }
        public int Height { get; }
        public int TotalMines { get; }
        public bool IsResolvable { get; }
        public MinePutterDifficulty MinePutterDifficulty { get; }

        public MineFieldSnapshot(
            FieldCell[,] cells,
            int minesLeft,
            int freeCellsLeft,
            int totalOpenCells,
            int width,
            int height,
            int totalMines,
            bool isResolvable,
            MinePutterDifficulty minePutterDifficulty
        )
        {
            Cells = cells;
            MinesLeft = minesLeft;
            FreeCellsLeft = freeCellsLeft;
            TotalOpenCells = totalOpenCells;
            Width = width;
            Height = height;
            TotalMines = totalMines;
            IsResolvable = isResolvable;
            MinePutterDifficulty = minePutterDifficulty;
        }
    }
}