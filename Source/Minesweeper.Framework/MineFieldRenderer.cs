using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Minesweeper.Framework
{
    public class MineFieldRenderer
    {
        private class TextureResolver
        {
            private Texture2D[] _textures;

            public TextureResolver(Texture2D tileSet, int tileWidth, int tileHeight)
            {
                _textures = new Texture2D[4 * 3];

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        _textures[i + 4 * j] = tileSet.Crop(i * tileWidth, j * tileHeight, tileWidth, tileHeight);
                    }
                }
            }

            public Texture2D ResolveForCell(FieldCell cell)
            {
                if (cell.IsFlagged)
                    return _textures[4 * 3 - 2];
                if (!cell.IsOpen)
                    return _textures[4 * 3 - 3];
                if (cell.IsMine)
                    return _textures[4 * 3 - 1];

                return _textures[cell.MinesAround];
            }
        }

        public RenderTarget2D RenderTarget { get; }

        public MineField MineField { get; }

        public GraphicsDevice GraphicsDevice { get; }

        private readonly SpriteBatch _spriteBatch;

        private TextureResolver _textureResolver;

        public MineFieldRenderer(MineField mineField, GraphicsDevice graphicsDevice, Texture2D tileSet)
        {
            MineField = mineField;
            GraphicsDevice = graphicsDevice;
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
                    var pos = new Vector2(j * 64, i * 64);

                    _spriteBatch.Draw(_textureResolver.ResolveForCell(cell), pos, null, Color.White);
                    if (cell.Type != FieldCellType.Mine && cell.IsOpen)
                    {
                        _spriteBatch.DrawRectangle(pos, new Size2(64, 64), Color.Gray);
                    }
                }
            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }
    }
}