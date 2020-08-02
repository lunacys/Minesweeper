using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Minesweeper.Framework.Screens;
using MonoGame.Extended.Screens;

namespace Minesweeper.Framework
{
    public class GameRoot : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private ScreenManager _screenManager;

        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.PreferMultiSampling = true;
            
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            _screenManager = new ScreenManager();
            
            
            Components.Add(_screenManager);
        }

        protected override void Initialize()
        {
            base.Initialize();
            var ga = new MineFieldScreen(this);
            _screenManager.LoadScreen(ga);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
