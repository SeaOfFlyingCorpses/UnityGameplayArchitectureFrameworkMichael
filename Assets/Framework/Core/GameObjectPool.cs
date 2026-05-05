using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    // =========================================
    // GameObjectPool
    // Generic pool for a single prefab type.
    // Get() activates and returns an instance.
    // Return() deactivates and returns it to pool.
    //
    // Usage:
    //   var pool = new GameObjectPool(prefab, 10, parent);
    //   var obj  = pool.Get(position, rotation);
    //   pool.Return(obj);
    // =========================================
    public class GameObjectPool
    {
        private readonly GameObject      _prefab;
        private readonly Transform       _parent;
        private readonly Queue<GameObject> _available = new();

        public int CountAvailable => _available.Count;

        public GameObjectPool(GameObject prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            // Pre-warm the pool
            for (int i = 0; i < initialSize; i++)
            {
                var obj = Create();
                obj.SetActive(false);
                _available.Enqueue(obj);
            }
        }

        // =========================================
        // GET — activates and returns an instance
        // Creates a new one if pool is empty
        // =========================================
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj;

            if (_available.Count > 0)
            {
                obj = _available.Dequeue();
            }
            else
            {
                // Pool exhausted — create a new instance
                obj = Create();
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            return obj;
        }

        // =========================================
        // RETURN — deactivates and returns to pool
        // =========================================
        public void Return(GameObject obj)
        {
            if (obj == null)
                return;

            obj.SetActive(false);

            if (_parent != null)
                obj.transform.SetParent(_parent);

            _available.Enqueue(obj);
        }

        // =========================================
        // RETURN AFTER DELAY — convenience wrapper
        // =========================================
        public void ReturnDelayed(GameObject obj, float delay, MonoBehaviour runner)
        {
            runner.StartCoroutine(ReturnAfterDelay(obj, delay));
        }

        private System.Collections.IEnumerator ReturnAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Return(obj);
        }

        private GameObject Create()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.SetActive(false);
            return obj;
        }
    }
}
