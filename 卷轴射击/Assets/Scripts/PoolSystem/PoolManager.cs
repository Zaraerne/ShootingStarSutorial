using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] Pool[] enemyPools;
    [SerializeField] Pool[] playerProjectilePools;
    [SerializeField] Pool[] enemyProjectilePools;
    [SerializeField] Pool[] vFXPools;
    [SerializeField] Pool[] lootItemPools;

    static Dictionary<GameObject, Pool> dictionary;

    private void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();
        Initalize(playerProjectilePools);
        Initalize(enemyProjectilePools);
        Initalize(vFXPools);
        Initalize(enemyPools);
        Initalize(lootItemPools);
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);
        CheckPoolSize(lootItemPools);
    }
#endif
    void CheckPoolSize(Pool[] pools)
    {
        foreach(Pool pool in pools)
        {
            if(pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(string.Format("Pool : {0} has a runtime size {1} bigger than its initial siz{2}!",
                    pool.Prefab.name,
                    pool.RuntimeSize,
                    pool.Size));
            }
        }
    }


    void Initalize(Pool[] pools)
    {
        foreach (Pool pool in pools)
        {
            #if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in mutiple pools! Prefab: " + pool.Prefab.name);
                continue;
            }
            #endif
            dictionary.Add(pool.Prefab, pool);
            Transform poolParent = new GameObject("Pool : " + pool.Prefab.name).transform;
            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// <para>根据传入的<paramref name="prefab"></paramref>参数，返回对象池中预备好的游戏对象</para>
    /// </summary>
    /// <param name="prefab">指定游戏对象预制体</param>
    /// <returns>
    /// <para>对象池中已经准备好的游戏对象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
        #endif


        return dictionary[prefab].PrepareObject();
    }
    public static GameObject Release(GameObject prefab,Vector3 position)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
            
        }
        #endif


        return dictionary[prefab].PrepareObject(position);
    }
    public static GameObject Release(GameObject prefab, Vector3 position,Quaternion quaternion)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
            
        }
        #endif
        return dictionary[prefab].PrepareObject(position, quaternion);
    }
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion quaternion, Vector3 localScale)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
            
        }
        #endif


        return dictionary[prefab].PrepareObject(position, quaternion, localScale);
    }

}
