namespace Minesweeper.Framework
{
    public class MineFieldSnapshot
    {
        public FieldCell[,] Cells { get; set; }
        public int MinesLeft { get; set; }
        public int FreeCellsLeft { get; set; }
        public int TotalOpenCells { get; set; }

        public MineFieldSnapshot(FieldCell[,] cells, int minesLeft, int freeCellsLeft, int totalOpenCells)
        {
            Cells = cells;
            MinesLeft = minesLeft;
            FreeCellsLeft = freeCellsLeft;
            TotalOpenCells = totalOpenCells;
        }
    }
}