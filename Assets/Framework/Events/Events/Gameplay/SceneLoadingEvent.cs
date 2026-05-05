namespace Framework.Events.Events.Gameplay
{
    // =========================================
    // SceneLoadingEvent
    // Fired by SceneLoader when a scene transition
    // starts. Subscribe to show a loading screen
    // or fade transition.
    //
    // IsLoading = true  — transition starting
    // IsLoading = false — (future: async loading done)
    //
    // Usage:
    //   EventBus.Subscribe<SceneLoadingEvent>(OnLoading);
    //
    //   private void OnLoading(SceneLoadingEvent e)
    //   {
    //       fadePanel.SetActive(e.IsLoading);
    //   }
    // =========================================
    public struct SceneLoadingEvent
    {
        public bool IsLoading;

        public SceneLoadingEvent(bool isLoading)
        {
            IsLoading = isLoading;
        }
    }
}
