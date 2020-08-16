using System.Collections.Generic;
using MonoGame.Extended;

namespace Minesweeper.Framework.Inputs
{
    public abstract class MineFieldCommandBase : ICommand
    {
        public MineField MineField { get; }
        public OrthographicCamera Camera { get; }
        public PlayerTurnsContainer Container { get; }

        protected MineFieldCommandBase(MineField mineField, OrthographicCamera camera, PlayerTurnsContainer container)
        {
            MineField = mineField;
            Camera = camera;
            Container = container;
        }

        public abstract void Execute(float time);
        
        public virtual void Undo()
        {
            Container.UndoTurn();
        }

        public void OnPlayerTurn(MineFieldSnapshot mineFieldSnapshot, PlayerTurnSnapshot playerTurnSnapshot, float time)
        {
            Container.AddTurn(mineFieldSnapshot, playerTurnSnapshot, GetDescriptionForTurn(playerTurnSnapshot), time);
        }

        private string GetDescriptionForTurn(PlayerTurnSnapshot snapshot)
        {
            if (snapshot.OldCellState.IsFlagged != snapshot.NewCellState.IsFlagged)
            {
                var newFlagged = snapshot.NewCellState.IsFlagged ? "Flagged" : "Not Flagged";
                var oldFlagged = snapshot.OldCellState.IsFlagged ? "Flagged" : "Not Flagged";
                
                return $"{oldFlagged}->{newFlagged}";
            }

            if (snapshot.OldCellState.IsOpen != snapshot.NewCellState.IsOpen)
            {
                if (snapshot.NewCellState.IsMine)
                    return "Caught a mine :(";
                
                return "Closed->Opened";
            }

            return "Unknown";
        }
    }
}