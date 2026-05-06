using NUnit.Framework;
using Framework.Events;

namespace Tests
{
    public struct TestEvent
    {
        public int Value;
        public TestEvent(int v) { Value = v; }
    }

    public struct OtherEvent
    {
        public string Message;
        public OtherEvent(string m) { Message = m; }
    }

    public class EventBusTests
    {
        [SetUp]
        public void SetUp()
        {
            EventBus.Clear();
        }

        [Test]
        public void EventBus_SubscriberReceivesEvent()
        {
            int received = 0;
            EventBus.Subscribe<TestEvent>(e => received = e.Value);
            EventBus.Publish(new TestEvent(42));

            Assert.AreEqual(42, received);
        }

        [Test]
        public void EventBus_MultipleSubscribersAllReceive()
        {
            int count = 0;
            EventBus.Subscribe<TestEvent>(_ => count++);
            EventBus.Subscribe<TestEvent>(_ => count++);
            EventBus.Publish(new TestEvent(1));

            Assert.AreEqual(2, count);
        }

        [Test]
        public void EventBus_UnsubscribedDoesNotReceive()
        {
            int count = 0;
            System.Action<TestEvent> handler = _ => count++;

            EventBus.Subscribe<TestEvent>(handler);
            EventBus.Unsubscribe<TestEvent>(handler);
            EventBus.Publish(new TestEvent(1));

            Assert.AreEqual(0, count);
        }

        [Test]
        public void EventBus_ClearRemovesAllListeners()
        {
            int count = 0;
            EventBus.Subscribe<TestEvent>(_ => count++);
            EventBus.Clear();
            EventBus.Publish(new TestEvent(1));

            Assert.AreEqual(0, count);
        }

        [Test]
        public void EventBus_DifferentTypesDoNotInterfere()
        {
            int   testCount  = 0;
            int   otherCount = 0;

            EventBus.Subscribe<TestEvent>(_ => testCount++);
            EventBus.Subscribe<OtherEvent>(_ => otherCount++);

            EventBus.Publish(new TestEvent(1));

            Assert.AreEqual(1, testCount);
            Assert.AreEqual(0, otherCount);
        }

        [Test]
        public void EventBus_PublishWithNoSubscribersDoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                EventBus.Publish(new TestEvent(1)));
        }
    }
}
