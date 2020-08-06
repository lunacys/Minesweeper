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

        public abstract PlayerTurnSnapshot Execute();
        
        public virtual void Undo()
        {
            Container.UndoTurn();
        }

        public void OnPlayerTurn(MineFieldSnapshot mineFieldSnapshot, PlayerTurnSnapshot playerTurnSnapshot)
        {
            Container.AddTurn(mineFieldSnapshot, playerTurnSnapshot);
        }
    }
}