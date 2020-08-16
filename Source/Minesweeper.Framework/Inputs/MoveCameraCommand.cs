using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Minesweeper.Framework.Inputs
{
    public class MoveCameraCommand : ICommand
    {
        private OrthographicCamera _camera;
        private Vector2 _oldCameraPos;
        
        public MoveCameraCommand(OrthographicCamera camera)
        {
            _camera = camera;
            _oldCameraPos = camera.Position;
        }
        
        public void Execute(float time)
        {
            _camera.Move(-InputManager.MouseVelocity / _camera.Zoom);
        }

        public void Undo()
        {
            _camera.Move(_oldCameraPos);
        }
    }
}