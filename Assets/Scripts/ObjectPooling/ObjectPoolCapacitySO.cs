using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class CapacityData
{
    public PooledObjectType objectType;
    public int pooledPrefabCount;

    public CapacityData(PooledObjectType type, int count)
    {
        this.objectType = type;
        this.pooledPrefabCount = count;
    }
}

[CreateAssetMenu(menuName = "ObjectPoolCapacityData", fileName = "ObjectPoolCapacityData")]
public class ObjectPoolCapacitySO : ScriptableObject
{
    [SerializeField] internal List<CapacityData> poolCapacities = new List<CapacityData>();
}
