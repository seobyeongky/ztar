using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PoolManager : MonoBehaviour
{
    Dictionary<string, Queue<GameObject>> poolDict = new Dictionary<string, Queue<GameObject>>();
    List<ManagedItem> managingBuf = new List<ManagedItem>();
    LinkedList<int> freeIdList = new LinkedList<int>();
    Dictionary<string, Vector3> localPosCacheDict = new Dictionary<string, Vector3>();
    Transform poolT;
    int idCounter = 0;

    public delegate void ReleaseEvent();

    void Awake()
    {
        var go = new GameObject();
        go.transform.SetParent(transform);
        go.name = "pool";
        poolT = go.transform;
    }

    public T Alloc<T>(string filename, bool activate = true) where T : Component
    {
        var obj = GetObjFromPoolOrCreate<T>(filename, activate);

        var newId = AllocId();
        obj.GetComponent<PoolableItem>().id = newId;

        while (managingBuf.Count <= newId)
        {
            managingBuf.Add(new ManagedItem());
        }

        managingBuf[newId].filename = filename;
        managingBuf[newId].T = obj.transform;
#if UNITY_EDITOR
        System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
        managingBuf[newId].tag = string.Format("{0} from ", filename, t.ToString());
#endif

        return obj;
    }

    public T Alloc<T>(Poolables what, bool activate = true) where T : Component
    {
        return Alloc<T>(PoolableUtil.ToFileName(what), activate);
   }

    public void Release<T>(T obj) where T : Component
    {
#if UNITY_EDITOR
        try
        {
#endif
            if (!IsPoolableItem(obj))
            {
                Debug.LogWarning("not poolable item " + obj.name);
                return;
            }

            obj.transform.SetParent(poolT, false);
            obj.gameObject.SetActive(false);
            var id = obj.GetComponent<PoolableItem>().id;
            var managedItem = managingBuf[id];

            if (managedItem.filename == null)
            {
                throw new Exception("filename is null");
            }
            poolDict[managedItem.filename].Enqueue(obj.gameObject);
            managedItem.CleanUp();
            ReleaseId(id);
#if UNITY_EDITOR
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            Debug.Log("dump managing buf ");
            foreach (var managedItem in managingBuf)
            {
                Debug.Log(managedItem.tag);
            }
            Debug.Log("dump obj : " + obj);
        }
        #endif
    }

    public bool IsPoolableItem<T>(T obj) where T : Component
    {
        var item = obj.GetComponent<PoolableItem>();
        return item != null && item.id >= 0;
    }

    public void OnPoolableItemReleased<T>(T obj, ReleaseEvent OnReleased) where T : Component
    {
        var id = obj.GetComponent<PoolableItem>().id;
        managingBuf[id].OnRelease += OnReleased;
    }

    T GetObjFromPoolOrCreate<T>(string filename, bool activate = true) where T : Component
    {
        var prefabPath = "Poolable/" + filename;

        if (!poolDict.ContainsKey(filename))
        {
            poolDict.Add(filename, new Queue<GameObject>());
        }
        else
        {
            var queue = poolDict[filename];
            if (queue.Count > 0)
            {
                var go = queue.Dequeue();
                go.transform.localPosition = localPosCacheDict[filename];
                go.transform.SetParent(null, false);

                if (activate)
                {
                    go.SetActive(true);
                }
                return go.GetComponent<T>();
            }
        }

        var prefab = Resources.Load<T>(prefabPath);
        if (prefab == null)
        {
            throw new Exception("no such prefab " + prefabPath);
        }
        var obj = Instantiate(prefab);
        if (obj.transform.parent == null)
        {
            SceneManager.MoveGameObjectToScene(obj.gameObject, gameObject.scene);
        }
        obj.name = filename;
        if (!localPosCacheDict.ContainsKey(filename))
        {
            localPosCacheDict.Add(filename, obj.transform.localPosition);
        }
        if (!activate)
        {
            obj.gameObject.SetActive(false);
        }
        return obj;
    }

    int GetCurrentQuantity(string filename)
    {
        int currentQuantity = 0;
        if (poolDict.ContainsKey(filename))
        {
            currentQuantity += poolDict[filename].Count;
        }
        for (int i = 0; i < managingBuf.Count; i++)
        {
            var managingItem = managingBuf[i];
            if (managingItem.filename == filename)
            {
                currentQuantity++;
            }
        }

        return currentQuantity;
    }

    public void Reserve(string filename, int quantity)
    {
        int currentQuantity = GetCurrentQuantity(filename);

        if (quantity <= currentQuantity)
        {
            return;
        }

        if (!poolDict.ContainsKey(filename))
        {
            poolDict.Add(filename, new Queue<GameObject>());
        }

        var prefabPath = "Poolable/" + filename;
        var queue = poolDict[filename];
        var prefab = Resources.Load<Transform>(prefabPath);
        if (prefab == null)
        {
            throw new Exception("no such prefab " + prefabPath);
        }

        for (int i = currentQuantity; i < quantity; i++)
        {
            var obj = Instantiate(prefab, poolT);
            if (obj.transform.parent == null)
            {
                SceneManager.MoveGameObjectToScene(obj.gameObject, gameObject.scene);
            }
            obj.name = filename;
            if (!localPosCacheDict.ContainsKey(filename))
            {
                localPosCacheDict.Add(filename, obj.transform.localPosition);
            }
            obj.gameObject.SetActive(false);
            queue.Enqueue(obj.gameObject);
        }
    }

    public void CleanUp()
    {
        //Debug.Log("--- pool manager cleaning up ---")
        foreach (var managedItem in managingBuf)
        {
            if (managedItem.filename != null)
            {
                //Debug.Log("lets release " + managedItem.filename);
                Release(managedItem.T);
            }
        }

        foreach (var poo in poolDict)
        {
            var queue = poolDict[poo.Key];
            foreach (var go in queue)
            {
                Destroy(go);
            }
            queue.Clear();
        }
    }

    int AllocId()
    {
        if (freeIdList.Count == 0)
        {
            return idCounter++;
        }

        var frontId = freeIdList.First.Value;
        freeIdList.RemoveFirst();
        return frontId;
    }

    void ReleaseId(int id)
    {
        freeIdList.AddLast(id);
    }

    class ManagedItem
    {
        public string filename;
        public Transform T;
#if UNITY_EDITOR
        public string tag;
#endif
        public event ReleaseEvent OnRelease;
        
        public void CleanUp()
        {
            filename = null;
#if UNITY_EDITOR
            tag = "";
#endif
            if (OnRelease != null)
            {
                foreach (Delegate d in OnRelease.GetInvocationList())
                {
                    OnRelease -= (ReleaseEvent)d;
                }
            }
        }
    }
}