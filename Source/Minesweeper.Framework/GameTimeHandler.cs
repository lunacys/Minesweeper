using Microsoft.Xna.Framework;
using Minesweeper.Framework.GameStateManagement;

namespace Minesweeper.Framework
{
    public class GameTimeHandler
    {
        public GameStateManager GameStateManager { get; }
        public float SecondsElapsed { get; private set; }

        public GameTimeHandler(GameStateManager gameStateManager)
        {
            GameStateManager = gameStateManager;
            SecondsElapsed = 0;
            
            GameStateManager.StateChange += GameStateManagerOnStateChange;
        }

        private void GameStateManagerOnStateChange(object sender, GameState e)
        {
            if (e == GameState.NewGame)
                SecondsElapsed = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (GameStateManager.CurrentState == GameState.Playing)
                SecondsElapsed += (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}