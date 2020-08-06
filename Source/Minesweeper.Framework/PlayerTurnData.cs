using Minesweeper.Framework.GameStateManagement;

namespace Minesweeper.Framework
{
    public class PlayerTurnData
    {
        public MineFieldSnapshot MineFieldSnapshot { get; }
        public PlayerTurnSnapshot PlayerTurnSnapshot { get; }
        public string Description { get; }
        public float Time { get; }
        public GameState GameState { get; }

        public PlayerTurnData(
            MineFieldSnapshot mineFieldSnapshot,
            PlayerTurnSnapshot playerTurnSnapshot,
            string description,
            float time,
            GameState gameState
        )
        {
            MineFieldSnapshot = mineFieldSnapshot;
            PlayerTurnSnapshot = playerTurnSnapshot;
            Description = description;
            Time = time;
            GameState = gameState;
        }

        public override string ToString()
        {
            return Description ?? "No Description";
        }
    }
}