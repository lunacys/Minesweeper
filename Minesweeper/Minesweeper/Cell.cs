namespace Minesweeper
{
    public class Cell
    {
        public CellType Type { get; set; }
        public int MinesAround { get; set; }

        public Cell(CellType type, int minesAround)
        {
            Type = type;
            MinesAround = minesAround;
        }
    }
}