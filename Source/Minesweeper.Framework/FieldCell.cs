namespace Minesweeper.Framework
{
    public class FieldCell
    {
        public FieldCellType Type { get; set; }
        public int MinesAround { get; set; }
        public bool IsOpen { get; set; }
        public bool IsMine => Type == FieldCellType.Mine;
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Gets or sets whether a waring overlay will be drawn on the cell
        /// </summary>
        public bool IsWarned { get; set; }

        public FieldCell(FieldCellType type, int minesAround)
        {
            Type = type;
            MinesAround = minesAround;
        }
    }
}