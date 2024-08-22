using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;

[System.Serializable]
public class Result
{
    public int region;
    public float area;
    public string text;
    public float jaccard_similarity;
}

[System.Serializable]
public class ResultsResponse
{
    public Result[] results;
}
// ����һ��������ʾ Python ���ص� JSON �ṹ
[System.Serializable]
public class PythonResponse
{
    public ResponseResult[] results;
}

[System.Serializable]
public class ResponseResult
{
    public string success;
    public string paper_title;
    public FileContent file_content;
}

[System.Serializable]
public class FileContent
{
    public Chart[] charts;
    public Reference[] references;
}

[System.Serializable]
public class Chart
{
    public string id;
    public string url;
}

[System.Serializable]
public class Reference
{
    public string id;
    public Link[] links;
}

[System.Serializable]
public class Link
{
    public string url;
}
public class CaptureImage : MonoBehaviour
{
    // Start is called before the first frame update
    private PhotoCapture photoCaptureObject = null;
    private Texture2D targetTexture = null;
    private string pythonServerUrl = "http://10.53.18.1:8001/gettitle";
    public TextMeshPro textMeshPro;
    public List<string> urls = new List<string>();
    public List<string> tableurls = new List<string>();
    public List<string> refurls = new List<string>();
    public ProgressIndicatorDemo progressIndicatorDemo;
    public TextMeshPro AudioText;
    public ProgressIndicatorOrbsRotator progressIndicator;
    public GameObject rotatingOrbs; // Ҫ���ص�RotatingOrbs����
    public TextMeshPro messageText; // Ҫ�޸Ĳ����ص�Text����

    public TextMeshPro warningText;
    public TextMeshPro countdownText; // ������ʾ����ʱ���ֵ�TextMeshPro���
    public float countdownDuration = 3f; // ����ʱ����ʱ��(��)

    void Start()
    {
        Debug.Log(pythonServerUrl);
        progressIndicatorDemo.OnClickSceneLoad();
        
    }
    private void SendImageToPython(byte[] imageData)
    {
        StartCoroutine(SendImageRequest(imageData));
    }
    public void start1()
    {
        
        StartCoroutine(SendImageRequest2());
    }
    private IEnumerator SendImageRequest2()
    {
        progressIndicator.OpenAsync();
        progressIndicator.Message = "Loading...";
        Debug.Log("��������");
        WWWForm form = new WWWForm();
        //form.AddBinaryData("image", imageData, "image.jpg", "image/jpeg");
        form.AddField("target_name", "1");
        using (UnityWebRequest www = UnityWebRequest.Post(pythonServerUrl, form))
        {
            yield return www.SendWebRequest();
            Debug.Log(pythonServerUrl);
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Image sent to Python successfully!");
                AudioText.SetText("Success!");

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
                    rotatingOrbs.SetActive(false);

                    // �޸�messageText���ı�����
                    progressIndicator.Message = "Loading Success!";

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
                textMeshPro.SetText("Error: " + www.error);
            }
        }
    }
    //byte[] imageData
    private IEnumerator SendImageRequest(byte[]imageData)
    {
        progressIndicator.OpenAsync();
        progressIndicator.Message = "Loading...";
        Debug.Log("��������");
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "image.jpg", "image/jpeg");
        //form.AddField("target_name", "1");
        using (UnityWebRequest www = UnityWebRequest.Post(pythonServerUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Image sent to Python successfully!");
                textMeshPro.SetText("Success!");

                // ����Python���ص�JSON����
                string jsonResponse = www.downloadHandler.text;
                PythonResponse response = JsonUtility.FromJson<PythonResponse>(jsonResponse);
                Debug.Log("response.results[0].success:::"+ response.results[0].success);
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
                        urls.Add(chartUrl);
                        if (chartId[0].Equals("g"))
                        {
                            urls.Add(chartUrl);
                        }
                        else
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
                    
                     Debug.Log(urls);
                    rotatingOrbs.SetActive(false);

                    // �޸�messageText���ı�����
                    progressIndicator.Message = "Loading Success!";

                    // ��һ�������messageText
                    Invoke("HideMessageText", 15f);

                }
                else
                {
                    rotatingOrbs.SetActive(false);

                    // �޸�messageText���ı�����
                    progressIndicator.Message = "Failed to take a photo";

                    // ��һ�������messageText
                    Invoke("HideMessageText", 15f);
                    Debug.Log("Python returned success: no");
                    // ��������Ӵ���successΪ"no"���߼�
                }
            }
            else
            {
                rotatingOrbs.SetActive(false);

                // �޸�messageText���ı�����
                progressIndicator.Message = "Failed to take a photo";

                // ��һ�������messageText
                Invoke("HideMessageText", 15f);
                Debug.LogError("Error sending image to Python: " + www.error);
                textMeshPro.SetText("Error: " + www.error);
            }
        }
    }
    void HideMessageText()
    {
        progressIndicator.CloseAsync();
    }
    public void TakePhoto()
    {
        var cameraMode = WebCam.Mode;
        if (cameraMode == WebCamMode.None)
        {
            //PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
            StartCoroutine(CountdownAndCapturePhoto());
        }
        else
            Debug.LogError("��ǰ��������ã�");
    }
    private IEnumerator CountdownAndCapturePhoto()
    {
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        warningText.gameObject.SetActive(false);
        // ���ص���ʱ�ı�
        countdownText.gameObject.SetActive(true);

        // ��ʼ����ʱ
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // ����ʱ����,��ʾ"����!"
        countdownText.text = "Take a picture!";
        yield return new WaitForSeconds(0.5f);

        // ���ص���ʱ�ı�
        countdownText.gameObject.SetActive(false);

        // ������Ƭ
        PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
    }
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
        CameraParameters cameraParm = new CameraParameters();
        cameraParm.hologramOpacity = 0.0f;
        cameraParm.cameraResolutionWidth = cameraResolution.width;
        cameraParm.cameraResolutionHeight = cameraResolution.height;
        cameraParm.pixelFormat = CapturePixelFormat.BGRA32;
        captureObject.StartPhotoModeAsync(cameraParm, OnPhotoModeStarted);
    }
    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("�޷���������ģʽ!");
        }
    }
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            //Ӧ��Texture2D
            string filename = string.Format(@"Image{0}_1.jpg", Time.time);
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            AudioText.SetText(filePath);
            StartCoroutine(saveTexture2DtoFile(targetTexture, filePath));

        }
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }
    private IEnumerator saveTexture2DtoFile(Texture2D texture, string path)
    {
        //�ȴ���Ⱦ�߳̽���  
        yield return new WaitForEndOfFrame();
        byte[] textureData = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(path, textureData);
        OnCapturedPhotoToDisk(textureData);

    }
    void OnCapturedPhotoToDisk(byte[] imageData)
    {

        // �����ｫͼƬ���ݷ��͵�Python������
        SendImageToPython(imageData);

    }
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}
