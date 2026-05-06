using NUnit.Framework;
using Framework.Core;

namespace Tests
{
    // Test services
    public class FakeService  { public int Value = 1; }
    public class OtherService { public int Value = 2; }

    public class ServiceLocatorTests
    {
        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
        }

        [Test]
        public void ServiceLocator_RegisterAndGet()
        {
            var svc = new FakeService();
            ServiceLocator.Register<FakeService>(svc);

            var result = ServiceLocator.Get<FakeService>();
            Assert.AreEqual(svc, result);
        }

        [Test]
        public void ServiceLocator_GetReturnsNullIfNotRegistered()
        {
            var result = ServiceLocator.Get<FakeService>();
            Assert.IsNull(result);
        }

        [Test]
        public void ServiceLocator_UnregisterRemovesService()
        {
            var svc = new FakeService();
            ServiceLocator.Register<FakeService>(svc);
            ServiceLocator.Unregister<FakeService>();

            Assert.IsNull(ServiceLocator.Get<FakeService>());
        }

        [Test]
        public void ServiceLocator_HasReturnsTrueWhenRegistered()
        {
            ServiceLocator.Register<FakeService>(new FakeService());
            Assert.IsTrue(ServiceLocator.Has<FakeService>());
        }

        [Test]
        public void ServiceLocator_HasReturnsFalseWhenNotRegistered()
        {
            Assert.IsFalse(ServiceLocator.Has<FakeService>());
        }

        [Test]
        public void ServiceLocator_ClearRemovesAll()
        {
            ServiceLocator.Register<FakeService>(new FakeService());
            ServiceLocator.Register<OtherService>(new OtherService());
            ServiceLocator.Clear();

            Assert.IsNull(ServiceLocator.Get<FakeService>());
            Assert.IsNull(ServiceLocator.Get<OtherService>());
        }

        [Test]
        public void ServiceLocator_RegisterNullDoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                ServiceLocator.Register<FakeService>(null));
        }

        [Test]
        public void ServiceLocator_SecondRegisterOverwritesFirst()
        {
            var first  = new FakeService { Value = 1 };
            var second = new FakeService { Value = 2 };

            ServiceLocator.Register<FakeService>(first);
            ServiceLocator.Register<FakeService>(second);

            Assert.AreEqual(2, ServiceLocator.Get<FakeService>().Value);
        }
    }
}
