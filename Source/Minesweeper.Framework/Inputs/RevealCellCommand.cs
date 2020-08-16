using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended;

namespace Minesweeper.Framework.Inputs
{
    public class RevealCellCommand : MineFieldCommandBase
    {
        public RevealCellCommand(MineField mineField, OrthographicCamera camera, PlayerTurnsContainer container)
            : base(mineField, camera, container)
        { }
        
        public override void Execute(float time)
        {
            var mousePos = Camera.ScreenToWorld(InputManager.MousePosition);
            var cellSize = MineField.CellSize;
            
            var fieldSnapshot = MineField.CreateSnapshot();
            MineField.RevealAt((int) mousePos.X / cellSize, (int) mousePos.Y / cellSize);

            /*if (cmd != null)
            {
                OnPlayerTurn(fieldSnapshot, cmd.FirstOrDefault(), time);    
            }*/
        }
    }
}