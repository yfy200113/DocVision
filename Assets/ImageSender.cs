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
                // ����Python����������Ӧ,�����Ҫ�Ļ�
            }
            else
            {
                Debug.Log("Error sending image to Python: " + www.error);
            }
        }
    }

    // ��������ɺ���ô˷�������ͼƬ����
    void OnCapturedPhotoToDisk(byte[] imageData)
    {
        
        // �����ｫͼƬ���ݷ��͵�Python������
        SendImageToPython(imageData);
       
    }
}