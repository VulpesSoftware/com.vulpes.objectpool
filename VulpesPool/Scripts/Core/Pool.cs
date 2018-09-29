using System.Collections.Generic;
using UnityEngine;

namespace Vulpes.Pooling
{
    [AddComponentMenu("Vulpes/Core/Pool"), DisallowMultipleComponent]
    public sealed class Pool : MonoBehaviour
    {
        private const int DEFAULT_POOL_SIZE = 8;

        private static Pool instance;

        public static Pool Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<Pool>();
                    if (!instance)
                    {
                        GameObject newInstance = new GameObject(typeof(Pool).ToString());
                        instance = newInstance.AddComponent<Pool>();
                    }
                }
                return instance;
            }
        }

        private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
        private Dictionary<int, int> keyDictionary = new Dictionary<int, int>();

        private void Awake()
        {
            instance = this;
        }

        public static void Add(GameObject akPrefab, int aiCount = DEFAULT_POOL_SIZE)
        {
            int poolKey = akPrefab.GetInstanceID();
            if (!Instance.poolDictionary.ContainsKey(poolKey))
            {
                Instance.poolDictionary.Add(poolKey, new Queue<GameObject>());
            } else
            {
                Debug.LogWarning(string.Format(
                    "Attempted to Add '{0} ({1}) but there is already a known Pool with that ID, expanding Pool by {2}.",
                    akPrefab.name,
                    akPrefab.GetInstanceID(),
                    aiCount.ToString()));
            }
            if (aiCount <= 0)
            {
                Debug.LogWarning("Pool Add Count must be greater than or equal to one!");
                aiCount = 1;
            }
            for (int i = aiCount - 1; i >= 0; i--)
            {
                GameObject newObject = Instantiate(akPrefab) as GameObject;
                Instance.keyDictionary.Add(newObject.GetInstanceID(), akPrefab.GetInstanceID());
                newObject.SetActive(false);
                newObject.transform.SetParent(Instance.transform);
                Instance.poolDictionary[poolKey].Enqueue(newObject);
            }
        }

        public static void Add<T>(T akPrefab, int aiCount = DEFAULT_POOL_SIZE) where T : Component
        {
            Add(akPrefab.gameObject, aiCount);
        }

        public static void Remove(GameObject akPrefab)
        {
            int poolKey = akPrefab.GetInstanceID();
            if (!Instance.poolDictionary.ContainsKey(poolKey))
            {
                Debug.LogWarning(string.Format(
                    "Attempted to Remove Pool for '{0} ({1}), but there is no known Pool with that ID.",
                    akPrefab.name,
                    akPrefab.GetInstanceID()));
                return;
            }
            List<GameObject> cleanup = new List<GameObject>();
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

        public static void Remove<T>(T akPrefab) where T : Component
        {
            Remove(akPrefab.gameObject);
        }

        public static GameObject Spawn(GameObject akPrefab, Vector3 avPosition = default(Vector3), Quaternion aqRotation = default(Quaternion), Transform akParent = null)
        {
            int poolKey = akPrefab.GetInstanceID();
            bool containsKey = Instance.poolDictionary.ContainsKey(poolKey);
            if (!containsKey || Instance.poolDictionary[poolKey].Count == 0)
            {
                if (!containsKey)
                {
                    Debug.LogWarning(string.Format(
                        "Attempted to Spawn '{0} ({1}), but there is no known Pool with that ID, a new Pool will be created for this instance.",
                        akPrefab.name,
                        akPrefab.GetInstanceID()));
                }
                Add(akPrefab);
            }
            GameObject newObject = Instance.poolDictionary[poolKey].Dequeue();
            newObject.transform.position = avPosition;
            newObject.transform.rotation = aqRotation;
            newObject.SetActive(true);
            newObject.transform.SetParent(akParent);
            ISpawnable[] spawnables = newObject.GetComponents<ISpawnable>();
            for (int i = spawnables.Length - 1; i >= 0; i--)
            {
                spawnables[i].OnSpawn();
            }
            return newObject;
        }

        public static T Spawn<T>(T akPrefab, Vector3 avPosition = default(Vector3), Quaternion aqRotation = default(Quaternion), Transform akParent = null) where T : Component
        {
            return Spawn(akPrefab.gameObject, avPosition, aqRotation, akParent).GetComponent<T>();
        }

        public static void Despawn(GameObject akObject)
        {
            int poolKey = Instance.keyDictionary[akObject.GetInstanceID()];
            ISpawnable[] spawnables = akObject.GetComponents<ISpawnable>();
            for (int i = spawnables.Length - 1; i >= 0; i--)
            {
                spawnables[i].OnDespawn();
            }
            akObject.SetActive(false);
            akObject.transform.SetParent(Instance.transform);
            if (Instance.poolDictionary.ContainsKey(poolKey))
            {
                Instance.poolDictionary[poolKey].Enqueue(akObject);
            } else
            {
                Debug.LogWarning(string.Format(
                    "Attempted to Despawn '{0} ({1}), but it is not a member of any known Pool, object reference will be Destroyed instead.", 
                    akObject.name, 
                    akObject.GetInstanceID()));
                Destroy(akObject);
            }
        }

        public static void Despawn<T>(T akObject) where T : Component
        {
            Despawn(akObject.gameObject);
        }
    }
}
