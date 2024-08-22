using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using Newtonsoft.Json;
using UnityEngine.XR;
using Vuplex.WebView;
using Unity.VisualScripting;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.Input;
using static UnityEngine.XR.OpenXR.Features.Interactions.PalmPoseInteraction;
using QRTracking;

public class ImageInfo
{
    public string filename;
    public string data;
}
public class ImageList
{
    public List<ImageInfo> images;
}
public class Test3 : MonoBehaviour
{
    public QRCodesVisualizer qRCodesVisualizer;
    public RawImage image1; // 用于显示图片的 RawImage1
    public RawImage image2; // 用于显示图片的 RawImage2
    public Interactable nextPageButton; // 下一页按钮
    public Interactable previousPageButton; // 上一页按钮
    private int currentPage = 0;
    private bool gestureflag = false;
    private bool startgesture = false;
    public Canvas canvas;
    public RawImage rawImagePrefab;
    public List<Texture2D> imagetexs;
    public float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }
    private string predictionEndpoint = "http://192.168.1.10:8880/crawlreference";
    private string predictionEndpoint2 = "http://192.168.1.10:8880/getreferenceurl";
    // Update is called once per frame
    private string GetUrlNum()
    {
        string number = "4";
        return number;
    }
    void Update()
    {
        timer+=Time.deltaTime;
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
        if (timer > 2f&&startgesture==true)
        {
            gestureflag = true;
        }
        if (startgesture==true&&gestureflag == true)
        {
            Debug.Log("开始手势识别");
            bool isright = IsRightHandGesture();
            if (isright == true)
            {
                Debug.Log("执行向下翻页手势");
                gestureflag = false;timer = 0f;
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
    }
    public void startcrawlreference()
    {
        StartCoroutine(GetPdfCrawl());
        //StartCoroutine(GetPdfUrl());
    }
    public IEnumerator GetPdfUrl()
    {
        Debug.Log("start!!!"); int i = 0;
        WWWForm webForm = new WWWForm();
        string referencenum = GetUrlNum();
        webForm.AddField("target_name", referencenum);
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(predictionEndpoint2, webForm))
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
                Debug.Log(unityWebRequest.downloadHandler.text);
                string url = unityWebRequest.downloadHandler.text;
                url = url.Trim('\"');
                Debug.Log(url);
                loadingurl(url);

            }
        }
    }
    public IEnumerator GetPdfCrawl()
    {
        Debug.Log("start!!!"); 
        int i = 0;
        WWWForm webForm = new WWWForm();
        string referencenum = GetUrlNum();
        webForm.AddField("target_name", referencenum);
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
                Debug.Log(unityWebRequest.downloadHandler.text);
                var imagesJson = unityWebRequest.downloadHandler.text;
                imagesJson = imagesJson.Trim('\"');
         
                Debug.Log(imagesJson);
                List<ImageInfo> images = JsonConvert.DeserializeObject<List<ImageInfo>>(imagesJson);
             
                int flag = 1;
                foreach (ImageInfo image in images)
                {
                    Debug.Log("Filename:" + image.filename);
                    Debug.Log("data:" + image.data);
                    Debug.Log(image.data.Length);
                    byte[] imageData = System.Convert.FromBase64String(image.data);
                  
                    Debug.Log(image.data.Length);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageData);
                    imagetexs.Add(tex);
                    Debug.Log("存到了第" + i+"  个！");
                    //RawImage rawImage = Instantiate(rawImagePrefab);
                    //rawImage.name = "RawImage" + i;


                    //// 设置父对象为Canvas
                    //rawImage.transform.SetParent(canvas.transform);

                    //rawImage.texture = tex;
                    //// 设置RawImage大小
                    //rawImage.SetNativeSize();
                    //float scale_factor = 0.0002f;

                    //// 获取原始图像的 RectTransform 组件
                    //RectTransform rectTransform = rawImage.rectTransform;

                    //// 获取原始宽度和高度
                    //float original_width = rectTransform.rect.width;
                    //float original_height = rectTransform.rect.height;

                    //// 计算新的宽度和高度
                    //float new_width = original_width * scale_factor;
                    //float new_height = original_height * scale_factor;

                    //// 设置新的宽度和高度
                    //rectTransform.sizeDelta = new Vector2(new_width, new_height);
                    //Transform rawImageTransform = rawImage.GetComponent<Transform>();

                    //// 设置新的世界坐标
                    //if (i < 4)
                    //{
                    //    rawImageTransform.position = new Vector3((float)(-0.3 + i * 0.12), (float)(0.2), (float)(0.4));
                    //}
                    //else if (i < 8)
                    //{
                    //    rawImageTransform.position = new Vector3((float)(-0.3 + (i - 4) * 0.12), 0, (float)(0.4));
                    //}
                    //else if (i < 12)
                    //{
                    //    rawImageTransform.position = new Vector3((float)(-0.3 + (i - 8) * 0.12), (float)(-0.2), (float)(0.4));
                    //}
                    ////rawImage.rectTransform.anchoredPosition = new Vector3((float)(i * 10), 0,(float)(0.8));
                    //BoxCollider boxCollider = rawImage.gameObject.AddComponent<BoxCollider>();
                    //boxCollider.size = new Vector3(new_width, new_height, (float)(0.0001));
                    i++;
                }
                GameObject pdfImage = canvas.transform.Find("pdfimage").gameObject; // 替换 "pdfimage" 为你的 pdfimage 对象的名称
                if (pdfImage != null)
                {
                    pdfImage.gameObject.SetActive(true);
                    Vector3 pos = qRCodesVisualizer.posxyz;
                    pdfImage.transform.localPosition = new Vector3((float)(pos.x +20), (float)(pos.y+8), (float)(pos.z - 0.1));
                }
                gestureflag = true;
                startgesture = true;
                ShowImages();
            
            }
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
    // 判断手是否为左手翻页手势
    private bool IsRightHandGesture()
    {
        MixedRealityPose indexTipPose,indexDistalPose, middleTipPose,middleDistalPose, ringTipPose,ringDistalPose,palmPose,thumbTipPose,thumbDistalPose;
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palmPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexDistalJoint, Handedness.Right, out indexDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out middleTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleDistalJoint, Handedness.Right, out middleDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out ringTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingDistalJoint, Handedness.Right, out ringDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out thumbTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbDistalJoint, Handedness.Right, out thumbDistalPose);
        bool flag=IsFistGesture(indexTipPose, indexDistalPose,middleTipPose,middleDistalPose, ringTipPose,ringDistalPose, palmPose);
        if (flag == true)
        {
            float horizontalThreshold = 30f;
            Debug.Log("是握拳");
            Debug.Log(Vector3.Angle((thumbDistalPose.Position-thumbTipPose.Position), Vector3.right));
            // 判断大拇指向左的角度是否在误差范围内
            return Vector3.Angle((thumbDistalPose.Position - thumbTipPose.Position), Vector3.right) >= 120f;
        }
        return false;
    }

    // 判断手是否为右手翻页手势
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

    private bool IsFistGesture(MixedRealityPose indexTipPose, MixedRealityPose indexDistalPose, MixedRealityPose middleTipPose, MixedRealityPose middleDistalPose, MixedRealityPose ringTipPose, MixedRealityPose ringDistalPose, MixedRealityPose palmPose)
    {
        float angleIndex = Vector3.Angle((indexDistalPose.Position-indexTipPose.Position), palmPose.Up);
        float angleMiddle = Vector3.Angle((middleDistalPose.Position-middleTipPose.Position), palmPose.Up);
        float angleRing = Vector3.Angle((ringDistalPose.Position-ringTipPose.Position), palmPose.Up);
        Debug.Log(angleIndex + "  " + angleMiddle + " " + angleRing);

        // 设定夹角的误差范围
        float angleThreshold = 60f;

        // 判断夹角是否在误差范围内
        return Mathf.Abs(angleIndex - 180f) <= 65f &&
               Mathf.Abs(angleMiddle - 180f) <= 50f &&
               Mathf.Abs(angleRing - 180f) <= 50f;
        // 判断夹角是否在误差范围内
        
    }


}
