using System;
using Minesweeper.Framework.MinePutters;

namespace Minesweeper.Framework
{
    public class MineField
    {

        public readonly FieldCell[,] Cells;
        public int TotalMines { get;}
        public int Width { get; }
        public int Height { get; }
        public int MinesLeft { get; private set; }
        public int FreeCellsLeft { get; private set; }
        public bool IsResolvable { get; }
        public bool UseRecursiveOpen { get; set; }
        public int TotalCells => Width * Height;

        private bool _isFirstTurn = true;
        private IMinePutter _minePutter;

        public event EventHandler Changed;

        public int CellSize => 64;

        public MineField(int width, int height, int totalMines, bool isResolvable, MinePutterDifficulty minePutterDifficulty)
        {
            Width = width;
            Height = height;
            IsResolvable = isResolvable;
            TotalMines = totalMines;
            _minePutter = new MinePutterFactory().Generate(minePutterDifficulty);

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(Width), "Width cannot be less or equal to zero");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(Height), "Height cannot be less or equal to zero");
            if (totalMines <= 0)
                throw new ArgumentOutOfRangeException(nameof(TotalMines), "Total mines cannot be less or equal to zero");
            if (totalMines >= TotalCells)
                throw new ArgumentOutOfRangeException(nameof(TotalCells), "Total mines cannot be greater or equal to total cells");

            Cells = new FieldCell[height, width];
        }

        public void Generate()
        {
            _isFirstTurn = true;
            MinesLeft = TotalMines;
            FreeCellsLeft = Width * Height - MinesLeft;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Cells[i, j] = new FieldCell(FieldCellType.NotMine, 0);
                }
            }

            RebuildOpenCells();

            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void RebuildOpenCells()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cell = Cells[i, j];

                    if (cell.Type == FieldCellType.Mine)
                        continue;

                    int minesAround = 0;

                    for (int i1 = -1; i1 <= 1; i1++)
                    {
                        for (int j1 = -1; j1 <= 1; j1++)
                        {
                            var xOffset = i + i1;
                            var yOffset = j + j1;

                            if (xOffset < 0 || xOffset >= Height
                                            || yOffset < 0 || yOffset >= Width)
                                continue;

                            if (Cells[xOffset, yOffset].Type == FieldCellType.Mine)
                                minesAround++;
                        }
                    }

                    cell.MinesAround = minesAround;
                }
            }
        }

        public bool RevealAt(int x, int y, bool ignoreMines = false)
        {
            if (y < 0 || y >= Height || x < 0 || x >= Width)
                return false;

            var cell = Cells[y, x];

            if (_isFirstTurn)
            {
                _minePutter.PutMines(this, x, y);
                RebuildOpenCells();
                _isFirstTurn = false;
            }

            if (cell.IsFlagged || cell.IsOpen)
                return false;
            if (ignoreMines && cell.IsMine)
                return false;

            cell.IsOpen = true;
            Changed?.Invoke(this, EventArgs.Empty);

            if (!ignoreMines && cell.IsMine)
                return true;

            if (cell.MinesAround == 0)
            {
                RevealAt(x - 1, y - 1);
                RevealAt(x - 1, y + 1);
                RevealAt(x + 1, y - 1);
                RevealAt(x + 1, y + 1);

                RevealAt(x - 1, y);
                RevealAt(x + 1, y);
                RevealAt(x, y - 1);
                RevealAt(x, y + 1);
            }

            return cell.IsMine;
        }

        public void FlagAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;
            
            var cell = Cells[y, x];

            if (cell.IsOpen)
                return;

            if (cell.IsFlagged)
            {
                MinesLeft++;
                cell.IsFlagged = false;
            }
            else
            {
                MinesLeft--;
                cell.IsFlagged = true;
            }
            
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}