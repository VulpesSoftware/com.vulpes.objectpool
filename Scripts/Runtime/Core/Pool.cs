using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vulpes.Pooling
{
    [AddComponentMenu("Vulpes/Pooling/Pool"), DisallowMultipleComponent, DefaultExecutionOrder(-99)]
    public sealed class Pool : MonoBehaviour
    {
        private const int DEFAULT_POOL_SIZE = 8;
        
        private readonly Dictionary<int, Queue<GameObject>> poolDictionary = new();
        private readonly Dictionary<int, int> keyDictionary = new();

        [SerializeField] private bool destroyUnpooledObjects = false;

        public static Pool Instance { get; private set; }

        private void Awake()
            => Instance = this;

        /// <summary>
        /// Adds a prefab to the pooling system, if the added prefab already exists and contains inactive instances it will expand the pool by the specified count.
        /// </summary>
        public static void Add(GameObject prefab, in int count = DEFAULT_POOL_SIZE)
        {
            int poolKey = prefab.GetInstanceID();
            if (!Instance.poolDictionary.ContainsKey(poolKey))
            {
                Instance.poolDictionary.Add(poolKey, new());
            } else
            {
                int availableCount = Instance.poolDictionary[poolKey].Count;
                if (availableCount == 0)
                {
                    Debug.LogWarning($"{Instance.GetType()}.Add: Attempted to Add '{prefab.name} ({prefab.GetInstanceID()}) but there is already a known Pool with that ID, expanding Pool by {count}.");
                } else
                {
                    Debug.LogWarning($"{Instance.GetType()}.Add: Attempted to Add '{prefab.name} ({prefab.GetInstanceID()}) but there is already a known Pool with that ID containing {availableCount} inactive instances");
                    return;
                }
            }
            if (count <= 0)
            {
                Debug.LogError($"{Instance.GetType()}.Add: Pool Add Count must be greater than or equal to one!");
                return;
            }
            for (int i = count - 1; i >= 0; i--)
            {
                GameObject newObject = Instantiate(prefab);
                Instance.keyDictionary.Add(newObject.GetInstanceID(), prefab.GetInstanceID());
                newObject.SetActive(false);
                newObject.transform.SetParent(Instance.transform);
                Instance.poolDictionary[poolKey].Enqueue(newObject);
            }
        }

        /// <summary>
        /// Adds a prefab to the pooling system, if the added prefab already exists and contains inactive instances it will expand the pool by the specified count.
        /// </summary>
        public static void Add<T>(T prefab, in int count = DEFAULT_POOL_SIZE) 
            where T : Component
            => Add(prefab.gameObject, count);

        /// <summary>
        /// Removes a prefab from the pooling system if it exists and cleans up all instances of that prefab.
        /// </summary>
        public static void Remove(GameObject prefab)
        {
            int poolKey = prefab.GetInstanceID();
            if (!Instance.poolDictionary.ContainsKey(poolKey))
            {
                Debug.LogWarning($"{Instance.GetType()}.Remove: Attempted to Remove Pool for '{prefab.name} ({prefab.GetInstanceID()}), but there is no known Pool with that ID.");
                return;
            }
            List<GameObject> cleanup = new();
            foreach (GameObject obj in Instance.poolDictionary[poolKey])
            {
                cleanup.Add(obj);
                Instance.keyDictionary.Remove(obj.GetInstanceID());
            }
            Instance.poolDictionary[poolKey].Clear();
            Instance.poolDictionary.Remove(poolKey);
            for (int i = cleanup.Count - 1; i >= 0; i--)
            {
                Destroy(cleanup[i]);
            }
        }

        /// <summary>
        /// Removes a prefab from the pooling system if it exists and cleans up all instances of that prefab.
        /// </summary>
        public static void Remove<T>(T prefab) 
            where T : Component
            => Remove(prefab.gameObject);

        /// <summary>
        /// Spawns and returns an instance of the specified prefab.
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            int poolKey = prefab.GetInstanceID();
            bool containsKey = Instance.poolDictionary.ContainsKey(poolKey);
            if (!containsKey || Instance.poolDictionary[poolKey].Count == 0)
            {
                if (!containsKey)
                {
                    Debug.LogWarning($"{Instance.GetType()}.Spawn: Attempted to Spawn '{prefab.name} ({prefab.GetInstanceID()}), but there is no known Pool with that ID, a new Pool will be created for this instance.");
                }
                Add(prefab);
            }
            GameObject newObject = Instance.poolDictionary[poolKey].Dequeue();
            newObject.transform.SetPositionAndRotation(position, rotation);
            newObject.SetActive(true);
            newObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(newObject, parent != null ? parent.gameObject.scene : SceneManager.GetActiveScene());
            newObject.transform.SetParent(parent);
            ISpawnable[] spawnables = newObject.GetComponents<ISpawnable>();
            for (int i = spawnables.Length - 1; i >= 0; i--)
            {
                spawnables[i].OnSpawn();
            }
            return newObject;
        }

        /// <summary>
        /// Spawns and returns an instance of the specified prefab.
        /// </summary>
        public static T Spawn<T>(T prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null) 
            where T : Component
            => Spawn(prefab.gameObject, position, rotation, parent).GetComponent<T>();

        /// <summary>
        /// Despawns an instance of the specified prefab and returns it to the pool.
        /// </summary>
        public static void Despawn(GameObject instance)
        {
            ISpawnable[] spawnables = instance.GetComponents<ISpawnable>();
            for (int i = spawnables.Length - 1; i >= 0; i--)
            {
                spawnables[i].OnDespawn();
            }
            if (!Instance.keyDictionary.TryGetValue(instance.GetInstanceID(), out int poolKey))
            {
                Debug.LogWarning($"{Instance.GetType()}.Despawn: Attempted to Despawn '{instance.name} ({instance.GetInstanceID()}), but it is not a member of any known Pool, object reference will be {(Instance.destroyUnpooledObjects ? "destroyed" : "made inactive")} instead.");
                if (Instance.destroyUnpooledObjects)
                {
                    Destroy(instance);
                } else
                {
                    instance.SetActive(false);
                }
                return;
            }
            instance.SetActive(false);
            instance.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(instance, Instance.gameObject.scene);
            instance.transform.SetParent(Instance.transform);
            Instance.poolDictionary[poolKey].Enqueue(instance);
        }

        /// <summary>
        /// Despawns an instance of the specified prefab and returns it to the pool.
        /// </summary>
        public static void Despawn<T>(T instance) 
            where T : Component
            => Despawn(instance.gameObject);
    }
}