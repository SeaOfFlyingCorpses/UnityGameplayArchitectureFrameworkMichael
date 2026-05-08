using System.Collections.Generic;
using Unity.Profiling;
using Framework.Core;

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
            using var marker = FrameworkProfiler.CommandExecute.Auto();

            while (_queue.Count > 0)
                _queue.Dequeue().Execute();
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}