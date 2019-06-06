using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public class MineField
    {
        private Cell[,] _cells;
        public int TotalMines { get; }
        public int Width { get; }
        public int Height { get; }

        public int TotalCells => Width * Height;

        private int _generatedMines = 0;

        private Random _random;

        public MineField(int width, int height, int totalMines)
        {
            Width = width;
            Height = height;
            TotalMines = totalMines;

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(Width), "Width cannot be less or equal to zero");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(Height), "Height cannot be less or equal to zero");
            if (totalMines <= 0)
                throw new ArgumentOutOfRangeException(nameof(TotalMines), "Total mines cannot be less or equal to zero");
            if (totalMines >= TotalCells)
                throw new ArgumentOutOfRangeException(nameof(TotalCells), "Total mines cannot be greater or equal to total cells");

            _cells = new Cell[height, width];
            _random = new Random();
        }

        public void Generate()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    _cells[i, j] = new Cell(CellType.NotMine, 0);
                }
            }

            while (_generatedMines < TotalMines)
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (_cells[i, j].Type != CellType.Mine && _random.Next(1, Height * Width) == 1)
                        {
                            _cells[i, j].Type = CellType.Mine;
                            _generatedMines++;
                        }
                    }
                }
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cell = _cells[i, j];

                    if (cell.Type == CellType.Mine)
                        continue;

                    int minesAround = 0;

                    for (int i1 = -1; i1 <= 1; i1++)
                    {
                        for (int j1 = -1; j1 <= 1; j1++)
                        {
                            var xOffset = i + i1;
                            var yOffset = j + j1;

                            if (xOffset < 0 || xOffset >= Width 
                                || yOffset < 0 || yOffset >= Height)
                                continue;

                            if (_cells[xOffset, yOffset].Type == CellType.Mine)
                                minesAround++;
                        }
                    }

                    cell.MinesAround = minesAround;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var cell = _cells[i, j];

                    if (cell.Type == CellType.Mine)
                    {
                        spriteBatch.Draw(AssetBank.MineCellTexture, new Vector2(i * 32, j * 32), null, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(AssetBank.Cells[cell.MinesAround], new Vector2(i * 32, j * 32), null, Color.White);
                    }
                }
            }

            spriteBatch.End();
        }
    }
}