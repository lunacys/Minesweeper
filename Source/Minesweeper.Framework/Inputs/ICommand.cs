using System.Collections.Generic;

namespace Minesweeper.Framework.Inputs
{
    public interface ICommand
    {
        void Execute(float time);
        void Undo();
    }

    public class NullCommand : ICommand
    {
        public void Execute(float time)
        { }

        public void Undo() { }
    }
}