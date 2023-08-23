using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class PrefabPaths
{
    public const string prefabRootPath = "Prefabs/";
}

/// <summary>
/// Provides object pooling for selected objects.
/// HOW TO USE: in current configuration, this script loads selected prefabs in resources folder 
/// with these rules: 
/// 
/// * PooledObjectType enums must be in "Folder1_Folder2_TargetFolder" format.
/// * Corresponding prefabs to be loaded must be in "Resources/Prefabs/Folder1/Folder2/TargetFolder"
/// * A ObjectPoolCapacitySO scriptable object asset named "ObjectPoolCapacityData" must be in "Resources" folder
/// 
/// As long as these conditions are met, this script will load and pool required objects automatically.
/// </summary>
public static class ObjectPool
{
    static Dictionary<PooledObjectType, GameObject[]> pooledPrefabs;
    static Dictionary<PooledObjectType, List<GameObject>> objectPools;
    static ObjectPoolCapacitySO poolCapacityData;

    public static GameObject objectPoolsHolder;


    /// <summary>
    /// Initializes the pools
    /// </summary>
    public static void Initialize()
    {
        InitializeDictionaries();
        InitializeContainerObjects();

        // fill pools and create parent objects in scene
        foreach (PooledObjectType objectType in Enum.GetValues(typeof(PooledObjectType)))
        {
            // here we add the prefabs in the resource path to our prefab dictionary
            // ** IMPORTANT NOTE ** read the class description about how this code loads prefabs.
            string relativePath = objectType.ToString().Replace('_', '/');
            GameObject[] prefabs = Resources.LoadAll<GameObject>(PrefabPaths.prefabRootPath + relativePath);

            if (prefabs.Length == 0)
            {
                Debug.LogWarning($"No prefabs loaded at {"Resources/" + PrefabPaths.prefabRootPath + relativePath}." +
                            "Ensure enum values and prefab paths match, and root path is set correctly.");
            }

            pooledPrefabs.Add(objectType, prefabs);

            if (!objectPools.ContainsKey(objectType))
            {
                Debug.LogError($"Key {objectType} is missing from the object pool.");
                continue;
            }

            // populate lists with GameObjects 
            for (int i = 0; i < objectPools[objectType].Capacity; i++)
            {
                objectPools[objectType].Add(GetNewObject(objectType));
            }
        }
    }

    /// <summary>
    /// Creates container parent game objects for pooled objects in the scene.
    /// </summary>
    private static void InitializeContainerObjects()
    {
        foreach (string name in Enum.GetNames(typeof(PooledObjectType)))
        {
            GameObject child = new GameObject(name);
            child.transform.parent = objectPoolsHolder.transform;
        }
    }

    /// <summary>
    /// Initializes prefab and pooled object dictionaries and lists.
    /// Uses data from "ObjectPoolCapacityData" scriptable object.
    /// Creates a default ObjectPoolCapacityData asset if one cannot be found.
    /// </summary>
    private static void InitializeDictionaries()
    {
        pooledPrefabs = new Dictionary<PooledObjectType, GameObject[]>();
        objectPools = new Dictionary<PooledObjectType, List<GameObject>>();

        poolCapacityData = Resources.Load<ObjectPoolCapacitySO>("ObjectPoolCapacityData");

        if (poolCapacityData == null)
        {
            Debug.LogWarning($"ObjectPoolOptionsSO scriptable object is missing in resources folder. " +
                            "Creating new asset with default values. Make sure to replace with custom values");
            CreateDefaultAsset();
        }
        else if (poolCapacityData.poolCapacities.Count != Enum.GetNames(typeof(PooledObjectType)).Length)
        {
            Debug.LogWarning($"ObjectPoolOptionsSO scriptable object has different number of entries than PooledObjectType. " +
                              "Creating new asset with default values. Make sure to replace with custom values");
            CreateDefaultAsset();
        }

        foreach (CapacityData capacityData in poolCapacityData.poolCapacities)
        {
            objectPools.Add(capacityData.objectType, new List<GameObject>(capacityData.pooledPrefabCount));
        }

        objectPoolsHolder = new GameObject("ObjectPoolsParent");
    }

    private static void CreateDefaultAsset()
    {
        ObjectPoolCapacitySO defaultData = ScriptableObject.CreateInstance<ObjectPoolCapacitySO>();
        // path has to start at "Assets"


        foreach (PooledObjectType objectType in Enum.GetValues(typeof(PooledObjectType)))
        {
            defaultData.poolCapacities.Add(new CapacityData(objectType, 20));
        }

        string path = "Assets/Resources/ObjectPoolCapacityData.asset";
        AssetDatabase.CreateAsset(defaultData, path);

        poolCapacityData = Resources.Load<ObjectPoolCapacitySO>("ObjectPoolCapacityData");
    }

    /// <summary>
    /// Gets a pooled object from the pool
    /// </summary>
    /// <returns>pooled object</returns>
    /// <param name="name">name of the pooled object to get</param>
    public static GameObject GetPooledObject(PooledObjectType name)
    {
        if (!objectPools.ContainsKey(name))
        {
            Debug.LogError($"Key {name} is missing from the object pool");
            GameObject placeholder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            return placeholder;
        }

        List<GameObject> pool = objectPools[name];

        // check for available object in pool
        if (pool.Count > 0)
        {
            // remove object from pool and return (replace code below)
            int i = UnityEngine.Random.Range(0, pool.Count);
            GameObject obj = pool[i];
            pool.RemoveAt(i);
            return obj;
        }
        else
        {
            // pool empty, so expand pool and return new object (replace code below)
            pool.Capacity++;
            return GetNewObject(name);
        }
    }

    /// <summary>
    /// Returns a pooled object to the pool
    /// </summary>
    /// <param name="name">name of pooled object</param>
    /// <param name="obj">object to return to pool</param>
    public static bool ReturnPooledObject(PooledObjectType name,
        GameObject obj)
    {
        if (!objectPools.ContainsKey(name))
        {
            Debug.LogError($"Key {name} is missing from the object pool");
            return false;
        }
        
        obj.SetActive(false);
        // obj.transform.localPosition = Vector3.zero;
        objectPools[name].Add(obj);

        return true;
    }

    /// <summary>
    /// Gets a new object
    /// </summary>
    /// <returns>new object</returns>
    static GameObject GetNewObject(PooledObjectType type)
    {
        if (pooledPrefabs[type].Length == 0)
        {
            Debug.LogWarning($"No prefabs loaded for {type.ToString()}, ensure enum value and prefab paths match.");
            return null;
        }
        GameObject obj;

        int i = UnityEngine.Random.Range(0, pooledPrefabs[type].Length);

        obj = GameObject.Instantiate(pooledPrefabs[type][i], GameObject.Find(type.ToString()).transform);
        obj.name = pooledPrefabs[type][i].name;

        obj.SetActive(false);
        return obj;
    }

    /// <summary>
    /// Removes all the pooled objects from the object pools
    /// </summary>
    public static void EmptyPools()
    {
        // add your code here
        foreach (KeyValuePair<PooledObjectType, List<GameObject>> keyValuePair in objectPools)
        {
            keyValuePair.Value.Clear();
        }
    }


    /// <summary>
    /// Gets the current pool count for the given pooled object
    /// </summary>
    /// <param name="type">pooled object name</param>
    /// <returns>current pool count</returns>
    public static int GetPoolCount(PooledObjectType type)
    {
        if (objectPools.ContainsKey(type))
        {
            return objectPools[type].Count;
        }
        else
        {
            // should never get here
            return -1;
        }
    }

    /// <summary>
    /// Gets the current pool capacity for the given pooled object
    /// </summary>
    /// <param name="type">pooled object name</param>
    /// <returns>current pool capacity</returns>
    public static int GetPoolCapacity(PooledObjectType type)
    {
        int capacity = 0;

        if (objectPools.ContainsKey(type))
        {
            capacity = objectPools[type].Capacity;
        }
        else
        {
            Debug.LogWarning($"Key {type} is missing from the object pool.");
        }

        return capacity;
    }

    /// <summary>
    ///  Returns all currently active pooled objects to object pool. 
    ///  This can be used to reset pooled objects instead of reloading a scene.
    /// </summary>
    public static void ReturnAllPooledObjects()
    {
        foreach (Transform container in objectPoolsHolder.transform)
        {
            foreach (Transform item in container)
            {
                // skip if item is already inactive (thus in object pool)
                if (!item.gameObject.activeInHierarchy)
                {
                    continue;
                }

                // PooledObjectType enums have an extra string "Obstacles_" before prefab name,
                // so we add it
                string name = "Obstacles_" + item.name;
                if (Enum.TryParse(name, out PooledObjectType type))
                {
                    // if our object matches with a PooledObjectType, return it to pool
                    ReturnPooledObject(type, item.gameObject);
                }
                else
                {
                    Debug.LogWarning($"{item.name} doesn't match any PooledObjectType enum value. Can't return to pool.");
                }
            }
        }
    }
}
