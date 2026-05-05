using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Core;

namespace Gameplay.Systems
{
    // =========================================
    // SceneLoader
    // Single point for all scene transitions.
    // Never call SceneManager.LoadScene directly.
    // Always go through SceneLoader.
    //
    // Guarantees cleanup order:
    //   1. Pause game (stops AI, physics)
    //   2. Publish SceneLoadingEvent (UI can fade)
    //   3. Clear EventBus
    //   4. Clear ServiceLocator
    //   5. Load scene
    //   (Services re-register in new scene Awake)
    //
    // Place on _GameSystems GameObject.
    // Register as service so anything can call it.
    //
    // Usage:
    //   ServiceLocator.Get<SceneLoader>()
    //       .Load("GameScene");
    //
    //   ServiceLocator.Get<SceneLoader>()
    //       .Reload(); // reload current scene
    // =========================================
    public class SceneLoader : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Seconds to wait before loading — " +
                 "gives UI time to fade out")]
        public float transitionDelay = 0f;

        private bool _isLoading;

        private void Awake()
        {
            ServiceLocator.Register<SceneLoader>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SceneLoader>();
        }

        // =========================================
        // LOAD BY NAME
        // =========================================
        public void Load(string sceneName)
        {
            if (_isLoading)
                return;

            StartCoroutine(LoadRoutine(sceneName));
        }

        // =========================================
        // LOAD BY INDEX
        // =========================================
        public void Load(int sceneIndex)
        {
            if (_isLoading)
                return;

            StartCoroutine(LoadRoutine(sceneIndex));
        }

        // =========================================
        // RELOAD CURRENT SCENE
        // =========================================
        public void Reload()
        {
            Load(SceneManager.GetActiveScene().name);
        }

        // =========================================
        // LOAD NEXT SCENE IN BUILD ORDER
        // =========================================
        public void LoadNext()
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;

            if (next >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogWarning("[SceneLoader] No next scene in build settings.");
                return;
            }

            Load(next);
        }

        // =========================================
        // LOAD ROUTINE — guaranteed cleanup order
        // =========================================
        private IEnumerator LoadRoutine(string sceneName)
        {
            _isLoading = true;

            yield return RunTransition();

            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator LoadRoutine(int sceneIndex)
        {
            _isLoading = true;

            yield return RunTransition();

            SceneManager.LoadScene(sceneIndex);
        }

        private IEnumerator RunTransition()
        {
            // Step 1 — pause game so AI and physics stop
            var pauseSystem = ServiceLocator.Get<PauseSystem>();
            pauseSystem?.SetPaused(true);

            // Step 2 — publish loading event so UI can fade
            EventBus.Publish(new SceneLoadingEvent(true));

            // Step 3 — wait for transition delay (fade out)
            if (transitionDelay > 0f)
                yield return new WaitForSecondsRealtime(transitionDelay);

            // Step 4 — clear registries before new scene loads
            EventBus.Clear();
            ServiceLocator.Clear();

            // timeScale reset — new scene starts unpaused
            Time.timeScale = 1f;
        }
    }
}
