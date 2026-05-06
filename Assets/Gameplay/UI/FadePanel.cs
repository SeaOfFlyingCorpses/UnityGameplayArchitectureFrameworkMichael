using System.Collections;
using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Gameplay.UI
{
    // =========================================
    // FadePanel
    // Fades a CanvasGroup in and out when a
    // scene transition starts.
    //
    // Setup:
    //   1. Create a Canvas (Screen Space Overlay)
    //   2. Add a full-screen black UI Image child
    //   3. Add CanvasGroup to the Canvas
    //   4. Add this component to the Canvas
    //   5. Drag the CanvasGroup into the slot
    //   6. Set fadeDuration to match SceneLoader
    //      transitionDelay (e.g. both 0.5f)
    //
    // Subscribes to SceneLoadingEvent via EventBus.
    // Zero coupling to SceneLoader directly.
    // =========================================
    public class FadePanel : MonoBehaviour
    {
        [Header("References")]
        public CanvasGroup canvasGroup;

        [Header("Settings")]
        [Tooltip("Seconds to fade in or out")]
        public float fadeDuration = 0.5f;

        private Coroutine _current;

        private void Awake()
        {
            // Start fully transparent
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 0f;
                canvasGroup.blocksRaycasts = false;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SceneLoadingEvent>(OnSceneLoading);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SceneLoadingEvent>(OnSceneLoading);
        }

        private void OnSceneLoading(SceneLoadingEvent e)
        {
            if (_current != null)
                StopCoroutine(_current);

            _current = StartCoroutine(
                e.IsLoading ? FadeIn() : FadeOut()
            );
        }

        // =========================================
        // FADE IN — black screen appears
        // =========================================
        private IEnumerator FadeIn()
        {
            canvasGroup.blocksRaycasts = true;

            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        // =========================================
        // FADE OUT — black screen disappears
        // =========================================
        private IEnumerator FadeOut()
        {
            float t = fadeDuration;

            while (t > 0f)
            {
                t -= Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha          = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        // =========================================
        // MANUAL CONTROL — call from anywhere
        // =========================================
        public void FadeToBlack()   => OnSceneLoading(new SceneLoadingEvent(true));
        public void FadeToClear()   => OnSceneLoading(new SceneLoadingEvent(false));
    }
}
