namespace Minesweeper.Framework.Inputs
{
    public interface ICommand
    {
        PlayerTurnSnapshot Execute(float time);
        void Undo();
    }

    public class NullCommand : ICommand
    {
        public PlayerTurnSnapshot Execute(float time)
        {
            return null;
        }

        public void Undo() { }
    }
}