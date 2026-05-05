using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.AI.Perception
{
    // =========================================
    // IPerceptionSensor
    // Contract for detecting targets in the world.
    //
    // Implement to swap sensing strategy without
    // touching PerceptionSystem or any AI logic.
    //
    // Built-in implementations:
    //   OverlapSphereSensor — Physics.OverlapSphere
    //   TriggerSensor       — Unity trigger collider
    //   MockSensor          — fixed list for testing
    //
    // Usage:
    //   perceptionSystem.SetSensor(new OverlapSphereSensor(...));
    //   perceptionSystem.SetSensor(new MockSensor(myTestTargets));
    // =========================================
    public interface IPerceptionSensor
    {
        // Returns all detected transforms this frame
        // Origin — the sensing agent's position
        List<Transform> Sense(Vector3 origin);
    }
}