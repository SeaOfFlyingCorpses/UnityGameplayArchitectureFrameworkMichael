using System.Collections.Generic;

namespace Framework.Commands
{
    public class CommandQueue : ICommandQueue
    {
        private readonly Queue<ICommand> _queue = new();

        public void Enqueue(ICommand command)
        {
            _queue.Enqueue(command);
        }

        public void ExecuteAll()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue().Execute();
            }
        }

        public void Clear()
        {
            _queue.Clear(); // FIXED
        }
    }
}