namespace Framework.Persistence
{
    // =========================================
    // ISaveable
    // Implement on any MonoBehaviour that needs
    // to persist state across sessions.
    //
    // SaveSystem calls CaptureState() on save
    // and RestoreState() on load.
    //
    // SaveId must be unique per object —
    // use the GameObject name or a hand-written
    // unique string. Duplicate ids overwrite.
    //
    // Example:
    //   public string SaveId => "PlayerHealth";
    //
    //   public object CaptureState()
    //       => new HealthSaveData { value = _health.Value };
    //
    //   public void RestoreState(object state)
    //   {
    //       var data = state as HealthSaveData;
    //       if (data != null) _health.Heal(data.value);
    //   }
    // =========================================
    public interface ISaveable
    {
        // Unique identifier for this saveable
        string SaveId { get; }

        // Return any serializable object representing state
        object CaptureState();

        // Restore from previously captured state
        void RestoreState(object state);
    }
}
