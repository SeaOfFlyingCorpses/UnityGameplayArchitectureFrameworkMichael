using NUnit.Framework;
using Gameplay.Systems.Health;
using Framework.Systems.Health;

namespace Tests
{
    // =========================================
    // HealthTests
    // Tests all IHealth implementations.
    // Run via: Window → General → Test Runner
    //          → EditMode → Run All
    // =========================================
    public class HealthTests
    {
        // =========================================
        // STANDARD HEALTH
        // =========================================
        [Test]
        public void Health_StartsAtMaxValue()
        {
            var h = new Health(100);
            Assert.AreEqual(100, h.Value);
            Assert.AreEqual(100, h.MaxValue);
        }

        [Test]
        public void Health_DamageReducesValue()
        {
            var h = new Health(100);
            h.Damage(30);
            Assert.AreEqual(70, h.Value);
        }

        [Test]
        public void Health_CannotGoBelowZero()
        {
            var h = new Health(100);
            h.Damage(999);
            Assert.AreEqual(0, h.Value);
            Assert.IsTrue(h.IsDead);
        }

        [Test]
        public void Health_HealRestoresValue()
        {
            var h = new Health(100);
            h.Damage(50);
            h.Heal(20);
            Assert.AreEqual(70, h.Value);
        }

        [Test]
        public void Health_HealCannotExceedMax()
        {
            var h = new Health(100);
            h.Heal(999);
            Assert.AreEqual(100, h.Value);
        }

        [Test]
        public void Health_FiresOnChangedEvent()
        {
            var h      = new Health(100);
            int fired  = 0;
            int lastVal = 0;

            h.OnChanged += v => { fired++; lastVal = v; };
            h.Damage(30);

            Assert.AreEqual(1,  fired);
            Assert.AreEqual(70, lastVal);
        }

        [Test]
        public void Health_FiresOnDeathEvent()
        {
            var h     = new Health(100);
            bool dead = false;

            h.OnDeath += () => dead = true;
            h.Damage(100);

            Assert.IsTrue(dead);
        }

        [Test]
        public void Health_DoesNotFireDeathTwice()
        {
            var h     = new Health(100);
            int count = 0;

            h.OnDeath += () => count++;
            h.Damage(100);
            h.Damage(100); // already dead

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Health_ResetRestoresMax()
        {
            var h = new Health(100);
            h.Damage(80);
            h.Reset();
            Assert.AreEqual(100, h.Value);
        }

        // =========================================
        // SHIELDED HEALTH
        // =========================================
        [Test]
        public void ShieldedHealth_ShieldAbsorbsFirst()
        {
            var h = new ShieldedHealth(100, 50);
            h.Damage(30);

            Assert.AreEqual(20, h.Shield);  // 50 - 30
            Assert.AreEqual(100, h.Value);  // HP untouched
        }

        [Test]
        public void ShieldedHealth_ExcessDamageHitsHP()
        {
            var h = new ShieldedHealth(100, 50);
            h.Damage(70); // 50 to shield, 20 to HP

            Assert.AreEqual(0,  h.Shield);
            Assert.AreEqual(80, h.Value);
        }

        [Test]
        public void ShieldedHealth_RestoreShield()
        {
            var h = new ShieldedHealth(100, 50);
            h.Damage(50); // deplete shield
            h.RestoreShield(30);

            Assert.AreEqual(30, h.Shield);
        }

        // =========================================
        // ARMOURED HEALTH
        // =========================================
        [Test]
        public void ArmouredHealth_FlatReductionApplied()
        {
            var h = new ArmouredHealth(100, armour: 10, armourPct: 0f);
            h.Damage(30); // 30 - 10 flat = 20

            Assert.AreEqual(80, h.Value);
        }

        [Test]
        public void ArmouredHealth_PctReductionApplied()
        {
            var h = new ArmouredHealth(100, armour: 0, armourPct: 0.5f);
            h.Damage(40); // 40 * 0.5 = 20

            Assert.AreEqual(80, h.Value);
        }

        [Test]
        public void ArmouredHealth_MinimumOneDamage()
        {
            var h = new ArmouredHealth(100, armour: 999, armourPct: 0f);
            h.Damage(5); // still at least 1 gets through

            Assert.AreEqual(99, h.Value);
        }

        // =========================================
        // SEGMENTED HEALTH
        // =========================================
        [Test]
        public void SegmentedHealth_StartsAtFullSegments()
        {
            var h = new SegmentedHealth(100, 5);
            Assert.AreEqual(5, h.CurrentSegment);
            Assert.AreEqual(5, h.TotalSegments);
        }

        [Test]
        public void SegmentedHealth_FiresOnSegmentBroken()
        {
            var h       = new SegmentedHealth(100, 5);
            int broken  = 0;

            h.OnSegmentBroken += _ => broken++;
            h.Damage(20); // one segment = 20 HP

            Assert.AreEqual(1, broken);
        }

        // =========================================
        // INVINCIBLE HEALTH
        // =========================================
        [Test]
        public void InvincibleHealth_NeverTakesDamage()
        {
            var h = new InvincibleHealth(100);
            h.Damage(9999);

            Assert.AreEqual(100, h.Value);
            Assert.IsFalse(h.IsDead);
        }

        // =========================================
        // COMPOSITE HEALTH
        // =========================================
        [Test]
        public void CompositeHealth_DamageFlowsThroughLayers()
        {
            // ShieldedHealth(maxHp:50, shield:50)
            // Plain Health(100) as inner HP layer
            var shield    = new ShieldedHealth(50, 50);
            var hp        = new Health(100);
            var composite = new CompositeHealth()
                .Add(shield)
                .Add(hp);

            // Damage 70:
            // ShieldedHealth absorbs shield first (50), then HP (20)
            // ShieldedHealth before.Value = 50, after.Value = 30
            // absorbed by shield layer = 50 - 30 = 20
            // remaining = 70 - 20 = 50 flows to hp layer
            // hp = 100 - 50 = 50
            composite.Damage(70);

            Assert.AreEqual(0,  shield.Shield);  // shield depleted
            Assert.AreEqual(30, shield.Value);    // shield HP took 20
            Assert.AreEqual(50, hp.Value);        // inner HP took 50
        }

        [Test]
        public void CompositeHealth_IsDead_WhenLastLayerDies()
        {
            // Use simple Health layers to avoid ShieldedHealth complexity
            var outer     = new Health(50);
            var inner     = new Health(10);
            var composite = new CompositeHealth()
                .Add(outer)
                .Add(inner);

            composite.Damage(999);

            Assert.IsTrue(composite.IsDead);
        }
    }
}