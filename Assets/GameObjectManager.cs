using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    // Start is called before the first frame update

    private static GameObjectManager instance;

    public List<ObjectInfo> objects;
    public OpenPics OpenPics;
    public test1 test1;
 
    // 获取 GameObjectManager 实例
    public static GameObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameObjectManager>();

                if (instance == null)
                {
                    GameObject go = new GameObject("GameObjectManager");
                    instance = go.AddComponent<GameObjectManager>();
                }
            }

            return instance;
        }
    }


    public void HideAllObjects()
    {
        foreach (ObjectInfo obj in objects)
        {
            if (obj.GameObject != null)
            {
                obj.GameObject.SetActive(false);
            }
        }
    }

    // 新增函数：全部打开
    public void ShowAllObjects()
    {
        foreach (ObjectInfo obj in objects)
        {
            if (obj.GameObject != null)
            {
                obj.GameObject.SetActive(true);
            }
        }
    }

    public string GetType(GameObject gameObject)
    {
        ObjectInfo objectInfo = objects.Find(obj => gameObject);
        string type = objectInfo.Type.ToString();
        return type;
    }

    public int GetId(GameObject gameObject)
    {
        ObjectInfo objectInfo = objects.Find(obj => gameObject);
        return objectInfo.PositionIndex;

    }
    public string Getid(GameObject game)
    {
        ObjectInfo objectInfo = objects.Find(obj => gameObject);
        return objectInfo.Id;
    }
    public string Geturl(GameObject game)
    {
        ObjectInfo objectInfo = objects.Find(obj => gameObject);
        return objectInfo.Url;
    }
    public void DeleteObject(GameObject objToDelete)
    {
        Debug.Log("执行删除函数");
        Destroy(objToDelete);
        GetObjectId getObjectId = objToDelete.GetComponent<GetObjectId>();
        //int id = getObjectId.getid();
        ObjectInfo objectInfo = objects.Find(obj => obj.GameObject == objToDelete);
       // Debug.Log("要被删除的id:" + objectInfo.Id);
        if (objectInfo != null)
        {
            if (objectInfo.Type == ObjectType.Image || objectInfo.Type == ObjectType.Chart)
            {
                
                    OpenPics.availablePositionIndices.Add(objectInfo.PositionIndex);
                Debug.Log("位置" + objectInfo.PositionIndex + "被释放    "+ OpenPics.availablePositionIndices);
                objectInfo.PositionIndex = -1;
               
            }
            
            objects.Remove(objectInfo);
        }
        PrintObjectInfo();
    }
    
   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            objects = new List<ObjectInfo>();
        }
        else
        {
            Destroy(gameObject); // 如果存在其他实例，则销毁当前实例
        }
    }

    public void AddObject(string id, ObjectType type)
    {                                                                                                                                                                      
        Debug.Log("开始执行");
        ObjectInfo newObject = new ObjectInfo(id, type);
        objects.Add(newObject);
    }

    public void AddObject(string id, ObjectType type,GameObject gameObject)
    {
        Debug.Log("开始执行");
        ObjectInfo newObject = new ObjectInfo(id, type,gameObject);
        objects.Add(newObject);
    }

    public void AddObject(string id, ObjectType type, GameObject gameObject,string url)
    {
        Debug.Log("开始执行");
        ObjectInfo newObject = new ObjectInfo(id, type, gameObject,url);
        objects.Add(newObject);
    }
    public void AddObject(string id, ObjectType type, GameObject gameObject, string url,int state)
    {
        Debug.Log("开始执行");
        ObjectInfo newObject = new ObjectInfo(id, type, gameObject, url,state);
        objects.Add(newObject);
    }

    public void AddObject(string id, ObjectType type, GameObject gameObject, int positionIndex,int state)
    {
        Debug.Log("开始执行");
        ObjectInfo newObject = new ObjectInfo(id, type, gameObject,positionIndex,state);
        objects.Add(newObject);
    }
    public void PrintObjectInfo()
    {
        Debug.Log("开始执行输出"+objects.Count);
        foreach (ObjectInfo obj in objects)
        {
            Debug.Log("Object ID: "+obj.Id+"  Type: "+obj.Type+" Gameobject:"+obj.GameObject);
        }
    }
}

public enum ObjectType
{
    Image = 1,
    Chart = 2,
    Video = 3,
    Model = 4,
    Reference = 5,
    // Add more types as needed
}

public class ObjectInfo
{
    public string Id { get; private set; }
    public ObjectType Type { get; private set; }

    public GameObject GameObject { get; private set; }

    public int PositionIndex { get; set; } = -1;

    public string Url { get; private set; }

    public int State { get; private set; }  

    //public GameObject GameObject { get; private set; }
    public ObjectInfo(string id, ObjectType type)
    {
        Id = id;
        Type = type;
    }

    public ObjectInfo(string id, ObjectType type,GameObject gameObject)
    {
        Id = id;
        Type = type;
        GameObject = gameObject;

    }
    public ObjectInfo(string id, ObjectType type, GameObject gameObject, string url)
    {
        Id = id;
        Type = type;
        GameObject = gameObject;
        Url=url;
    }
    public ObjectInfo(string id, ObjectType type, GameObject gameObject, string url,int state)
    {
        Id = id;
        Type = type;
        GameObject = gameObject;
        Url = url;
        State = state;
    }
    public ObjectInfo(string id, ObjectType type, GameObject gameObject,int positionIndex)
    {
        Id = id;
        Type = type;
        GameObject = gameObject;
        PositionIndex = positionIndex;
    }
    public ObjectInfo(string id, ObjectType type, GameObject gameObject, int positionIndex,int state)
    {
        Id = id;
        Type = type;
        GameObject = gameObject;
        PositionIndex = positionIndex;
        State = state;
    }
}
