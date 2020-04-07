using System;

namespace Minesweeper.Framework
{
    public class MineField
    {

        public readonly FieldCell[,] Cells;
        public int TotalMines { get;}
        public int Width { get; }
        public int Height { get; }
        public bool IsResolvable { get; }
        public int TotalCells => Width * Height;

        private int _generatedMines = 0;
        private Random _random;

        public event EventHandler Changed;

        public int CellSize => 64;

        public MineField(int width, int height, int totalMines, bool isResolvable)
        {
            Width = width;
            Height = height;
            IsResolvable = isResolvable;
            TotalMines = totalMines;

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(Width), "Width cannot be less or equal to zero");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(Height), "Height cannot be less or equal to zero");
            if (totalMines <= 0)
                throw new ArgumentOutOfRangeException(nameof(TotalMines), "Total mines cannot be less or equal to zero");
            if (totalMines >= TotalCells)
                throw new ArgumentOutOfRangeException(nameof(TotalCells), "Total mines cannot be greater or equal to total cells");

            Cells = new FieldCell[height, width];
            _random = new Random();
        }

        public void Generate()
        {
            _generatedMines = 0;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Cells[i, j] = new FieldCell(FieldCellType.NotMine, 0);
                }
            }

            while (_generatedMines < TotalMines)
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (Cells[i, j].Type != FieldCellType.Mine && _random.Next(1, Height * Width) == 1)
                        {
                            Cells[i, j].Type = FieldCellType.Mine;
                            _generatedMines++;
                        }
                    }
                }
            }

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

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool RevealAt(int x, int y, bool ignoreMines = false)
        {
            if (y < 0 || y >= Height || x < 0 || x >= Width)
                return false;

            var cell = Cells[y, x];

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
            var cell = Cells[y, x];

            if (cell.IsOpen)
                return;

            cell.IsFlagged = !cell.IsFlagged;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}