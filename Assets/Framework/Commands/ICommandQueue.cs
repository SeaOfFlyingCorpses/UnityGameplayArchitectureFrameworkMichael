namespace Framework.Commands
{
    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        void ExecuteAll();
    }
}