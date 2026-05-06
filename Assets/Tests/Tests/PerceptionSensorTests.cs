using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.AI.Perception;

namespace Tests
{
    public class PerceptionSensorTests
    {
        [Test]
        public void MockSensor_ReturnsFixedList()
        {
            var go1 = new GameObject("Target1");
            var go2 = new GameObject("Target2");

            var targets = new List<Transform>
            {
                go1.transform,
                go2.transform
            };

            var sensor  = new MockSensor(targets);
            var results = sensor.Sense(Vector3.zero);

            Assert.AreEqual(2, results.Count);
            Assert.Contains(go1.transform, results);
            Assert.Contains(go2.transform, results);

            Object.DestroyImmediate(go1);
            Object.DestroyImmediate(go2);
        }

        [Test]
        public void MockSensor_NullListDoesNotThrow()
        {
            var sensor = new MockSensor(null);
            Assert.DoesNotThrow(() => sensor.Sense(Vector3.zero));
        }

        [Test]
        public void MockSensor_EmptyListReturnsEmpty()
        {
            var sensor  = new MockSensor(new List<Transform>());
            var results = sensor.Sense(Vector3.zero);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }
    }
}
