using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper.Framework
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Crops a texture and returns a part of it based on the
        /// given x and y coordinates and with given width and height.
        /// Please note that this function is very slow.
        /// </summary>
        /// <param name="texture"><see cref="Texture2D"/> to crop</param>
        /// <param name="x">x coordinate related to the top left corner of the texture</param>
        /// <param name="y">y coordinate related to the top left corner of the texture</param>
        /// <param name="w">Width of the area to crop</param>
        /// <param name="h">Height of the area to crop</param>
        /// <returns></returns>
        public static Texture2D Crop(this Texture2D texture, int x, int y, int w, int h)
        {
            Texture2D dummy = new Texture2D(texture.GraphicsDevice, w, h);
            Color[] data = new Color[w * h];
            texture.GetData(0, new Rectangle(x, y, w, h), data, 0, w * h);
            dummy.SetData(data);
            return dummy;
        }
    }
}