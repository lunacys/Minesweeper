namespace Minesweeper.Framework
{
    public class PlayerTurnData
    {
        public MineFieldSnapshot MineFieldSnapshot { get; }
        public PlayerTurnSnapshot PlayerTurnSnapshot { get; }
        public string Description { get; }
        public float Time { get; }

        public PlayerTurnData(
            MineFieldSnapshot mineFieldSnapshot,
            PlayerTurnSnapshot playerTurnSnapshot,
            string description,
            float time
        )
        {
            MineFieldSnapshot = mineFieldSnapshot;
            PlayerTurnSnapshot = playerTurnSnapshot;
            Description = description;
            Time = time;
        }

        public override string ToString()
        {
            return Description ?? "No Description";
        }
    }
}