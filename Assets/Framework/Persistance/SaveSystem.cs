using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core;
using Framework.Events;
using Framework.Events.Events.Gameplay;

namespace Framework.Persistence
{
    // =========================================
    // SaveSystem
    // Collects all registered ISaveable objects,
    // serializes their state to JSON and writes
    // to disk. Restores on load.
    //
    // Place on _GameSystems GameObject.
    //
    // Usage:
    //   ServiceLocator.Get<SaveSystem>().Save();
    //   ServiceLocator.Get<SaveSystem>().Load();
    //
    // Any system that needs to save:
    //   1. Implement ISaveable
    //   2. Register: ServiceLocator.Get<SaveSystem>()
    //                    ?.Register(this);
    //   3. Unregister on destroy: Unregister(this)
    //
    // Save file location:
    //   Application.persistentDataPath/save.json
    // =========================================
    public class SaveSystem : MonoBehaviour
    {
        private readonly Dictionary<string, ISaveable> _saveables = new();

        [Header("Settings")]
        public string saveFileName = "save.json";

        private string SavePath =>
            System.IO.Path.Combine(Application.persistentDataPath, saveFileName);

        private void Awake()
        {
            ServiceLocator.Register<SaveSystem>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<SaveSystem>();
        }

        // =========================================
        // REGISTER / UNREGISTER
        // =========================================
        public void Register(ISaveable saveable)
        {
            if (saveable == null) return;

            if (_saveables.ContainsKey(saveable.SaveId))
            {
                Debug.LogWarning(
                    $"[SaveSystem] Duplicate SaveId '{saveable.SaveId}' — overwriting.");
            }

            _saveables[saveable.SaveId] = saveable;
        }

        public void Unregister(ISaveable saveable)
        {
            if (saveable == null) return;
            _saveables.Remove(saveable.SaveId);
        }

        // =========================================
        // SAVE
        // =========================================
        public void Save()
        {
            var data = new SaveFile();

            foreach (var kvp in _saveables)
            {
                try
                {
                    var state       = kvp.Value.CaptureState();
                    var json        = JsonUtility.ToJson(state);
                    data.entries.Add(new SaveEntry
                    {
                        id       = kvp.Key,
                        typeName = state.GetType().AssemblyQualifiedName,
                        json     = json
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveSystem] Failed to save '{kvp.Key}': {e.Message}");
                }
            }

            string fileJson = JsonUtility.ToJson(data, prettyPrint: true);
            System.IO.File.WriteAllText(SavePath, fileJson);

            Debug.Log($"[SaveSystem] Saved {data.entries.Count} entries to {SavePath}");

            EventBus.Publish(new GameSavedEvent(true));
        }

        // =========================================
        // LOAD
        // =========================================
        public void Load()
        {
            if (!System.IO.File.Exists(SavePath))
            {
                Debug.Log("[SaveSystem] No save file found.");
                return;
            }

            string fileJson = System.IO.File.ReadAllText(SavePath);
            var    data     = JsonUtility.FromJson<SaveFile>(fileJson);

            if (data?.entries == null)
            {
                Debug.LogWarning("[SaveSystem] Save file is empty or corrupt.");
                return;
            }

            int restored = 0;

            foreach (var entry in data.entries)
            {
                if (!_saveables.TryGetValue(entry.id, out var saveable))
                    continue;

                try
                {
                    var type  = Type.GetType(entry.typeName);
                    var state = JsonUtility.FromJson(entry.json, type);
                    saveable.RestoreState(state);
                    restored++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveSystem] Failed to restore '{entry.id}': {e.Message}");
                }
            }

            Debug.Log($"[SaveSystem] Restored {restored} entries from {SavePath}");

            EventBus.Publish(new GameSavedEvent(false));
        }

        // =========================================
        // DELETE SAVE
        // =========================================
        public void DeleteSave()
        {
            if (System.IO.File.Exists(SavePath))
            {
                System.IO.File.Delete(SavePath);
                Debug.Log("[SaveSystem] Save file deleted.");
            }
        }

        public bool HasSave => System.IO.File.Exists(SavePath);

        // =========================================
        // INTERNAL DATA STRUCTURES
        // =========================================
        [Serializable]
        private class SaveFile
        {
            public List<SaveEntry> entries = new();
        }

        [Serializable]
        private class SaveEntry
        {
            public string id;
            public string typeName;
            public string json;
        }
    }
}
