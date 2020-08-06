using System;

namespace Minesweeper.Framework.GameStateManagement
{
    public class GameStateManager
    {
        private GameState _currentState;
        public GameState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                StateChange?.Invoke(this, _currentState);
            }
        }

        public event EventHandler<GameState> StateChange;

        public bool HasLost => CurrentState == GameState.Lost;
        public bool HasWon => CurrentState == GameState.Won;
        public bool IsPlaying => CurrentState == GameState.Playing;
        public bool IsNewGame => CurrentState == GameState.NewGame;

        public GameStateManager()
        {
            _currentState = GameState.NewGame;
        }
    }
}