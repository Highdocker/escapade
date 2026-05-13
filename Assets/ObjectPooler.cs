using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    public static Dictionary<string, Component> poolLookup = new Dictionary<string, Component>();
    public static Dictionary<string, Queue<Component>> poolDictionary = new Dictionary<string, Queue<Component>>();

    // Puts Object in queue (recycle)
    public static void EnqueueObject<T>(T item, string name) where T : Component
    {
        // If object is already inactive, assume it's already been returned
        if (!item.gameObject.activeSelf) { return; }

        // Reset transform and other properties
        item.transform.position = Vector2.zero;

        // Reset physics state if present
        //var rb = item.GetComponent<Rigidbody2D>();
        //if (rb != null)
        //{
        //    rb.linearVelocity = Vector2.zero;
        //    rb.angularVelocity = 0f;
        //}

        if (!poolDictionary.ContainsKey(name))
        {
            poolDictionary.Add(name, new Queue<Component>());
        }

        poolDictionary[name].Enqueue(item);
        item.gameObject.SetActive(false);
    }

    // Retrieves Object
    public static T DequeueObject<T>(string key) where T : Component
    {
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].TryDequeue(out var item))
        {
            return (T)item;
        }

        // Pool empty -> create a new instance and return it WITHOUT enqueuing it.
        if (poolLookup.TryGetValue(key, out var prefab))
        {
            return EnqueueNewInstance((T)prefab, key);
        }

        return null;
    }

    // Create a new instance to grow the pool � do NOT enqueue it here.
    public static T EnqueueNewInstance<T>(T item, string key) where T : Component
    {
        T newInstance = Object.Instantiate(item);
        newInstance.gameObject.SetActive(false);
        newInstance.transform.position = Vector2.zero;
        // Do not enqueue the new instance here. Caller will activate and use it.
        return newInstance;
    }

    public static void SetupPool<T>(T pooledItemPrefab, int poolSize, string dictionaryEntry) where T : Component
    {
        if (!poolDictionary.ContainsKey(dictionaryEntry))
            poolDictionary.Add(dictionaryEntry, new Queue<Component>());

        if (!poolLookup.ContainsKey(dictionaryEntry))
            poolLookup.Add(dictionaryEntry, pooledItemPrefab);

        for (int i = 0; i < poolSize; i++)
        {
            T pooledInstance = Object.Instantiate(pooledItemPrefab);
            pooledInstance.gameObject.SetActive(false);
            pooledInstance.transform.position = Vector2.zero;
            poolDictionary[dictionaryEntry].Enqueue(pooledInstance);
        }
    }
}
