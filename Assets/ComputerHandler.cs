using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class ComputerHandler : MonoBehaviour
{
    public int ssstate = 1;
    public List<string> urls = new List<string>();
    public List<string> tableurls = new List<string>();
    public List<string> refurls = new List<string>();
    public somesettings somesettings;
    public Getcontent getcontent;
    string predictionEndpoint = "http://192.168.43.195:8001/getnewreference";
    public ProgressIndicatorOrbsRotator progressIndicator;
    public GameObject rotatingOrbs; // 要隐藏的RotatingOrbs对象
    // Start is called before the first frame update
    void Start()
    {
        ssstate = 1;
        Debug.Log("predictionEndpoint handler:" + predictionEndpoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        
        GameObject colliderobject = other.gameObject;
        string type = GameObjectManager.Instance.GetType(colliderobject);
        
        if (type.Equals("Reference"))
        {
            ssstate = 2;
            Debug.Log(ssstate);
            Debug.Log("true");
            string id = GameObjectManager.Instance.Getid(colliderobject);
            string numberPart = id.Substring(3);
            int number = int.Parse(numberPart);
            string url = GameObjectManager.Instance.Geturl(colliderobject);
            Debug.Log(url);
            GameObjectManager.Instance.DeleteObject(other.gameObject);
            //StartCoroutine(WaitForSomeSettings());
            StartCoroutine(GetNewReference(id));

            Debug.Log(id);
        }
    }

    private IEnumerator WaitForSomeSettings()
    {
        //Debug.Log("somesettings.state:::"+somesettings.state);
        while (getcontent == null)
        {
            Debug.Log("somesettings.state111:::" + getcontent.sstate);
            yield return null;
        }
        Debug.Log("somesettings.state111:::" + getcontent.sstate);
        // somesettings对象已经被实例化,可以安全地访问它的属性或方法

        getcontent.sstate = 2;
        Debug.Log("somesettings.state222:::" + getcontent.sstate);
    }


    private IEnumerator GetNewReference(string id)
    {
        
        //progressIndicator.OpenAsync();
        //progressIndicator.Message = "Loading...";
        Debug.Log("尝试连接");
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest www = UnityWebRequest.Post(predictionEndpoint, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {

                // 解析Python返回的JSON数据
                string jsonResponse = www.downloadHandler.text;
                PythonResponse response = JsonUtility.FromJson<PythonResponse>(jsonResponse);
                Debug.Log("response.results[0].success:::" + response.results[0].success);
                // 检查 success 字段是否为 "yes"
                if (response.results[0].success == "yes")
                {
                    // 获取 file_content 字段的内容
                    FileContent fileContent = response.results[0].file_content;

                    // 存储 charts 中的 id 和 url
                    foreach (Chart chart in fileContent.charts)
                    {
                        string chartId = chart.id;
                        string chartUrl = chart.url;
                        //urls.Add(chartUrl);
                        if (chartId.StartsWith("g"))
                        {
                            urls.Add(chartUrl);
                        }
                        else if (chartId.StartsWith("t"))
                        {
                            tableurls.Add(chartUrl);
                        }
                        Debug.Log(chartId + "   " + chartUrl);
                        // 在这里添加存储 chartId 和 chartUrl 的逻辑,例如将其存储到字典或列表中
                    }

                    // 存储 references 中的 id 和第一个 links 的 url
                    foreach (Reference reference in fileContent.references)
                    {
                        string referenceId = reference.id;
                        string referenceUrl = "";

                        if (reference.links != null && reference.links.Length > 0)
                        {
                            referenceUrl = reference.links[0].url;
                            refurls.Add(referenceUrl);
                        }

                        Debug.Log(referenceId + "   " + referenceUrl);
                    }
                    Debug.Log($"长度：{urls.Count}   {tableurls.Count}   {refurls.Count}");
                    Debug.Log(urls);
                    //rotatingOrbs.SetActive(false);

                    // 修改messageText的文本内容
                    //progressIndicator.Message = "Loading Success!";

                    // 在一秒后隐藏messageText
                    Invoke("HideMessageText", 1.5f);

                }
                else
                {
                    Debug.Log("Python returned success: no");
                    // 在这里添加处理success为"no"的逻辑
                }
            }
            else
            {
                Debug.LogError("Error sending image to Python: " + www.error);

            }
        
        }
    }
}
