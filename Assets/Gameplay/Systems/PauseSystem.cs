using UnityEngine;
using Framework.Events;
using Framework.Events.Events.Gameplay;
using Framework.Input;
using Framework.Core;
using Gameplay.Input;

namespace Gameplay.Systems
{
    public class PauseSystem : MonoBehaviour
    {
        [Header("References")]
        public InputHandler inputHandler;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register<PauseSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<PauseSystem>();
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (inputHandler == null)
                return;

            if (inputHandler.State.PausePressed)
                Toggle();
        }

        public void Toggle()
        {
            SetPaused(!IsPaused);
        }

        public void SetPaused(bool paused)
        {
            IsPaused       = paused;
            Time.timeScale = IsPaused ? 0f : 1f;

            EventBus.Publish(new GamePausedEvent(IsPaused));
        }
    }
}