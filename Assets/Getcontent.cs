using Dummiesman;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Vuplex.WebView;
using Unity.VisualScripting;
using QRTracking;
using UnityEngine.Video;
using TMPro;

public class ContentInfo
{
    public string name;
    public string data;
}
public class ContentList
{
    public List<ContentInfo> contents;
}


public class Getcontent : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage image1; // 用于显示图片的 RawImage1
    public RawImage image2; // 用于显示图片的 RawImage2
    public Interactable nextPageButton; // 下一页按钮
    public Interactable previousPageButton; // 上一页按钮
    private int currentPage = 0;
    public int id = 0;
    public int sstate = 1;
    private bool gestureflag = false;
    private bool startgesture = false;
    private bool startdeletegesture = false;
    private bool deletegestureflag = false;
    public Canvas canvas;
    public RawImage rawImagePrefab;
    public List<Texture2D> imagetexs;
    public float timer = 0f;
    public test1 test1;
    public test4 test4;
    string error = string.Empty;

    public ProgressIndicatorOrbsRotator progressIndicator;
    public GameObject rotatingOrbs; // 要隐藏的RotatingOrbs对象
    public TextMeshPro messageText; // 要修改并隐藏的Text对象


    public QRCodesVisualizer qRCodesVisualizer;

    //GameObject loadedObject;
    // Start is called before the first frame update
    void Start()
    {

    }
    private string predictionEndpoint = "http://10.53.18.1:8001/findcontent";

    async void loadingpdfurl(string url)
    {
        Vector3 pos = qRCodesVisualizer.posxyz;
        Debug.Log("pos:::" + pos);

        Debug.Log("打开网站url：" + url);
        var webViewPrefab = WebViewPrefab.Instantiate(0.5f, 0.3f);
        webViewPrefab.transform.localScale = new Vector3(0.6f, 0.6f, 0.3f);
        BoxCollider box = webViewPrefab.AddComponent<BoxCollider>();
        ObjectManipulator objectManipulator = webViewPrefab.AddComponent<ObjectManipulator>();
        NearInteractionGrabbable nearInteractionGrabbable = webViewPrefab.AddComponent<NearInteractionGrabbable>();
        //box.center = new Vector3(0f,-webViewPrefab.transform.localScale.y * 0.075f, 0f);
        //box.size = new Vector3(webViewPrefab.transform.localScale.x, webViewPrefab.transform.localScale.y * 0.15f, 0.02f);
        box.isTrigger = true;
        box.center = new Vector3(0f, -0.25f, 0f);
        box.size = new Vector3(0.5f, 0.1f, 0.02f);
        webViewPrefab.transform.localPosition = new Vector3((float)(pos.x+0.55), (float)(pos.y+0.18), (float)(pos.z - 0.05));
        Debug.Log(webViewPrefab.transform.localPosition);
        await webViewPrefab.WaitUntilInitialized();
        webViewPrefab.WebView.LoadUrl("https://www.freeimg.cn/i/2024/03/12/65f067c620ccb.png");
    }

    public IEnumerator GetPdfContent(string bytesurl)
    {
        Debug.Log("开始执行GetPdfContent!!!");
        progressIndicator.OpenAsync();
        progressIndicator.Message = "Loading...";
        WWWForm webForm = new WWWForm();
        //string referencenum = GetUrlNum();
        webForm. AddField("target_name", bytesurl);
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(predictionEndpoint, webForm))
        {
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                Debug.Log(unityWebRequest.error);
            }
            else
            {
                Debug.Log("处理中...");

                startdeletegesture = true;
                //Debug.Log(unityWebRequest.downloadHandler.text);
                var contentsJson = unityWebRequest.downloadHandler.text;
                contentsJson = contentsJson.Trim('\"');

                Debug.Log(contentsJson+"________"); 
                List<ContentInfo> contents = JsonConvert.DeserializeObject<List<ContentInfo>>(contentsJson);
                Debug.Log("contents.Count:" + contents.Count);
                int flag = 1;
                if (contents.Count == 1)
                {
                    //是图/表/视频

                    //是图
                    if (contents[0].name == "11111")
                    {
                        test1.starturl(int.Parse(contents[0].data),0);
                    }
                    //是表
                    if (contents[0].name == "22222")
                    {
                        test1.starturl(int.Parse(contents[0].data), 1);
                    }
                    //是视频
                    if (contents[0].name == "33333")
                    {
                        test4.startdown();
                    }
                    //id++;
                }
                else if (contents.Count == 3)
                {

                    string obj_str = contents[1].data;
                    string mtl_str = contents[2].data;
                    byte[] objBytes = System.Text.Encoding.UTF8.GetBytes(obj_str);
                    obj_str = obj_str.Trim('\"');
                    //string[] results = receivedString.Split(new string[] { "abcd" }, System.StringSplitOptions.None);

                    // 将 \n 替换为正确的换行符
                    obj_str = obj_str.Replace("\\n", System.Environment.NewLine);
                    mtl_str = mtl_str.Replace("\\n", System.Environment.NewLine);
                    obj_str=obj_str.Substring(2, obj_str.Length - 3);
                    mtl_str=mtl_str.Substring(2, mtl_str.Length - 3);
                    //string[] lines = receivedString.Split('\n');
                    //Debug.Log("lines::"+lines.Length);
                    Debug.Log("obj:::" + obj_str);
                    Debug.Log("mtl:::" + mtl_str);
                    string objFilePath = "Assets/" + contents[0].data +".obj";
                    string mtlFilePath = "Assets/" + contents[0].data +".mtl";
                    File.WriteAllText(mtlFilePath, mtl_str);
                    File.WriteAllText(objFilePath, obj_str);
                    //File.WriteAllLines(objFilePath, lines);
                    Debug.Log("save success!");
                    addObj(objFilePath);
                    id++;

                }
                else if (contents.Count > 3)
                {
                    int i = 0;
                    //是参考文献
                    foreach (ContentInfo content in contents)
                    {
                        if (i != 0)
                        {
                            if (i == 1)
                            {
                                Debug.Log("name:" + content.name);
                                Debug.Log("data:" + content.data);
                                loadingpdfurl(content.data);
                            }
                            else
                            {
                                Debug.Log("name:" + content.name);
                                Debug.Log("data:" + content.data);
                                Debug.Log(content.data.Length);
                                byte[] contentData = System.Convert.FromBase64String(content.data);

                                Debug.Log(content.data.Length);
                                Texture2D tex = new Texture2D(2, 2);
                                tex.LoadImage(contentData);
                                imagetexs.Add(tex);
                                Debug.Log("存到了第" + i + "  个！");
                            }
                            
                        }
                        i++;

                    }

                    //处理参考文献pdf图片
                    GameObject pdfImage = canvas.transform.Find("pdfimage").gameObject; // 替换 "pdfimage" 为你的 pdfimage 对象的名称
                    if (pdfImage != null)
                    {
                        pdfImage.gameObject.SetActive(true);
                        Vector3 pos = qRCodesVisualizer.posxyz;
                        //pdfImage.transform.position = new Vector3((float)(pos.x + 0.35), (float)(pos.y-0.06), (float)(pos.z - 0.05));
                        pdfImage.transform.position = new Vector3((float)(pos.x+0.02), (float)(pos.y + 0.19), (float)(pos.z - 0.05));
                    }

                    GameObjectManager.Instance.AddObject("11", ObjectType.Reference,pdfImage);
                    GetObjectId getObjectId = pdfImage.gameObject.AddComponent<GetObjectId>();
                    getObjectId.setid(id);
                    GameObjectManager.Instance.PrintObjectInfo();

                    gestureflag = true;
                    startgesture = true;
                    ShowImages();
                    id++;
                }


            }
        }

    }
    public void addObj(string objPath)
    {
        GameObject loadedObject;
        if (!File.Exists(objPath))
        {
            error = "File doesn't exist.";
        }
        else
        {
            //if (loadedObject != null)
            //    Destroy(loadedObject);
            loadedObject = new OBJLoader().Load(objPath);
            loadedObject.transform.localPosition = new Vector3(-0.06f, 0.12f, 0.34f);
            loadedObject.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
            Transform defaultTransform = loadedObject.transform.Find("default");
            MeshFilter mf = defaultTransform.GetComponent<MeshFilter>();
            loadedObject.AddComponent<ObjectManipulator>();
            loadedObject.AddComponent<NearInteractionGrabbable>();


            GameObjectManager.Instance.AddObject("model"+"8SQG", ObjectType.Model,loadedObject);
            GetObjectId getObjectId = loadedObject.AddComponent<GetObjectId>();
            getObjectId.setid(id);
            GameObjectManager.Instance.PrintObjectInfo();

            rotatingOrbs.SetActive(false);

            // 修改messageText的文本内容
            progressIndicator.Message = "Loading Success!";

            // 在一秒后隐藏messageText
            Invoke("HideMessageText", 1.5f);
            Bounds bounds = mf.mesh.bounds;
            BoxCollider bc = loadedObject.AddComponent<BoxCollider>();
            bc.size = bounds.size;
            Debug.Log(bc.size);
            error = string.Empty;
        }
    }

    void HideMessageText()
    {
        progressIndicator.CloseAsync();
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        //if (timer > 2f)
        //{
        //    Debug.Log("开始调用");
        //    IsLeftHandGesture();
        //    if (IsLeftHandGesture() == true)
        //    {
        //        Debug.Log("向左翻页");
        //    }
        //    timer = 0f;
        //}
        if (timer > 2f && startgesture == true)
        {
            gestureflag = true;
        }
        if (startgesture == true && gestureflag == true)
        {
            Debug.Log("开始手势识别");
            bool isright = IsRightHandGesture();
            if (isright == true)
            {
                Debug.Log("执行向下翻页手势");
                gestureflag = false; timer = 0f;
                currentPage = Mathf.Clamp(currentPage + 1, 0, imagetexs.Count - 2); // 修改这里，只移动一页
                ShowImages();
            }
            bool isleft = IsLeftHandGesture();
            if (isleft == true)
            {
                Debug.Log("执行向上翻页手势");
                gestureflag = false; timer = 0f;
                currentPage = Mathf.Clamp(currentPage - 1, 0, imagetexs.Count - 2); // 修改这里，只移动一页
                ShowImages();
            }
        }
        /*if (timer > 1f && startdeletegesture == true)
        {
        Debug.Log("update");
            if(startdeletegesture == true)
            {
                deletegestureflag = true;
                Debug.Log("truetruetrue");
            }
                
            
        }
        if (startdeletegesture == true && deletegestureflag == true)
        {
            bool flag = IsDeleteHandGesture();
            if (flag == true)
            {
                StartDelete();
                deletegestureflag = false;
                timer = 0f;
            }
            
        }*/
        if (timer > 1f)
        {
            /*bool flag = IsDeleteHandGesture();
            if (flag == true)
            {
                StartDelete();
                deletegestureflag = false;
                timer = 0f;
            }*/
        }
    }

    public void close()
    {
        GameObject pdfImage = canvas.transform.Find("pdfimage").gameObject; // 替换 "pdfimage" 为你的 pdfimage 对象的名称
        startgesture = false;
        pdfImage.gameObject.SetActive(false);

    }
    void ShowImages()
    {
        Debug.Log("开始");
        // 显示当前页和下一页的图片
        image1.texture = imagetexs[currentPage];

        // 如果存在下一页，则显示下一页图片；否则，保持 image2 显示空
        if (currentPage + 1 < imagetexs.Count)
        {
            image2.texture = imagetexs[currentPage + 1];
        }
        else
        {
            image2.texture = null;
        }

        // 更新按钮的可点击状态
        UpdateButtonInteractivity();
    }
    void UpdateButtonInteractivity()
    {
        // 判断是否存在下一页和上一页
        bool hasNextPage = currentPage + 1 < imagetexs.Count;
        bool hasPreviousPage = currentPage - 1 >= 0;

        nextPageButton.gameObject.SetActive(hasNextPage);
        previousPageButton.gameObject.SetActive(hasPreviousPage);

        // 设置按钮的可点击状态
        //nextPageButton.interactable = hasNextPage;
        //previousPageButton.interactable = hasPreviousPage;
    }
    public void NextPage()
    {
        // 切换到下一页
        currentPage = Mathf.Clamp(currentPage + 1, 0, imagetexs.Count - 2); // 修改这里，只移动一页
        ShowImages();
    }

    public void PreviousPage()
    {
        // 切换到上一页
        currentPage = Mathf.Clamp(currentPage - 1, 0, imagetexs.Count - 2); // 修改这里，只移动一页
        ShowImages();
    }


    async void loadingurl(string url)
    {
        var webViewPrefab = WebViewPrefab.Instantiate(0.5f, 0.3f);
        webViewPrefab.transform.localScale = new Vector3(0.6f, 0.6f, 0.3f);
        BoxCollider box = webViewPrefab.AddComponent<BoxCollider>();
        //box.center = new Vector3(0f,-webViewPrefab.transform.localScale.y * 0.075f, 0f);
        //box.size = new Vector3(webViewPrefab.transform.localScale.x, webViewPrefab.transform.localScale.y * 0.15f, 0.02f);
        box.isTrigger = true;
        box.center = new Vector3(0f, -0.15f, 0f);
        box.size = new Vector3(0.5f, 0.3f, 0.02f);
        webViewPrefab.transform.localPosition = new Vector3(0, 0, (float)(0.5));
        await webViewPrefab.WaitUntilInitialized();
        webViewPrefab.WebView.LoadUrl(url);
    }
    // 判断手是否为右手翻页手势
    private bool IsRightHandGesture()
    {
        MixedRealityPose indexTipPose, indexDistalPose, middleTipPose, middleDistalPose, ringTipPose, ringDistalPose, palmPose, thumbTipPose, thumbDistalPose;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palmPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexDistalJoint, Handedness.Right, out indexDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out middleTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleDistalJoint, Handedness.Right, out middleDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out ringTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingDistalJoint, Handedness.Right, out ringDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumbTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Right, out thumbDistalPose);
        bool flag = IsFistGesture(indexTipPose, indexDistalPose, middleTipPose, middleDistalPose, ringTipPose, ringDistalPose, palmPose);
        if (flag == true)
        {
            float horizontalThreshold = 30f;
            Debug.Log("是握拳");
            Debug.Log(Vector3.Angle((thumbDistalPose.Position - thumbTipPose.Position), Vector3.right));
            // 判断大拇指向左的角度是否在误差范围内
            return Vector3.Angle((thumbDistalPose.Position - thumbTipPose.Position), Vector3.right) >= 120f;
        }
        return false;
    }

    // 判断手是否为左手翻页手势
    private bool IsLeftHandGesture()
    {
        MixedRealityPose indexTipPose, indexDistalPose, middleTipPose, middleDistalPose, ringTipPose, ringDistalPose, palmPose, thumbTipPose, thumbDistalPose;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out palmPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out indexTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexDistalJoint, Handedness.Left, out indexDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Left, out middleTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleDistalJoint, Handedness.Left, out middleDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Left, out ringTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingDistalJoint, Handedness.Left, out ringDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Left, out thumbTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Left, out thumbDistalPose);
        bool flag = IsFistGesture(indexTipPose, indexDistalPose, middleTipPose, middleDistalPose, ringTipPose, ringDistalPose, palmPose);
        if (flag == true)
        {
            float horizontalThreshold = 30f;
            Debug.Log("是握拳");
            Debug.Log(Vector3.Angle((thumbDistalPose.Position - thumbTipPose.Position), Vector3.left));
            // 判断大拇指向左的角度是否在误差范围内
            return Vector3.Angle((thumbDistalPose.Position - thumbTipPose.Position), Vector3.left) >= 120f;
        }
        return false;
    }

    private bool IsDeleteHandGesture()
    {
        MixedRealityPose thumbProximalPose, thumbDistalPose, indexKnucklePose, indexMiddlePose, indexTipPose, indexDistalPose, middleTipPose, middleDistalPose, ringTipPose, ringDistalPose, palmPose, thumbTipPose;

        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbProximalJoint, Handedness.Right, out thumbProximalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Right, out thumbDistalPose);

        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out indexKnucklePose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexMiddleJoint, Handedness.Right, out indexMiddlePose);

        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palmPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexDistalJoint, Handedness.Right, out indexDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out middleTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleDistalJoint, Handedness.Right, out middleDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out ringTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingDistalJoint, Handedness.Right, out ringDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumbTipPose);


        bool flag = IsHalfFistGesture(middleTipPose, middleDistalPose, ringTipPose, ringDistalPose, palmPose);
        if (flag == true)
        {
            float angle = Vector3.Angle((thumbProximalPose.Position - thumbDistalPose.Position), (indexKnucklePose.Position - indexMiddlePose.Position));
            Debug.Log("angle :  "+angle);
            if (angle >= 0 && angle <= 40f)
            {
                float angle2= Vector3.Angle((indexMiddlePose.Position - indexKnucklePose.Position), (indexMiddlePose.Position - indexDistalPose.Position));
                Debug.Log("angle2 :  " + angle2);
                if (angle2>=140f)
                {
                    Debug.Log("判断为删除手势");
                    return true;
                    
                }
            }
            float horizontalThreshold = 30f;
            


        }
        if (flag == false)
        {
            Debug.Log("不是");
        }
        return false;
    }

    private void StartDelete()
    {
        Debug.Log("开始执行删除操作！");
        List<ObjectInfo> objectsList = GameObjectManager.Instance.objects;
        float minDistance = float.MaxValue;
        MixedRealityPose indexTipPose;
        GameObject nearestObject=null;
        int i = 1;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTipPose);
        foreach (ObjectInfo obj in objectsList)
        {
            float distance = Vector3.Distance(indexTipPose.Position, obj.GameObject.transform.position);
            Debug.Log("物体"+i+ "的距离:"+distance);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestObject = obj.GameObject;
            }
            i++;
        }
        if (nearestObject != null)
        {
            GameObjectManager.Instance.DeleteObject(nearestObject);
        }

    }

    private bool IsFistGesture(MixedRealityPose indexTipPose, MixedRealityPose indexDistalPose, MixedRealityPose middleTipPose, MixedRealityPose middleDistalPose, MixedRealityPose ringTipPose, MixedRealityPose ringDistalPose, MixedRealityPose palmPose)
    {
        float angleIndex = Vector3.Angle((indexDistalPose.Position - indexTipPose.Position), palmPose.Up);
        float angleMiddle = Vector3.Angle((middleDistalPose.Position - middleTipPose.Position), palmPose.Up);
        float angleRing = Vector3.Angle((ringDistalPose.Position - ringTipPose.Position), palmPose.Up);
        Debug.Log(angleIndex + "  " + angleMiddle + " " + angleRing);

        // 设定夹角的误差范围
        float angleThreshold = 60f;

        // 判断夹角是否在误差范围内
        return Mathf.Abs(angleIndex - 180f) <= 65f &&
               Mathf.Abs(angleMiddle - 180f) <= 50f &&
               Mathf.Abs(angleRing - 180f) <= 50f;
        // 判断夹角是否在误差范围内
    }

    private bool IsHalfFistGesture(MixedRealityPose middleTipPose, MixedRealityPose middleDistalPose, MixedRealityPose ringTipPose, MixedRealityPose ringDistalPose, MixedRealityPose palmPose)
    {
        float angleMiddle = Vector3.Angle((middleDistalPose.Position - middleTipPose.Position), palmPose.Up);
        float angleRing = Vector3.Angle((ringDistalPose.Position - ringTipPose.Position), palmPose.Up);
        

        // 设定夹角的误差范围
        float angleThreshold = 60f;
        Debug.Log(Mathf.Abs(angleMiddle - 180f) + " ::::::::::::  " + Mathf.Abs(angleRing - 180f));

        // 判断夹角是否在误差范围内
        return Mathf.Abs(angleMiddle - 180f) <= 80f &&
               Mathf.Abs(angleRing - 180f) <= 80f;
        // 判断夹角是否在误差范围内
    }

}
