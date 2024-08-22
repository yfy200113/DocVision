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
    public GameObject rotatingOrbs; // Ҫ���ص�RotatingOrbs����
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
        // somesettings�����Ѿ���ʵ����,���԰�ȫ�ط����������Ի򷽷�

        getcontent.sstate = 2;
        Debug.Log("somesettings.state222:::" + getcontent.sstate);
    }


    private IEnumerator GetNewReference(string id)
    {
        
        //progressIndicator.OpenAsync();
        //progressIndicator.Message = "Loading...";
        Debug.Log("��������");
        WWWForm form = new WWWForm();
        form.AddField("id", id);
        using (UnityWebRequest www = UnityWebRequest.Post(predictionEndpoint, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {

                // ����Python���ص�JSON����
                string jsonResponse = www.downloadHandler.text;
                PythonResponse response = JsonUtility.FromJson<PythonResponse>(jsonResponse);
                Debug.Log("response.results[0].success:::" + response.results[0].success);
                // ��� success �ֶ��Ƿ�Ϊ "yes"
                if (response.results[0].success == "yes")
                {
                    // ��ȡ file_content �ֶε�����
                    FileContent fileContent = response.results[0].file_content;

                    // �洢 charts �е� id �� url
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
                        // ��������Ӵ洢 chartId �� chartUrl ���߼�,���罫��洢���ֵ���б���
                    }

                    // �洢 references �е� id �͵�һ�� links �� url
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
                    Debug.Log($"���ȣ�{urls.Count}   {tableurls.Count}   {refurls.Count}");
                    Debug.Log(urls);
                    //rotatingOrbs.SetActive(false);

                    // �޸�messageText���ı�����
                    //progressIndicator.Message = "Loading Success!";

                    // ��һ�������messageText
                    Invoke("HideMessageText", 1.5f);

                }
                else
                {
                    Debug.Log("Python returned success: no");
                    // ��������Ӵ���successΪ"no"���߼�
                }
            }
            else
            {
                Debug.LogError("Error sending image to Python: " + www.error);

            }
        
        }
    }
}
