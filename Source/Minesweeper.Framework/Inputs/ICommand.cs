namespace Minesweeper.Framework.Inputs
{
    public interface ICommand
    {
        PlayerTurnSnapshot Execute();
        void Undo();
    }

    public class NullCommand : ICommand
    {
        public PlayerTurnSnapshot Execute()
        {
            return null;
        }

        public void Undo() { }
    }
}