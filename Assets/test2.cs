using Dummiesman;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class test2 : MonoBehaviour
{
    //string objPath = "C:/Users/yfy/Downloads/5DZL/5DZL.obj";
    //string objPath = "C:/Users/yfy/Downloads/8U0V.obj";
    //string objPath = "Assets/2DK6.obj";
    string error = string.Empty;
    GameObject loadedObject;
    private string predictionEndpoint = "http://10.53.22.68:8000/crawl";
    public void startcrawl()
    {
        StartCoroutine(GetObjCrawl());
    }

    public IEnumerator GetObjCrawl()
    {
        Debug.Log("start!!!");
        WWWForm webForm = new WWWForm();
        webForm.AddField("target_name", "8SQG", Encoding.UTF8);
        using(UnityWebRequest unityWebRequest = UnityWebRequest.Post(predictionEndpoint, webForm))
        {
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                Debug.Log(unityWebRequest.error);
            }
            else
            {
                Debug.Log("处理中......");
                byte[] objBytes = unityWebRequest.downloadHandler.data;

                // 将字节数组转换为字符串
                string receivedString = System.Text.Encoding.UTF8.GetString(objBytes);
                receivedString = receivedString.Trim('\"');
                string[] results = receivedString.Split(new string[] { "abcd" }, System.StringSplitOptions.None);

                // 将 \n 替换为正确的换行符
                results[0] = results[0].Replace("\\n", System.Environment.NewLine);
                results[1] = results[1].Replace("\\n", System.Environment.NewLine);
                //string[] lines = receivedString.Split('\n');
                //Debug.Log("lines::"+lines.Length);
                Debug.Log(objBytes.Length);
                string objFilePath = "Assets/8R9C.obj";
                string mtlFilePath = "Assets/8R9C.mtl";
                File.WriteAllText(mtlFilePath, results[1]);
                File.WriteAllText(objFilePath, results[0]);
                //File.WriteAllLines(objFilePath, lines);
                Debug.Log("save success!");
                addObj(objFilePath);
            }
        }
    }
    public void addObj(string objPath)
    {
        if (!File.Exists(objPath))
        {
            error = "File doesn't exist.";
        }
        else
        {
            if (loadedObject != null)
                Destroy(loadedObject);
            loadedObject = new OBJLoader().Load(objPath);
            loadedObject.transform.localPosition = new Vector3(-0.06f, 0.12f, 0.34f);
            loadedObject.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
            Transform defaultTransform = loadedObject.transform.Find("default");
            MeshFilter mf = defaultTransform.GetComponent<MeshFilter>();
            loadedObject.AddComponent<ObjectManipulator>();
            loadedObject.AddComponent<NearInteractionGrabbable>();

            Bounds bounds = mf.mesh.bounds;
            BoxCollider bc = loadedObject.AddComponent<BoxCollider>();
            bc.size = bounds.size;
            Debug.Log(bc.size);
            error = string.Empty;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
