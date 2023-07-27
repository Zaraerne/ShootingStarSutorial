using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool 
{
    [SerializeField] GameObject prefab;
    [SerializeField] int size;
    Transform parent;
    Queue<GameObject> queue;

    public int Size => size;
    public GameObject Prefab => prefab;
    public int RuntimeSize => queue.Count;

    public void Initialize(Transform parent)
    {
        this.parent = parent;
        queue = new Queue<GameObject>();
        for(int i = 0; i < size; i++)
        {
            queue.Enqueue(Copy());
        }
    }

    GameObject AvailableObject()
    {
        GameObject avaiableObject = null;

        if(queue.Count > 0 && !queue.Peek().activeSelf)
        {
            avaiableObject = queue.Dequeue();
        }
        else
        {
            avaiableObject = Copy();
        }
        Return(avaiableObject);
        return avaiableObject;
    }

    GameObject Copy()
    {
        GameObject copy = Object.Instantiate(prefab, parent);
        copy.SetActive(false);
        return copy;
    }

    public void Return(GameObject gameObject)
    {
        queue.Enqueue(gameObject);
    }

    public GameObject PrepareObject()
    {
        GameObject prepareObject = AvailableObject();
        prepareObject.SetActive(true);
        return prepareObject;
    }
    public GameObject PrepareObject(Vector3 position)
    {
        GameObject prepareObject = AvailableObject();
        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        return prepareObject;
    }
    public GameObject PrepareObject(Vector3 position,Quaternion quaternion)
    {
        GameObject prepareObject = AvailableObject();
        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        prepareObject.transform.rotation = quaternion;
        return prepareObject;
    }
    public GameObject PrepareObject(Vector3 position, Quaternion quaternion,Vector3 localScale)
    {
        GameObject prepareObject = AvailableObject();
        prepareObject.SetActive(true);
        prepareObject.transform.position = position;
        prepareObject.transform.rotation = quaternion;
        prepareObject.transform.localScale = localScale;
        return prepareObject;
    }



}
