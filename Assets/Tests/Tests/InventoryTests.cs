using NUnit.Framework;
using Framework.Items;

namespace Tests
{
    // Simple test item — no ScriptableObject needed
    public class TestItem : IItem
    {
        public string Id          { get; }
        public string DisplayName { get; }
        public int    StackSize   { get; }

        public TestItem(string id, int stackSize = 1)
        {
            Id          = id;
            DisplayName = id;
            StackSize   = stackSize;
        }
    }

    public class InventoryTests
    {
        private Inventory _inv;

        [SetUp]
        public void SetUp()
        {
            _inv = new Inventory(capacity: 10, ownerId: "Test");
        }

        [Test]
        public void Inventory_AddItem()
        {
            var item   = new TestItem("sword");
            bool added = _inv.Add(item, 1);

            Assert.IsTrue(added);
            Assert.IsTrue(_inv.Has("sword"));
        }

        [Test]
        public void Inventory_GetCount()
        {
            var item = new TestItem("coin", stackSize: 99);
            _inv.Add(item, 5);

            Assert.AreEqual(5, _inv.GetCount("coin"));
        }

        [Test]
        public void Inventory_RemoveItem()
        {
            var item = new TestItem("coin", stackSize: 99);
            _inv.Add(item, 5);
            _inv.Remove("coin", 3);

            Assert.AreEqual(2, _inv.GetCount("coin"));
        }

        [Test]
        public void Inventory_RemoveAll_RemovesSlot()
        {
            var item = new TestItem("coin", stackSize: 99);
            _inv.Add(item, 5);
            _inv.Remove("coin", 5);

            Assert.IsFalse(_inv.Has("coin"));
            Assert.AreEqual(0, _inv.Slots.Count);
        }

        [Test]
        public void Inventory_StackableItemsStack()
        {
            var item = new TestItem("potion", stackSize: 99);
            _inv.Add(item, 3);
            _inv.Add(item, 2);

            Assert.AreEqual(5,  _inv.GetCount("potion"));
            Assert.AreEqual(1,  _inv.Slots.Count); // one slot
        }

        [Test]
        public void Inventory_NonStackableCreatesNewSlot()
        {
            var sword = new TestItem("sword", stackSize: 1);
            _inv.Add(sword, 1);
            _inv.Add(sword, 1);

            Assert.AreEqual(2, _inv.Slots.Count);
        }

        [Test]
        public void Inventory_CapacityPreventsAdd()
        {
            var inv  = new Inventory(capacity: 2);
            var item = new TestItem("sword", stackSize: 1);

            inv.Add(item, 1);
            inv.Add(item, 1);
            bool added = inv.Add(item, 1); // over capacity

            Assert.IsFalse(added);
        }

        [Test]
        public void Inventory_HasWithAmount()
        {
            var item = new TestItem("arrow", stackSize: 99);
            _inv.Add(item, 10);

            Assert.IsTrue(_inv.Has("arrow", 10));
            Assert.IsFalse(_inv.Has("arrow", 11));
        }

        [Test]
        public void Inventory_RemoveMoreThanExistsReturnsFalse()
        {
            var item = new TestItem("coin", stackSize: 99);
            _inv.Add(item, 3);

            bool removed = _inv.Remove("coin", 5);
            Assert.IsFalse(removed);
            Assert.AreEqual(3, _inv.GetCount("coin"));
        }

        [Test]
        public void Inventory_GetCountReturnsZeroIfMissing()
        {
            Assert.AreEqual(0, _inv.GetCount("nonexistent"));
        }
    }
}
