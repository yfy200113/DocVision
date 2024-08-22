using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Networking;
using System.Collections;

public class JSONParser : MonoBehaviour
{
    void Start()
    {
        // ��ȡJSON�ļ�
        string jsonFilePath = @"C:\Users\yfy\Desktop\paperresource\Glyoxal fixation facilitates transcriptome analysis after antigen staining and cell sorting by flow cytometry.json";
        string jsonString = File.ReadAllText(jsonFilePath);

        // ����JSON
        JObject jsonObj = JObject.Parse(jsonString);

        // ����ͼƬ��Ϣ
        Debug.Log("ͼƬ��Ϣ:");
        
        Debug.Log("��Ŀ��" + jsonObj["paper_title"]);
        JArray charts = (JArray)jsonObj["charts"];
        foreach (JObject chart in charts)
        {
            string id = (string)chart["id"];
            string url = (string)chart["url"];
            //StartCoroutine(LoadImageCoroutine(id,url));
            Debug.Log($"ID: {id}, URL: {url}");
        }

        // �����ο�������Ϣ
        Debug.Log("�ο�������Ϣ:");
        JArray references = (JArray)jsonObj["references"];
        foreach (JObject reference in references)
        {
            string id = (string)reference["id"];

            string linkUrl = "";
            JArray links = (JArray)reference["links"];
            if (links != null && links.Count > 0)
            {
                JObject firstLink = (JObject)links[0];
                linkUrl = (string)firstLink["url"];
            }

            Debug.Log($"ID: {id}, Link URL: {linkUrl}");
        }
    }

    private IEnumerator LoadImageCoroutine(string name,string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("�޷�����ͼƬ: " + request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            GameObject imageObject = new GameObject(name);
            

            BoxCollider collider = imageObject.AddComponent<BoxCollider>();
            Debug.Log($"id:  {name}   ͼƬ����"+texture.width +"    ͼƬ��"+ texture.height);
            collider.size = new Vector3((float)(texture.width*0.01), (float)(texture.height * 0.01), 0.01f);

            imageObject.AddComponent<ObjectManipulator>();
            imageObject.AddComponent<NearInteractionGrabbable>();

            SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            imageObject.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        }
    }
}