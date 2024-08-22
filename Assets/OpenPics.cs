using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Utilities;
using Microsoft.MixedReality.Toolkit.Rendering;
using Microsoft.MixedReality.Toolkit.UI;
using QRTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Vuplex.WebView;

public class OpenPics : MonoBehaviour
{
    string[] url = {
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g001",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g002",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g003",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g004",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g005",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g006",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g007",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.g008",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0065894.t001",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g002",
        "https://journals.plos.org/plosone/article/figure/image?size=inline&id=10.1371/journal.pone.0240769.g003",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g004",
        "https://journals.plos.org/plosone/article/figure/image?size=medium&id=10.1371/journal.pone.0197543.g005"

         };
    public ComputerHandler computerHandler;
    public TextMeshPro textMeshPro;
    int number = 0;//临时
    public JSONParser JSONParser;
    public Getcontent getcontent;
    public QRCodesVisualizer qRCodesVisualizer;
    //九个可用位置
    public List<int> availablePositionIndices = new List<int> {0, 1, 2, 3, 4, 5, 6, 7,8};
    public CaptureImage captureImage;
    Vector3[] positionxyz=new Vector3[9];
    Vector2[] positionsize = new Vector2[9];
    int[] flagifavailable = new int[9];
    public float maxSize = 600f; // 图片的最大尺寸
    //public GameObject[] ImageContent;
    public GameObject ImageContent;
    public GameObject SnapGameObject;
    public SpeechRecognitionManager speechRecognitionManager;
    public somesettings somesettings;
    public List<Vector3> snapPositions = new List<Vector3>();
    ManipulationHandler[] manipulationHandlers;
    int nnn = 1;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("openpics  :"+somesettings.state);
        Debug.Log("computerhandler:" + computerHandler.ssstate);

        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void startref()
    {
        addreferenceAsync("1");
    }
    public void addreferenceAsync(string num)
    {
        Vector3 newpos = qRCodesVisualizer.posxyz;
        var webViewPrefab = WebViewPrefab.Instantiate(0.32f, 0.24f);
        Debug.Log("11111");
        webViewPrefab.transform.localPosition = new Vector3(newpos.x + 0.5f + 0.12f, newpos.y + 0.1f - 0.12f, newpos.z - 0.15f);
        webViewPrefab.transform.eulerAngles = new Vector3(-2, 180 + 42, -2);

        BoxCollider box = webViewPrefab.GetComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = new Vector3(0f, -0.025f, 0f);
        box.size = new Vector3(0.32f, 0.05f, 0.02f);
        Debug.Log("222222");
        ObjectManipulator objectManipulator = webViewPrefab.AddComponent<ObjectManipulator>();
        //objectManipulator.AllowFarManipulation = false;
        NearInteractionGrabbable nearInteractionGrabbable = webViewPrefab.AddComponent<NearInteractionGrabbable>();

        StartCoroutine(InitializeWebView(webViewPrefab, num));
    }

    private IEnumerator InitializeWebView(WebViewPrefab webViewPrefab, string num)
    {
        yield return webViewPrefab.WaitUntilInitialized();

        //int number = int.Parse(num);
        int number = 1;
        if (num.Equals("one"))
        {
            number = 1;
        }
        else if (num.Equals("two"))
        {
            number = 2;
        }
        else if (num.Equals("three"))
        {
            number = 3;
        }
        else if (num.Equals("four"))
        {
            number = 4;
        }
        else if (num.Equals("five"))
        {
            number = 5;
        }
        else if (num.Equals("six"))
        {
            number = 6;
        }
        else if (num.Equals("seven"))
        {
            number = 7;
        }
        else if (num.Equals("eight"))
        {
            number = 8;
        }
        else if (num.Equals("nine"))
        {
            number = 9;
        }
        else if (num.Equals("ten"))
        {
            number = 10;
        }
        //Debug.Log(captureImage.refurls[number - 1]);
        Debug.Log(computerHandler.ssstate);
        if (computerHandler.ssstate == 1)
        {
            Debug.Log("computerHandler.ssstate: " + computerHandler.ssstate+"captureImage.refurls[number - 1]:" + captureImage.refurls[number - 1]);
            webViewPrefab.WebView.LoadUrl(captureImage.refurls[number - 1]);

            string newid = "ref" + number;
            GameObjectManager.Instance.AddObject(newid, ObjectType.Reference, webViewPrefab.gameObject, captureImage.refurls[number - 1], computerHandler.ssstate);
            GameObjectManager.Instance.PrintObjectInfo();
        }
        else
        {
            Debug.Log("computerHandler.ssstate: " + computerHandler.ssstate + "  computerHandler.refurls[number - 1]:" + computerHandler.refurls[number - 1]);
            webViewPrefab.WebView.LoadUrl(computerHandler.refurls[number - 1]);

            string newid = "ref" + number;
            GameObjectManager.Instance.AddObject(newid, ObjectType.Reference, webViewPrefab.gameObject, computerHandler.refurls[number - 1], computerHandler.ssstate);
            GameObjectManager.Instance.PrintObjectInfo();
        }




        Debug.Log("初始化webview12222::  ");
        Debug.Log("33333");
    }
    public void start1()
    {
        if (nnn <= 8)
        {
             availablePosition("figure", Convert.ToString(nnn));
        }
        else
        {
            availablePosition("table", Convert.ToString(1));
        }
       
        nnn++;
    }
    public void availablePosition(string type,string num)
    {
        int number = 0;
        if (num.Equals("one")||num.Equals("1"))
        {
            number = 1;
        }
        else if (num.Equals("two"))
        {
            number = 2;
        }
        else if (num.Equals("three"))
        {
            number = 3;
        }
        else if (num.Equals("four"))
        {
            number = 4;
        }
        else if (num.Equals("five"))
        {
            number = 5;
        }
        else if (num.Equals("six"))
        {
            number = 6;
        }
        else if (num.Equals("seven"))
        {
            number = 7;
        }
        else if (num.Equals("eight"))
        {
            number = 8;
        }
        else if (num.Equals("nine"))
        {
            number = 9;
        }
        else if (num.Equals("ten"))
        {
            number = 10;
        }
        if (availablePositionIndices.Count > 0)
        {
            int positionid = availablePositionIndices[0];
            Debug.Log("位置" + positionid + "被占用    " + availablePositionIndices);
            availablePositionIndices.RemoveAt(0);

            
            
            StartCoroutine(addpic(positionid,type,number));

        }
        else
        {
            Debug.Log("暂无可用位置"); 
        }
    }
    public void modifylocation()
    {

    }
    public void addvalue(Vector3 newpos)
    {
        float x = newpos.x;
        float y = newpos.y;
        float z = newpos.z;
        snapPositions.Add(new Vector3(x - 0.09f, y + 0.09f, z - 0.15f));
        snapPositions.Add(new Vector3(x - 0.09f, y + 0.09f - (float)(600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f));
        snapPositions.Add(new Vector3(x - 0.09f, y + 0.09f - (float)(600f * 0.0003 + 600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f));
        snapPositions.Add(new Vector3(x - 0.24f, y + 0.09f, z - 0.15f - 0.11f));
        snapPositions.Add(new Vector3(x - 0.24f, y + 0.09f - (float)(600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f - 0.11f));
        snapPositions.Add(new Vector3(x - 0.24f, y + 0.09f - (float)(600f * 0.0003 + 600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f - 0.11f));
        snapPositions.Add(new Vector3(x + 0.09f, (float)(600f * 0.00015), z - 0.15f));
        snapPositions.Add(new Vector3(x + (float)(600f * 0.00015) + (float)(600f * 0.00015) + 0.1f, y + (float)(600f * 0.00015), z - 0.15f));
        snapPositions.Add(new Vector3(x + (float)(600f * 0.0003 + 600f * 0.00015) + (float)(600f * 0.00015) + 0.1f - 0.09f, y + (float)(600f * 0.00015), z - 0.15f));
        Debug.Log(snapPositions[0]);
        //snapPositions[0] = new Vector3(x - 0.09f, y + 0.09f, z - 0.15f);
        //snapPositions[1] = new Vector3(x - 0.09f, y + 0.09f - (float)(600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f);
        //snapPositions[2] = new Vector3(x - 0.09f, y + 0.09f - (float)(600f * 0.0003 + 600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f);
        //snapPositions[3] = new Vector3(x - 0.24f, y + 0.09f, z - 0.15f-0.11f);
        //snapPositions[4] = new Vector3(x - 0.24f, y + 0.09f - (float)(600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f - 0.11f);
        //snapPositions[5] = new Vector3(x - 0.24f, y + 0.09f - (float)(600f * 0.0003 + 600f * 0.00015) - (float)(600f * 0.00015), z - 0.15f - 0.11f);
        //snapPositions[6] = new Vector3(x + 0.09f, (float)(600f * 0.00015), z - 0.15f);
        //snapPositions[7] = new Vector3(x+(float)(600f * 0.00015) + (float)(600f * 0.00015) + 0.1f,y+ (float)(600f * 0.00015), z - 0.15f);
        //snapPositions[8] = new Vector3(x+(float)(600f * 0.0003 + 600f * 0.00015) + (float)(600f * 0.00015) + 0.1f - 0.09f,y+ (float)(600f * 0.00015), z - 0.15f);
    }
    public IEnumerator addpic(int positionid,string type,int num)
    {
        Debug.Log("进来了addpic");
        textMeshPro.SetText("!!!   "+number);
        string newurl = "";
        //加载图片代码
        if (type.Equals("figure"))
        {
            newurl = captureImage.urls[num-1];
            Debug.Log("figure");
        }
        else if (type.Equals("table"))
        {
            newurl = captureImage.tableurls[num-1];
        }
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(newurl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            //Debug.LogError("无法加载图片: " + captureImage.urls[number]);
            textMeshPro.SetText("无法加载图片: ");
        }
        else
        {
            //textMeshPro.SetText("NUMBER:" + number + " " + url[number]);
            number++;
            Vector3 newpos = qRCodesVisualizer.posxyz;
            if (number == 1)
            {
                addvalue(newpos);
                Debug.Log("开始addvalue");
            }
            //getcontent.id++;//物体管理类id
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            
            GameObject imageContent = Instantiate(ImageContent, Vector3.zero, Quaternion.identity);
            //GameObject imageContent = ImageContent[positionid];
            GameObject imageObject = imageContent.transform.Find("Image").gameObject;
            imageObject.SetActive(true);
            imageObject.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            ObjectManipulator manipulationHandler = imageObject.GetComponent<ObjectManipulator>();
            Debug.Log(manipulationHandler);
            // 订阅manipulation事件
            manipulationHandler.OnManipulationStarted.AddListener(OnManipulationStarted);
            manipulationHandler.OnManipulationEnded.AddListener(OnManipulationEnded);

       

            BoxCollider collider = imageObject.AddComponent<BoxCollider>();

            //imageObject.AddComponent<ObjectManipulator>();
            //imageObject.AddComponent<NearInteractionGrabbable>();
            if (texture == null)
            {
                Debug.Log("无法打开");
            }
            Debug.Log($"id:  {name}   图片长：" + texture.width + "    图片宽：" + texture.height);

            //被占用过
            if (flagifavailable[positionid] == 1)
            {
                Debug.Log($"位置{positionid}被占用过");
                Debug.Log($"位置{positionid}的大小：positionsize[positionid]");
                
                texture = AdjustImageSize((int)positionsize[positionid].x, (int)positionsize[positionid].y,texture);
                
                collider.size = new Vector3((float)(texture.width * 0.01), (float)(texture.height * 0.01), 0.01f);
                //collider2.size = new Vector3((float)(texture.width * 0.001*0.3), (float)(texture.height * 0.001 * 0.3), (float)(0.001f*0.3));
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
                GameObjectManager.Instance.AddObject(type+num, ObjectType.Chart, imageContent, positionid,somesettings.state);
                GameObjectManager.Instance.PrintObjectInfo();
            }
            else
            {
                Debug.Log($"位置{positionid}未！！！被占用过");

                GameObject snapgameobject = Instantiate(SnapGameObject, Vector3.zero, Quaternion.identity);
                BoxCollider snapcollider = snapgameobject.GetComponent<BoxCollider>();
                

                flagifavailable[positionid] = 1;
                texture = AdjustImageSize((int)600f, (int)600f, texture);
                Vector2 newwd;newwd.x = 600f;newwd.y = 600f;
                positionsize[positionid] = newwd;

                collider.size = new Vector3((float)(texture.width * 0.01), (float)(texture.height * 0.01), 0.01f);
                //collider2.size = new Vector3((float)(texture.width * 0.001 * 0.3), (float)(texture.height * 0.001 * 0.3), (float)(0.001f * 0.3));

                RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(texture.width / 100, texture.height / 100);
                MeshRenderer renderer = imageObject.GetComponent<MeshRenderer>();

                
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
                
                Vector3 newvector3=CalculateImagePosition(positionid,texture.width,texture.height, newpos);
                Vector3 vector3;
                vector3.x = newvector3.x;vector3.y = newvector3.y;vector3.z = newvector3.z - 0.15f;
                positionxyz[positionid] = vector3;
                snapgameobject.transform.localPosition = vector3;
                
                imageContent.transform.localPosition =vector3;
                if (positionid < 3)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9+positionid*9,  - 25, 0);
                    snapgameobject.transform.eulerAngles = new Vector3(-9 + positionid * 9, -25, 0);
                }
                else if (positionid < 6)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9 + (positionid-3) * 9, - 44, 0);
                    snapgameobject.transform.eulerAngles = new Vector3(-9 + (positionid - 3) * 9, -44, 0);
                }
                else if (positionid < 9)
                {
                    imageObject.transform.eulerAngles = new Vector3(-9, 0, 0);
                    snapgameobject.transform.eulerAngles = new Vector3(-9, 0, 0);
                }
                //BoxCollider collider2 = imageContent.AddComponent<BoxCollider>();
                //collider2.size = new Vector3((float)(texture.width * 0.001 * 0.3), (float)(texture.height * 0.001 * 0.3), (float)(0.001f * 0.3));
                GameObjectManager.Instance.AddObject(type + num, ObjectType.Chart, imageContent, positionid,somesettings.state);
                GameObjectManager.Instance.PrintObjectInfo();
            }
        }

        
        //


    }
    //调整图片尺寸
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
    private static Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        if (source != null)
        {
            // 创建临时的RenderTexture
            RenderTexture renderTex = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            // 复制source的纹理到RenderTexture里
            Graphics.Blit(source, renderTex);
            // 开启当前RenderTexture激活状态
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            // 创建修改后的纹理，保持与源纹理相同压缩格式
            Texture2D resizedTexture = new Texture2D(width, height, source.format, false);
            // 读取像素到创建的纹理中
            resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            // 应用修改到GPU上
            resizedTexture.Apply();
            // 停止当前RenderTexture工作
            RenderTexture.active = previous;
            // 释放内存
            RenderTexture.ReleaseTemporary(renderTex);
            return resizedTexture;
        }
        else
        {
            return null;
        }
    }

    //生成图片位置，未被占用过时调用
    private Vector3 CalculateImagePosition(int positionid,int width, int height,Vector3 computerPosition)
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
                //y =y+0.09f- (float)(positionsize[positionid - 1].y*0.00015)-(float)(height*0.00015);
                y = y + 0.09f - (float)(600f * 0.00015) - (float)(600f * 0.00015);
                break;
            case 2:
                x -= 0.09f;
                //y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.0003 + positionsize[positionid - 2].y * 0.00015) - (float)(height * 0.00015);
                y = y + 0.09f - (float)(600f * 0.0003 + 600f * 0.00015) - (float)(600f * 0.00015);
                break;
            case 3:
                x -= 0.24f;
                y = y + 0.09f;
                z = z - 0.11f;
                break;
            case 4:
                x -= 0.24f;
                //y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.00015) - (float)(height * 0.00015);
                y = y + 0.09f - (float)(600f * 0.00015) - (float)(600f * 0.00015);
                z = z - 0.11f;
                break;
            case 5:
                x -= 0.24f;
                //y = y + 0.09f - (float)(positionsize[positionid - 1].y * 0.0003 + positionsize[positionid - 2].y * 0.00015) - (float)(height * 0.00015);
                y = y + 0.09f - (float)(600f * 0.0003 + 600f * 0.00015) - (float)(600f * 0.00015);
                z = z - 0.11f;
                break;
            case 6:
                x += 0.09f;
                y += (float)(600f* 0.00015);
                break;
            case 7:
                //x += (float)(positionsize[positionid - 1].x * 0.00015)+(float)(width * 0.00015)+ 0.1f;
                x += (float)(600f * 0.00015) + (float)(600f * 0.00015) + 0.1f;
                //y += (float)(height*0.00015);
                y += (float)(600f * 0.00015);
                break;
            case 8:
                //x += (float)(positionsize[positionid - 1].x * 0.0003 + positionsize[positionid - 2].x * 0.00015)+(float)(width * 0.00015)+ 0.1f;
                x += (float)(600f * 0.0003 + 600f * 0.00015) + (float)(600f * 0.00015) + 0.1f;
                //y += (float)(height* 0.00015);
                y += (float)(600f * 0.00015);
                break;
        }

        return new Vector3(x, y,z);
    }

    private GameObject draggedObject;
    public void test()
    {
        Debug.Log("1在拖动！！！");
    }
    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        //Debug.Log("正在拖动");

        // 存储正在拖动的对象
        draggedObject = eventData.ManipulationSource;
        for(int i = 0; i < snapPositions.Count; i++)
        {
            Vector3 objectPosition = eventData.Pointer.Result.Details.Object.transform.position;

            // 计算物体与参考点之间的距离
            float distance = Vector3.Distance(objectPosition, snapPositions[i]);
            //Debug.Log($"distance{i}:   {distance}");
        }

        //Debug.Log(draggedObject);
    }
    
    public void OnManipulationEnded(ManipulationEventData eventData)
    {
        // 拖动结束后清除对象引用
        draggedObject = null;
        //Debug.Log("停止移动");
    }
}
