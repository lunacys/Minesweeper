using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Minesweeper.Framework.MinePutters;

namespace Minesweeper.Framework
{
    public class MineField
    {

        public FieldCell[,] Cells { get; private set; }
        public int TotalMines { get;}
        public int Width { get; }
        public int Height { get; }
        public int MinesLeft { get; private set; }
        public int FreeCellsLeft { get; private set; }
        public int TotalOpenCells { get; private set; }
        public bool IsResolvable { get; }
        public bool UseRecursiveOpen { get; set; } = true;
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
            TotalOpenCells = 0;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Cells[i, j] = new FieldCell(FieldCellType.NotMine, 0);
                }
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Reset()
        {
            _isFirstTurn = true;
            MinesLeft = TotalMines;
            FreeCellsLeft = Width * Height - MinesLeft;
            TotalOpenCells = 0;
            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Cells[i, j].IsFlagged = false;
                    Cells[i, j].IsOpen = false;
                }
            }
            
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Solve()
        {
            // TODO: Add a proper algorithm
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cell = Cells[i, j];
                    if (cell.IsMine)
                    {
                        FlagAt(j, i);
                    }
                    else
                    {
                        RevealAt(j, i);
                    }
                }
            }
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

        public bool RevealAt(int x, int y, bool overrideRecursion = false)
        {
            if (MinesLeft == 0 && FreeCellsLeft == 0)
                return false;

            if (y < 0 || y >= Height || x < 0 || x >= Width)
                return false;

            var cell = Cells[y, x];

            if (_isFirstTurn)
            {
                var count = _minePutter.PutMines(this, x, y);
                Console.WriteLine(count);
                RebuildOpenCells();
                _isFirstTurn = false;
            }

            if (cell.IsFlagged)
                return false;
            if (cell.IsOpen)
            {
                if (UseRecursiveOpen && !_isFirstTurn && !overrideRecursion)
                {
                    var flags = GetFlagsAroundCell(x, y);

                    if (flags > 0 && flags == cell.MinesAround)
                    {
                        RevealAt(x - 1, y - 1, true);
                        RevealAt(x - 1, y + 1, true);
                        RevealAt(x + 1, y - 1, true);
                        RevealAt(x + 1, y + 1, true);

                        RevealAt(x - 1, y, true);
                        RevealAt(x + 1, y, true);
                        RevealAt(x, y - 1, true);
                        RevealAt(x, y + 1, true);
                    }
                    
                    return false;
                }
                else
                {
                    return false;
                }
            }

            cell.IsOpen = true;
            FreeCellsLeft--;
            Changed?.Invoke(this, EventArgs.Empty);

            if (cell.IsMine)
                return true;

            TotalOpenCells++;

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
            if (MinesLeft == 0 && FreeCellsLeft == 0)
                return;
            
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

            CheckCellsAroundForFlags(x, y);
            
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<Vector2> GetSuitableCellPositionsAt(int x, int y)
        {
            var cell = Cells[y, x];

            if (!cell.IsOpen)
                return new[] {new Vector2(x, y)};

            var list = new List<Vector2>();

            if (cell.IsFlagged || cell.MinesAround == 0)
                return list;

            var flagsAround = GetFlagsAroundCell(x, y);

            //if (cell.MinesAround == flagsAround)
            {
                return GetSuitableCellsAround(x, y);
            }

            return list;
        }

        public void RestoreFromSnapshot(MineFieldSnapshot snapshot)
        {
            Cells = snapshot.Cells;
            MinesLeft = snapshot.MinesLeft;
            FreeCellsLeft = snapshot.FreeCellsLeft;
            TotalOpenCells = snapshot.TotalOpenCells;
            
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void CheckCellsAroundForFlags(int x, int y)
        {
            for (int i1 = -1; i1 <= 1; i1++)
            {
                for (int j1 = -1; j1 <= 1; j1++)
                {
                    var xOffset = y + i1;
                    var yOffset = x + j1;

                    if (xOffset < 0 || xOffset >= Height
                                    || yOffset < 0 || yOffset >= Width)
                        continue;

                    var cell = Cells[xOffset, yOffset]; 

                    if (cell.IsOpen && !cell.IsMine && cell.MinesAround > 0 && GetFlagsAroundCell(yOffset, xOffset) > cell.MinesAround)
                        cell.IsWarned = true;
                    else
                        cell.IsWarned = false;
                }
            }
        }

        private IEnumerable<Vector2> GetSuitableCellsAround(int x, int y)
        {
            for (int i1 = -1; i1 <= 1; i1++)
            {
                for (int j1 = -1; j1 <= 1; j1++)
                {
                    var xOffset = y + i1;
                    var yOffset = x + j1;

                    if (xOffset < 0 || xOffset >= Height
                                    || yOffset < 0 || yOffset >= Width)
                        continue;

                    var cell = Cells[xOffset, yOffset];

                    if (!cell.IsFlagged && !cell.IsOpen)
                    {
                        yield return new Vector2(yOffset, xOffset);
                    }
                }
            }
        }
        
        private int GetFlagsAroundCell(int x, int y)
        {
            if (Cells[y, x].MinesAround == 0)
                return 0;
            
            int flagsAround = 0;

            for (int i1 = -1; i1 <= 1; i1++)
            {
                for (int j1 = -1; j1 <= 1; j1++)
                {
                    var xOffset = y + i1;
                    var yOffset = x + j1;

                    if (xOffset < 0 || xOffset >= Height
                                    || yOffset < 0 || yOffset >= Width)
                        continue;

                    if (Cells[xOffset, yOffset].IsFlagged)
                        flagsAround++;
                }
            }

            return flagsAround;
        }
    }
}