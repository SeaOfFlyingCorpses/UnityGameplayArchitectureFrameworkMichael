using NUnit.Framework;
using Framework.Commands;

namespace Tests
{
    public class TestCommand : ICommand
    {
        public bool Executed { get; private set; }
        public void Execute() => Executed = true;
    }

    public class CommandQueueTests
    {
        [Test]
        public void CommandQueue_ExecutesEnqueuedCommand()
        {
            var queue   = new CommandQueue();
            var command = new TestCommand();

            queue.Enqueue(command);
            queue.ExecuteAll();

            Assert.IsTrue(command.Executed);
        }

        [Test]
        public void CommandQueue_ExecutesMultipleInOrder()
        {
            var queue  = new CommandQueue();
            var order  = new System.Collections.Generic.List<int>();

            queue.Enqueue(new LambdaCommand(() => order.Add(1)));
            queue.Enqueue(new LambdaCommand(() => order.Add(2)));
            queue.Enqueue(new LambdaCommand(() => order.Add(3)));
            queue.ExecuteAll();

            Assert.AreEqual(new[] { 1, 2, 3 }, order.ToArray());
        }

        [Test]
        public void CommandQueue_ClearPreventsExecution()
        {
            var queue   = new CommandQueue();
            var command = new TestCommand();

            queue.Enqueue(command);
            queue.Clear();
            queue.ExecuteAll();

            Assert.IsFalse(command.Executed);
        }

        [Test]
        public void CommandQueue_EmptyExecuteDoesNotThrow()
        {
            var queue = new CommandQueue();
            Assert.DoesNotThrow(() => queue.ExecuteAll());
        }

        // Helper
        private class LambdaCommand : ICommand
        {
            private readonly System.Action _action;
            public LambdaCommand(System.Action action) => _action = action;
            public void Execute() => _action?.Invoke();
        }
    }
}
