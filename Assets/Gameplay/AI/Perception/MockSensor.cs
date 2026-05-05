using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // MockSensor
    // Returns a fixed list of transforms.
    // No physics — for unit testing and debugging.
    //
    // Usage:
    //   var mock = new MockSensor(new List<Transform> { target });
    //   perceptionSystem.SetSensor(mock);
    // =========================================
    public class MockSensor : IPerceptionSensor
    {
        private readonly List<Transform> _targets;

        public MockSensor(List<Transform> targets)
        {
            _targets = targets ?? new List<Transform>();
        }

        public List<Transform> Sense(Vector3 origin)
        {
            return _targets;
        }
    }
}
