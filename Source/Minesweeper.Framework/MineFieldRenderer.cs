using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Minesweeper.Framework
{
    public class MineFieldRenderer
    {
        private class TextureResolver
        {
            private Rectangle[] _rectangles;
            private int _tileCountW;
            private int _tileCountH;

            public TextureResolver(Texture2D tileSet, int tileWidth, int tileHeight)
            {
                _tileCountW = tileSet.Width / tileWidth;
                _tileCountH = tileSet.Height / tileHeight;

                _rectangles = new Rectangle[_tileCountW * _tileCountH];

                for (int i = 0; i < _tileCountW; i++)
                {
                    for (int j = 0; j < _tileCountH; j++)
                    {
                        _rectangles[i + _tileCountW * j] = new Rectangle(
                            i * tileWidth,
                            j * tileHeight,
                            tileWidth,
                            tileHeight
                        );
                    }
                }
            }

            public Rectangle GetSourceRectForCell(FieldCell cell)
            {
                if (cell.IsFlagged)
                    return _rectangles[_tileCountW * _tileCountH - 2];
                if (!cell.IsOpen)
                    return _rectangles[_tileCountW * _tileCountH - 3];
                if (cell.IsMine)
                    return _rectangles[_tileCountW * _tileCountH - 1];

                return _rectangles[cell.MinesAround];
            }
        }

        public RenderTarget2D RenderTarget { get; }

        public MineField MineField { get; }

        public GraphicsDevice GraphicsDevice { get; }

        private readonly SpriteBatch _spriteBatch;

        private TextureResolver _textureResolver;

        private Texture2D _tileSet;

        public MineFieldRenderer(MineField mineField, GraphicsDevice graphicsDevice, Texture2D tileSet)
        {
            MineField = mineField;
            GraphicsDevice = graphicsDevice;
            _tileSet = tileSet;
            _textureResolver = new TextureResolver(tileSet, MineField.CellSize, MineField.CellSize);

            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                mineField.Width * mineField.CellSize,
                mineField.Height * mineField.CellSize,
                true,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
            );

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            MineField.Changed += (sender, args) => Render();
        }

        private void Render()
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);

            var field = MineField;

            GraphicsDevice.Clear(Color.Gray);

            _spriteBatch.Begin();

            for (int i = 0; i < field.Height; i++)
            {
                for (int j = 0; j < field.Width; j++)
                {
                    var cell = field.Cells[i, j];
                    var pos = new Vector2(j * MineField.CellSize, i * MineField.CellSize);

                    // Drawing the cell's texture
                    _spriteBatch.Draw(_tileSet, pos, _textureResolver.GetSourceRectForCell(cell), Color.White);
                    
                    if (cell.IsWarned && cell.MinesAround > 0)
                    {
                        _spriteBatch.FillRectangle(pos, new Size2(field.CellSize, field.CellSize), Color.Red * 0.5f);
                    }
                    
                    if (cell.Type != FieldCellType.Mine && cell.IsOpen)
                    {
                        // Drawing an outline rectangle
                        _spriteBatch.DrawRectangle(pos, new Size2(MineField.CellSize, MineField.CellSize), Color.Gray);
                    }
                }
            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }
    }
}