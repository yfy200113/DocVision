using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObjectId : MonoBehaviour
{
    public int ObjectId;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void setid(int id)
    {
        ObjectId = id;
    }
    public int getid()
    {
        return ObjectId;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
