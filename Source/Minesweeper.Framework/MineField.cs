using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Minesweeper.Framework.MinePutters;
using Point = Microsoft.Xna.Framework.Point;

namespace Minesweeper.Framework
{
    public class MineField
    {
        public Random Random { get; private set; }
        private int? _seed;
        public int Seed
        {
            get
            {
                if (!_seed.HasValue)
                    _seed = (int)GetCurrentUnixTimestampSeconds();

                return _seed.Value;
            }
            set
            {
                _seed = value;
                Random = new Random(_seed.Value);
            } 
        }
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
            // Seed = (int) GetCurrentUnixTimestampSeconds();
            Random = new Random();
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

                    cell.IsOpen = false;
                    cell.IsFlagged = false;
                    
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

        public PlayerTurnSnapshot RevealAt(int x, int y, bool overrideRecursion = false)
        {
            if (MinesLeft == 0 && FreeCellsLeft == 0)
                return null;

            if (y < 0 || y >= Height || x < 0 || x >= Width)
                return null;
            
            if (_isFirstTurn)
            {
                var count = _minePutter.PutMines(this, x, y, Random);
                RebuildOpenCells();
                _isFirstTurn = false;
            }
            
            var cell = Cells[y, x];
            var oldState = (FieldCell) cell.Clone();

            if (cell.IsFlagged)
                return null;
            
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
                    
                    return null;
                }
                else
                {
                    return null;
                }
            }

            cell.IsOpen = true;
            FreeCellsLeft--;
            Changed?.Invoke(this, EventArgs.Empty);

            if (!cell.IsMine)
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

            return new PlayerTurnSnapshot(new Point(x, y), cell, oldState);
        }

        public PlayerTurnSnapshot FlagAt(int x, int y)
        {
            if (MinesLeft == 0 && FreeCellsLeft == 0)
                return null;
            
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;
            
            var cell = Cells[y, x];

            if (cell.IsOpen)
                return null;

            var oldCell = (FieldCell) cell.Clone();

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
            
            return new PlayerTurnSnapshot(new Point(x, y), cell, oldCell);
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
        }

        public void RestoreFromSnapshot(MineFieldSnapshot snapshot)
        {
            Cells = CloneCells(snapshot.Cells);
            MinesLeft = snapshot.MinesLeft;
            FreeCellsLeft = snapshot.FreeCellsLeft;
            TotalOpenCells = snapshot.TotalOpenCells;
            
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public MineFieldSnapshot CreateSnapshot()
        {
            return new MineFieldSnapshot(CloneCells(Cells), MinesLeft, FreeCellsLeft, TotalOpenCells);
        }

        private FieldCell[,] CloneCells(FieldCell[,] oldCells)
        {
            var cells = new FieldCell[Height, Width];
            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cell = oldCells[i, j];

                    cells[i, j] = (FieldCell)cell.Clone();
                }
            }

            return cells;
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
        
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long) (DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime DateTimeFromUnixTimestampMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        public static long GetCurrentUnixTimestampSeconds()
        {
            return (long) (DateTime.UtcNow - UnixEpoch).TotalSeconds;
        }

        public static DateTime DateTimeFromUnixTimestampSeconds(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds);
        }
    }
}