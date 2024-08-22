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
 
    // ��ȡ GameObjectManager ʵ��
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

    // ����������ȫ����
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
        Debug.Log("ִ��ɾ������");
        Destroy(objToDelete);
        GetObjectId getObjectId = objToDelete.GetComponent<GetObjectId>();
        //int id = getObjectId.getid();
        ObjectInfo objectInfo = objects.Find(obj => obj.GameObject == objToDelete);
       // Debug.Log("Ҫ��ɾ����id:" + objectInfo.Id);
        if (objectInfo != null)
        {
            if (objectInfo.Type == ObjectType.Image || objectInfo.Type == ObjectType.Chart)
            {
                
                    OpenPics.availablePositionIndices.Add(objectInfo.PositionIndex);
                Debug.Log("λ��" + objectInfo.PositionIndex + "���ͷ�    "+ OpenPics.availablePositionIndices);
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
            Destroy(gameObject); // �����������ʵ���������ٵ�ǰʵ��
        }
    }

    public void AddObject(string id, ObjectType type)
    {                                                                                                                                                                      
        Debug.Log("��ʼִ��");
        ObjectInfo newObject = new ObjectInfo(id, type);
        objects.Add(newObject);
    }

    public void AddObject(string id, ObjectType type,GameObject gameObject)
    {
        Debug.Log("��ʼִ��");
        ObjectInfo newObject = new ObjectInfo(id, type,gameObject);
        objects.Add(newObject);
    }

    public void AddObject(string id, ObjectType type, GameObject gameObject,string url)
    {
        Debug.Log("��ʼִ��");
        ObjectInfo newObject = new ObjectInfo(id, type, gameObject,url);
        objects.Add(newObject);
    }
    public void AddObject(string id, ObjectType type, GameObject gameObject, string url,int state)
    {
        Debug.Log("��ʼִ��");
        ObjectInfo newObject = new ObjectInfo(id, type, gameObject, url,state);
        objects.Add(newObject);
    }

    public void AddObject(string id, ObjectType type, GameObject gameObject, int positionIndex,int state)
    {
        Debug.Log("��ʼִ��");
        ObjectInfo newObject = new ObjectInfo(id, type, gameObject,positionIndex,state);
        objects.Add(newObject);
    }
    public void PrintObjectInfo()
    {
        Debug.Log("��ʼִ�����"+objects.Count);
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
