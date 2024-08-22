using Dummiesman;
using System.IO;
using UnityEngine;

public class ObjFromFile : MonoBehaviour
{
    string objPath = "C:/Users/yfy/Downloads/5DZL/5DZL.obj";
    string error = string.Empty;
    GameObject loadedObject;

    void OnGUI() {
        objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);

        GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
        if(GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
        {
            //file path
            if (!File.Exists(objPath))
            {
                error = "File doesn't exist.";
            }else{
                if(loadedObject != null)            
                    Destroy(loadedObject);
                loadedObject = new OBJLoader().Load(objPath);
                loadedObject.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
                Transform defaultTransform = loadedObject.transform.Find("default");
                MeshFilter mf = defaultTransform.GetComponent<MeshFilter>();

                Bounds bounds = mf.mesh.bounds;
                BoxCollider bc = loadedObject.AddComponent<BoxCollider>();
                bc.size = bounds.size;
                Debug.Log(bc.size);
                error = string.Empty;
            }
        }

        if(!string.IsNullOrWhiteSpace(error))
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
            GUI.color = Color.white;
        }
    }
}
