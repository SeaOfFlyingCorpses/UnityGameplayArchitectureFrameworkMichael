using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Events;
using Framework.Core;

namespace Framework.Core
{
    // =========================================
    // SceneLifecycleManager
    // Single guaranteed cleanup point for all
    // static framework registries.
    //
    // Place on the _GameSystems GameObject.
    // Must be in every scene that uses the framework.
    //
    // On scene unload:
    //   - EventBus.Clear()       — removes stale listeners
    //   - ServiceLocator.Clear() — removes stale services
    //
    // On scene load:
    //   Services re-register themselves via their
    //   own Awake() calls automatically.
    //   EventBus listeners re-subscribe via their
    //   own Start() or OnEnable() calls.
    // =========================================
    public class SceneLifecycleManager : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            EventBus.Clear();
            ServiceLocator.Clear();

            Debug.Log($"[SceneLifecycleManager] Scene '{scene.name}' unloaded — " +
                      $"EventBus and ServiceLocator cleared.");
        }
    }
}
