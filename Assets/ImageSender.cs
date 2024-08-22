using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;

public class ImageSender : MonoBehaviour
{
    private string pythonServerUrl = "http://localhost:8001/findcontent";

    private void SendImageToPython(byte[] imageData)
    {
        StartCoroutine(SendImageRequest(imageData));
    }

    private IEnumerator SendImageRequest(byte[] imageData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageData, "image.jpg", "image/jpeg");

        using (UnityWebRequest www = UnityWebRequest.Post(pythonServerUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Image sent to Python successfully!");
                // 处理Python服务器的响应,如果需要的话
            }
            else
            {
                Debug.Log("Error sending image to Python: " + www.error);
            }
        }
    }

    // 在拍照完成后调用此方法发送图片数据
    void OnCapturedPhotoToDisk(byte[] imageData)
    {
        
        // 在这里将图片数据发送到Python服务器
        SendImageToPython(imageData);
       
    }
}