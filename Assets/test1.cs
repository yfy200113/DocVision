using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using QRTracking;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Vuplex.WebView;



public class test1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = 0; i < 10; i++)
        {
            int index = i;
            buttons[i].OnClick.AddListener(() => OnButtonClicked(index));

            // 获取按钮的Image子物体
            GameObject imageObject = buttons[i].gameObject.transform.Find("ButtonContent/Image").gameObject;

            // 启动协程来加载按钮背景图片并设置长宽比
            StartCoroutine(LoadButtonImage(imageObject, url[index], index));

        }*/
        for (int i = 0; i < 9; i++)
        {
            int index = i;
            //buttons[i].OnClick.AddListener(() => OnButtonClicked(index));

            // 获取按钮的Image子物体
            GameObject imageObject = imagecontents[i].gameObject.transform.Find("Image").gameObject;
            imagecontents[i].transform.localPosition =new Vector3(0, 0, 0.3f);
            Debug.Log(imageObject);
            // 启动协程来加载按钮背景图片并设置长宽比
            //StartCoroutine(LoadButtonImage(imageObject, url[index], index));

        }
    }

    int j = 0;
    public Getcontent getcontent;
    public QRCodesVisualizer qRCodesVisualizer;
    private static int nextWebViewId = 0;
    public Dictionary<int,WebViewPrefab> webViewPrefabs = new Dictionary<int, WebViewPrefab>();
    //public somesettings ss;
    int i = 0;
    public List<int> availablePositionIndices = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };

    public Interactable[] buttons;
    public GameObject[] imagecontents;
    public GameObject ImageContent;
    public somesettings somesettings;

    Vector3[] positionxyz = new Vector3[9];
    Vector2[] positionsize = new Vector2[9];
    int[] flagifavailable = new int[9];
    string[] url = {
        "https://media.springernature.com/lw685/springer-static/image/art%3A10.1038%2Fs41586-024-07265-8/MediaObjects/41586_2024_7265_Fig1_HTML.png",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g002",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g003",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g004",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g005",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g002",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g003",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g004",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g005",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g002",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g003",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g004",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g005"
         }
        ;
    string[] tableurl =
    {
        "https://picdm.sunbangyan.cn/2024/02/01/018fb9f3b11be0a009b5a3f7348bbe3f.jpeg",
        "https://www.freeimg.cn/i/2024/03/11/65eeba9876ea8.png"
    };
    // Update is called once per frame

    private void OnButtonClicked(int index)
    {
        // 获取对应的图片URL地址
        string imageUrl = url[index];

        // 使用WebView显示图片
        loadingurl(imageUrl, 0);
    }
    public void availablePosition()
    {
        if (availablePositionIndices.Count > 0)
        {
            int positionid = availablePositionIndices[0];
            //Debug.Log("位置" + positionid + "被占用    " + availablePositionIndices);
            availablePositionIndices.RemoveAt(0);

            StartCoroutine(LoadButtonImage(positionid));

        }
        else
        {
            Debug.Log("暂无可用位置");
        }
    }
    private IEnumerator LoadButtonImage(int positionid)
    {
        Vector3 newpos = qRCodesVisualizer.posxyz;
        // 使用UnityWebRequest加载图片
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url[positionid]);
        request.certificateHandler = new BypassCertificateHandler();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 获取加载的图片纹理
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // 创建Sprite并设置为Image组件的sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            GameObject imageContent = Instantiate(ImageContent, Vector3.zero, Quaternion.identity);
            //GameObject imageContent = ImageContent[positionid];
            GameObject imageObject = imageContent.transform.Find("Image").gameObject;
            imageObject.GetComponent<Image>().sprite = sprite;
            BoxCollider collider = imageObject.AddComponent<BoxCollider>();

            imageObject.AddComponent<ObjectManipulator>();
            imageObject.AddComponent<NearInteractionGrabbable>();


            if (flagifavailable[positionid] == 1)
            {
                Debug.Log($"位置{positionid}被占用过");
                Debug.Log($"位置{positionid}的大小：positionsize[positionid]");

                texture = AdjustImageSize((int)positionsize[positionid].x, (int)positionsize[positionid].y, texture);
                collider.size = new Vector3((float)(texture.width * 0.01), (float)(texture.height * 0.01), 0.01f);
 
                RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(texture.width / 100, texture.height / 100);

                imageObject.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                imageContent.transform.localPosition = positionxyz[positionid];
                if (positionid < 3)
                {
                    imageObject.transform.eulerAngles = new Vector3(9 - positionid * 9, 180 - 25, 0);
                }
                else if (positionid < 6)
                {
                    imageObject.transform.eulerAngles = new Vector3(9 - (positionid - 3) * 9, 180 - 44, 0);
                }
                else if (positionid < 9)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9, 0, 0);
                }
                GameObjectManager.Instance.AddObject("chart", ObjectType.Chart, imageContent, positionid,somesettings.state);
                GameObjectManager.Instance.PrintObjectInfo();
            }
            else
            {
                Debug.Log($"位置{positionid}未！！！被占用过");
                flagifavailable[positionid] = 1;
                texture = AdjustImageSize((int)600f, (int)600f, texture);
                Vector2 newwd; newwd.x = texture.width; newwd.y = texture.height;
                positionsize[positionid] = newwd;

                collider.size = new Vector3((float)(texture.width * 0.01), (float)(texture.height * 0.01), 0.01f);

                RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(texture.width / 100, texture.height / 100);
                MeshRenderer renderer = imageObject.GetComponent<MeshRenderer>();

                if (positionid == 2)
                {
                    GameObject buttonContent = GameObject.Find("ButtonContent2");
                    GameObject image2 = GameObject.Find("ButtonContent2/Image");
                    image2.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                    RectTransform rect2 = image2.GetComponent<RectTransform>();
                    rect2.sizeDelta = new Vector2(texture.width / 5, texture.height / 5);
                    // 在Buttoncontent的子对象中查找Image组件
                    buttonContent.transform.localPosition = new Vector3(0.2f, -0.07f, 0.35f);
                }
                if (renderer != null && renderer.sharedMaterials.Length > 0 && renderer.sharedMaterials[0] != null)
                {
                    //方法一:
                    Material[] newMaterials = new Material[renderer.sharedMaterials.Length - 1];
                    for (int i = 1; i < renderer.sharedMaterials.Length; i++)
                    {
                        newMaterials[i - 1] = renderer.sharedMaterials[i];
                    }
                    renderer.sharedMaterials = newMaterials;

                    //方法二:
                    //renderer.sharedMaterials[0] = null;
                    //Material[] mats = renderer.materials;

                    renderer.SetPropertyBlock(null);
                }

                imageObject.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

                Vector3 newvector3 = CalculateImagePosition(positionid, texture.width, texture.height, newpos);
                Vector3 vector3;
                vector3.x = newvector3.x; vector3.y = newvector3.y; vector3.z = newvector3.z - 0.15f;
                positionxyz[positionid] = vector3;
                imageContent.transform.localPosition = vector3;
                if (positionid < 3)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9 + positionid * 9, -25, 0);
                }
                else if (positionid < 6)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9 + (positionid - 3) * 9, -44, 0);
                }
                else if (positionid < 9)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9, 0, 0);
                }
                GameObjectManager.Instance.AddObject("11", ObjectType.Chart, imageContent, positionid, somesettings.state);
                GameObjectManager.Instance.PrintObjectInfo();
            }

        }
        else
        {
            Debug.LogError("Failed to load image: " + request.error);
        }
    }
    private Texture2D AdjustImageSize(int width, int height, Texture2D texture)
    {
        float textureAspectRatio = (float)texture.width / texture.height;
        float regionAspectRatio = (float)width / height;
        int adjustedWidth, adjustedHeight;

        if (textureAspectRatio > regionAspectRatio)
        {
            // 图片比给定区域更宽,以区域宽度为基准缩放
            adjustedWidth = width;
            adjustedHeight = Mathf.RoundToInt(width / textureAspectRatio);
        }
        else
        {
            // 图片比给定区域更高或等宽高比,以区域高度为基准缩放
            adjustedHeight = height;
            adjustedWidth = Mathf.RoundToInt(height * textureAspectRatio);
        }

        Debug.Log($"调整后大小：{adjustedWidth} x {adjustedHeight}");

        Texture2D adjustedTexture = new Texture2D(adjustedWidth, adjustedHeight);
        Graphics.ConvertTexture(texture, adjustedTexture);

        return adjustedTexture;
    }

    private Vector3 CalculateImagePosition(int positionid, int width, int height, Vector3 computerPosition)
    {
        float x = computerPosition.x;
        float y = computerPosition.y;
        float z = computerPosition.z;
        float y2 = computerPosition.y + 0.09f;



        switch (positionid)
        {
            case 0:
                x -= 0.09f;
                y = y + 0.09f;
                break;
            case 1:
                x -= 0.09f;
                y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.00015) - (float)(height * 0.00015);
                break;
            case 2:
                x -= 0.09f;
                y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.0003 + positionsize[positionid - 2].y * 0.00015) - (float)(height * 0.00015);
                break;
            case 3:
                x -= 0.24f;
                y = y + 0.09f;
                z = z - 0.11f;
                break;
            case 4:
                x -= 0.24f;
                y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.00015) - (float)(height * 0.00015);
                z = z - 0.11f;
                break;
            case 5:
                x -= 0.24f;
                y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.0003 + positionsize[positionid - 2].y * 0.00015) - (float)(height * 0.00015);
                z = z - 0.11f;
                break;
            case 6:
                x += 0.09f;
                y += (float)(height * 0.00015);
                break;
            case 7:
                x += (float)(positionsize[positionid - 1].x * 0.00015) + (float)(width * 0.00015) + 0.1f;
                y += (float)(height * 0.00015);
                break;
            case 8:
                x += (float)(positionsize[positionid - 1].x * 0.0003 + positionsize[positionid - 2].x * 0.00015) + (float)(width * 0.00015) + 0.1f;
                y += (float)(height * 0.00015);
                break;
        }

        return new Vector3(x, y, z);
    }

    /*private IEnumerator LoadButtonImage(GameObject imageObject, string imageUrl,int index)
    {
        // 使用UnityWebRequest加载图片
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        request.certificateHandler = new BypassCertificateHandler();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 获取加载的图片纹理
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            
            // 创建Sprite并设置为Image组件的sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            imageObject.GetComponent<Image>().sprite = sprite;
            
            // 计算图片的长宽比
            float aspectRatio = (float)texture.width / texture.height;
            Debug.Log("Aspect Ratio: " + aspectRatio+"index: "+index+"j:  "+j+"  "+imageUrl);

            // 获取Image子物体的RectTransform组件
            RectTransform imageRect = imageObject.GetComponent<RectTransform>();

            // 根据长宽比设置Image子物体的高度,保持宽度不变
            float width = imageRect.sizeDelta.x;
            float height = width / aspectRatio;
            imageRect.sizeDelta = new Vector2(width, height);
            if (j == 2)
            {
                GameObject buttonContent3 = GameObject.Find("ButtonContent3");
                GameObject image3 = GameObject.Find("ButtonContent3/Image");
                image3.GetComponent<Image>().sprite = sprite;
                RectTransform rect3 = image3.GetComponent<RectTransform>();
                rect3.sizeDelta = new Vector2(width*5, height*5);
                // 在Buttoncontent的子对象中查找Image组件
                buttonContent3.transform.localPosition = new Vector3(-0.2f, -0.07f, 0.35f);
                
                GameObject buttonContent = GameObject.Find("ButtonContent");
                GameObject image2 = GameObject.Find("ButtonContent/Image");
                image2.GetComponent<Image>().sprite = sprite;
                RectTransform rect2 = image2.GetComponent<RectTransform>();
                rect2.sizeDelta = new Vector2(width, height);
                // 在Buttoncontent的子对象中查找Image组件
                buttonContent.transform.localPosition = new Vector3(0, -0.07f, 0.35f);


                
            }
        }
        else
        {
            Debug.LogError("Failed to load image: " + request.error);
        }
        j++;
    }*/

    private class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // 始终返回true,绕过证书验证
        }
    }
    void Update()
    {
        
    }
    public void starttturl()
    {
        int flag = 0;
        starturl(i,flag);
        i++;
        
    }
    public void starturl(int number,int flag)
    {
        Debug.Log("nextid:::" + nextWebViewId);
        //string url = "https://picss.sunbangyan.cn/2023/11/17/4f080c5d9fbe47edfcb50b79cd19b550.png";
        //string url = speechMap["Figure Two"];
        Debug.Log("url:" + url[number]);
        if (flag == 0)
        {
            loadingurl(url[number],flag);
        }
        else
        {
            loadingurl(tableurl[number],flag);
        }
        
    }
    async void loadingurl(string url,int flag)
    {

        Debug.Log("初始化webview0000:: ");

        getcontent.id++;
        var webViewPrefab = WebViewPrefab.Instantiate(0.5f, 0.25f);

        //GameObjectManager objectManager = webViewPrefab.AddComponent<GameObjectManager>();
        
        GetObjectId getObjectId = webViewPrefab.AddComponent<GetObjectId>();
        getObjectId.setid(getcontent.id);
        GameObjectManager.Instance.PrintObjectInfo();
        MyWebView webView = webViewPrefab.GetComponent<MyWebView>();

        // 为 WebView 分配编号
        webView.webViewId = nextWebViewId;

        webViewPrefab.transform.localScale = new Vector3(0.4f, 0.4f, 0.3f);
        Debug.Log(qRCodesVisualizer.posxyz);
        Vector3 pos = qRCodesVisualizer.posxyz;
        Vector3 newpos=qRCodesVisualizer.posxyz;
        newpos.z = (float)(pos.z - 0.15f);
        if (availablePositionIndices.Count > 0)
        {
            int nextWebViewId = availablePositionIndices[0];
            Debug.Log("位置" + nextWebViewId + "被占用    " + availablePositionIndices);
            availablePositionIndices.RemoveAt(0);

            if (flag == 0)
            {
                Debug.Log("将要执行");
                GameObjectManager.Instance.AddObject("11", ObjectType.Image, webViewPrefab.gameObject,nextWebViewId, somesettings.state);
            }
            else
            {
                GameObjectManager.Instance.AddObject("11", ObjectType.Chart, webViewPrefab.gameObject,nextWebViewId, somesettings.state);
            }

            if (nextWebViewId <= 3)
            {
                //webViewPrefab.transform.eulerAngles = new Vector3(-(nextWebViewId - 1) * 19, 180 - 25, 0);
                if (nextWebViewId == 0)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(19, 180 - 25, 0);
                }
                if (nextWebViewId == 3)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(-19, 180 - 25, 0);
                }
                if (nextWebViewId == 1)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(9, 180 - 25, 0);
                }
                if (nextWebViewId == 2)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(-9, 180 - 25, 0);
                }
                //if (nextWebViewId == 1)
                //{
                //    pos.z = pos.z + 0.02f;
                //}
                //电脑左侧
                newpos.z = (float)(pos.z - 0.15f);
                newpos.x = (float)(pos.x - 0.07);

                newpos.y = (float)(pos.y + 0.15 - 0.1 * nextWebViewId);
            }
            else if (nextWebViewId <= 7)
            {
                if (nextWebViewId == 4)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(19, 180 - 44, 0);
                }
                if (nextWebViewId == 7)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(-19, 180 - 44, 0);
                }
                if (nextWebViewId == 5)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(9, 180 - 44, 0);
                }
                if (nextWebViewId == 6)
                {
                    webViewPrefab.transform.eulerAngles = new Vector3(-9, 180 - 44, 0);
                }

                //if (nextWebViewId == 6)
                //{
                //    pos.z = pos.z + 0.02f;
                //}
                //webViewPrefab.transform.eulerAngles = new Vector3(-(nextWebViewId - 6) * 22, 180 - 44, 0);
                newpos.z = (float)(pos.z - 0.25f);
                newpos.x = (float)(pos.x - 0.25);
                newpos.y = (float)(pos.y + 0.15 - 0.1 * (nextWebViewId - 4));
                if (flag == 1)
                {
                    GameObjectManager.Instance.DeleteObject(webViewPrefab.gameObject);
                }
            }
        }





            //if (nextWebViewId <= 2)
            //{

            //   webViewPrefab.transform.eulerAngles =new Vector3(-(nextWebViewId - 1) * 19, 180-25, 0);

            //    if (nextWebViewId == 1)
            //    {
            //        pos.z = pos.z + 0.02f;
            //    }
            //    //电脑左侧
            //    newpos.z = (float)(pos.z - 0.15f);
            //    newpos.x = (float)(pos.x - 0.07);

            //    newpos.y = (float)(pos.y + 0.1 - 0.15 * nextWebViewId);
            //}  

            //else if (nextWebViewId <= 5)
            //{
            //    if (nextWebViewId == 4)
            //    {
            //        pos.z = pos.z + 0.02f;
            //    }
            //    webViewPrefab.transform.eulerAngles = new Vector3(-(nextWebViewId - 4) * 19, 180 + 25, 0);
            //    newpos.z = (float)(pos.z - 0.15f);
            //    newpos.x = (float)(pos.x + 0.58);
            //    newpos.y = (float)(pos.y + 0.1 - 0.15 * (nextWebViewId-3));
            //}
            //else if (nextWebViewId <= 8)
            //{
            //    if (nextWebViewId == 7)
            //    {
            //        pos.z = pos.z + 0.02f;
            //    }
            //    webViewPrefab.transform.eulerAngles = new Vector3(-(nextWebViewId - 7) * 22, 180 - 44, 0);
            //    newpos.z = (float)(pos.z - 0.25f);
            //    newpos.x = (float)(pos.x - 0.25);
            //    newpos.y = (float)(pos.y + 0.1 - 0.15 * (nextWebViewId-6));
            //}
            //else if (nextWebViewId <= 11)
            //{
            //    if (nextWebViewId == 10)
            //    {
            //        pos.z = pos.z + 0.02f;
            //    }
            //    webViewPrefab.transform.eulerAngles = new Vector3(-(nextWebViewId - 10) * 22, 180 + 44, 0);
            //    newpos.z = (float)(pos.z - 0.25f);
            //    newpos.x = (float)(pos.x +0.58f+ 0.20f);
            //    newpos.y = (float)(pos.y + 0.1 - 0.15 * (nextWebViewId - 9));
            //}
            //string css = "body{border-radius:10px;}";
            webViewPrefab.SetCutoutRect(new Rect(0, 0, 0, 0));
        
        webViewPrefab.transform.localScale = new Vector3(0.4f, 0.4f, 0.3f);
        webViewPrefab.transform.localPosition = newpos;

        
        TextMeshProUGUI textMeshPro = webViewPrefab.transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
        textMeshPro.text =nextWebViewId.ToString();
        textMeshPro.fontSize = 3;
        textMeshPro.transform.localPosition =new Vector3(newpos.x, newpos.y, newpos.z);
        textMeshPro.SetText("Figure One");
        webViewPrefabs.Add(nextWebViewId,webViewPrefab);

        // 递增编号以供下一个 WebView 使用
        nextWebViewId++;

        BoxCollider box = webViewPrefab.GetComponent<BoxCollider>();
        //box.center = new Vector3(0f,-webViewPrefab.transform.localScale.y * 0.075f, 0f);
        //box.size = new Vector3(webViewPrefab.transform.localScale.x, webViewPrefab.transform.localScale.y * 0.15f, 0.02f);
        box.isTrigger = true;
        box.center = new Vector3(0f, -0.125f, 0f);
        box.size = new Vector3(0.5f, 0.25f, 0.02f);
        ObjectManipulator objectManipulator = webViewPrefab.AddComponent<ObjectManipulator>();
        objectManipulator.AllowFarManipulation = false;
        NearInteractionGrabbable nearInteractionGrabbable = webViewPrefab.AddComponent<NearInteractionGrabbable>();
        //BoundingBox boundingBox = webViewPrefab.AddComponent<BoundingBox>();
        //boundingBox.BoundsOverride = box;
        

        //TextMeshProUGUI textMeshPro = webViewPrefab.transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
        //textMeshPro.text = "Figure One";
        //textMeshPro.SetText("Figure One");
        // Position the prefab how we want it
        //webViewPrefab.transform.parent = transform;
        //webViewPrefab.transform.localPosition = new Vector3(0, 0f, 0.5f);
        //webViewPrefab.transform.LookAt(transform);
        // Load a URL once the prefab finishes initializing
        await webViewPrefab.WaitUntilInitialized();
        Debug.Log("初始化webview1111::  ");

        webViewPrefab.WebView.LoadUrl(url);
        Debug.Log("初始化webview2222");

    }
}
