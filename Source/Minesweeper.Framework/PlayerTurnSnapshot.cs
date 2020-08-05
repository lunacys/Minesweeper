using Microsoft.Xna.Framework;

namespace Minesweeper.Framework
{
    public class PlayerTurnSnapshot
    {
        public Point Position { get; }
        public FieldCell NewCellState { get; }
        public FieldCell OldCellState { get; }

        public PlayerTurnSnapshot(Point position, FieldCell newCellState, FieldCell oldCellState)
        {
            Position = position;
            NewCellState = newCellState;
            OldCellState = oldCellState;
        }

        public string CompareStates()
        {
            var changeSet = "";

            if (OldCellState.IsFlagged != NewCellState.IsFlagged)
            {
                var newFlagged = NewCellState.IsFlagged ? "Flagged" : "Not Flagged";
                var oldFlagged = OldCellState.IsFlagged ? "Flagged" : "Not Flagged";
                
                changeSet += $"{oldFlagged}->{newFlagged}";
            }

            if (OldCellState.IsOpen != NewCellState.IsOpen)
            {
                changeSet += ", ";
                changeSet += "Closed->Opened";
            }

            return changeSet;
        }
    }
}