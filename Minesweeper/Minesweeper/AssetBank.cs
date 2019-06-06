using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Minesweeper
{
    public static class AssetBank
    {
        public static SpriteFont MainFont { get; private set; }

        public static Texture2D MineCellTexture { get; private set; }
        public static Texture2D FlagCellTexture { get; private set; }
        public static Texture2D ClosedCellTexture { get; private set; }

        public static Texture2D[] Cells { get; private set; }

        public static void Load(ContentManager content)
        {
            MainFont = content.Load<SpriteFont>(Path.Combine("Fonts", "MainFont"));

            MineCellTexture = content.Load<Texture2D>(Path.Combine("Images", "Cell_Mine"));
            FlagCellTexture = content.Load<Texture2D>(Path.Combine("Images", "Cell_Flag"));
            ClosedCellTexture = content.Load<Texture2D>(Path.Combine("Images", "Cell_Closed"));

            // 9 cells: from 0 to 8 where 0 - empty cell
            Cells = new Texture2D[9];
            for (int i = 0; i < 9; i++)
            {
                Cells[i] = content.Load<Texture2D>(Path.Combine("Images", $"Cell_" + i));
            }
        }
    }
}