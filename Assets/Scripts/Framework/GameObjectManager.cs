using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    public string name;
    public Queue<GameObject> pool = new Queue<GameObject>();
    private int maxCount = 5;
    public float maxIdleTime = 10.0f;
    private float idleTime = 0;
    //add obj
    public bool Add(GameObject obj, bool force = false)
    {
        idleTime = 0;
        if (pool.Contains(obj))
        {
            return true;
        }
        if (force || pool.Count < maxCount)
        {
            pool.Enqueue(obj);
            return true;
        }else
        {
            return false;
        }
    }

    public GameObject getObject()
    {
        GameObject obj = null;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        idleTime = 0;
        return obj;
    }

    public int getCount()
    {
        return pool.Count;
    }

    public bool isFull()
    {
        return (maxCount > 0 && pool.Count >= maxCount);
    }

    public bool isEmpty()
    {
        return pool.Count == 0;
    }

    public void Destory(){
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            GameObject.DestroyImmediate(obj);
        }
        idleTime = 0;
    }

    public bool Tick()
    {
        if (maxIdleTime <= 0)
        {
            return false;
        }
        idleTime += Time.deltaTime;
        return idleTime >= maxIdleTime;
    }
}

public class GameObjectManager : MonoBehaviour {
    private static GameObjectManager mInstance;

    private Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();

    private float maxIdleTime = 10.0f;

    public static GameObjectManager GetInstance()
    {
        if (mInstance == null)
        {
#if UNITY_EDITOR
            Debug.Log("GameObjectManager is null");
#endif
        }
        return mInstance;
    }

    void Awake()
    {
        mInstance = this;     
    }
    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LateUpdate()
    {
        try
        {
            foreach (var item in pools)
            {
                GameObjectPool objectPool = item.Value;
                if (objectPool.Tick())
                {
                    objectPool.Destory();
                    if (!string.IsNullOrEmpty(objectPool.name))
                    {
                        pools.Remove(objectPool.name);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
#if UNITY_EDITOR
            Debug.LogException(ex);
#endif
        }

    }

    public GameObject GetObject(string path)
    {
        GameObject gameObject = null;
        if (pools.ContainsKey(path))
        {
            GameObjectPool pool;
            pools.TryGetValue(path, out pool);
            gameObject = pool.getObject();
        }
        if (gameObject == null)
        {
            CreateObject(path);
        }
        return gameObject;

    }

    public GameObject CreateObject(GameObject prefab, string path)
    {
        GameObject obj = GameObject.Instantiate(prefab);
        //obj.transform.SetParent(transform);
        //obj.AddComponent<game>
        return obj;
    }

    public GameObject CreateObject(string path)
    {
        GameObject uiPrefab = ResourceManager.LoadResourceUI(path);
        if (uiPrefab == null)
        {
#if UNITY_EDITOR
            Debug.LogFormat("Can not get prefab {0}",path);
#endif
        }
        return CreateObject(uiPrefab, path);
    }

    public bool StoreObject(GameObject obj, string path)
    {
        bool success = true;

        string key = path;
        GameObjectPool pool = null;
        //不在缓存池里面，创建新的添加
        if (!pools.ContainsKey(key))
        {
            pool = new GameObjectPool();
            pool.Add(obj);
            pool.maxIdleTime = maxIdleTime;
            pools.Add(key, pool);
        }
        //已经在缓存池里面,直接取出
        pools.TryGetValue(key, out pool);
        pool.Add(obj);
        pool.maxIdleTime = maxIdleTime;

        return success;
    }

    //clear pool
    public void ClearObject(string path, bool force = true)
    {
        GameObjectPool objectPool = null;
        if (pools.ContainsKey(path))
        {
            pools.TryGetValue(path, out objectPool);
            objectPool.Destory();
            pools.Remove(path);
        }
    }
}
