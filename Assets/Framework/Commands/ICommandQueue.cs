namespace Framework.Commands
{
    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        void ExecuteAll();

        // =========================================
        // CLEAR
        // Discards all pending commands.
        // Used by StaggerState to cancel queued
        // movement/attack on hit interruption.
        // =========================================
        void Clear();
    }
}