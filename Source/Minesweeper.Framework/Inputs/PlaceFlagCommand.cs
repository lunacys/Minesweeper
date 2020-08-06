using MonoGame.Extended;

namespace Minesweeper.Framework.Inputs
{
    public class PlaceFlagCommand : MineFieldCommandBase
    {
        public PlaceFlagCommand(MineField mineField, OrthographicCamera camera, PlayerTurnsContainer container)
            : base(mineField, camera, container)
        { }

        public override PlayerTurnSnapshot Execute(float time)
        {
            var mousePos = Camera.ScreenToWorld(InputManager.MousePosition);
            var cellSize = MineField.CellSize;

            var fieldSnapshot = MineField.CreateSnapshot();
            var cmd = MineField.FlagAt((int) mousePos.X / cellSize, (int) mousePos.Y / cellSize);

            if (cmd != null)
            {
                OnPlayerTurn(fieldSnapshot, cmd, time);
            }

            return cmd;
        }
    }
}