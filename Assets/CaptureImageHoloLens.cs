using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.XR.WSA.Input;

using Microsoft.MixedReality.OpenXR;
using UnityEngine.Windows.WebCam;
using System.Collections;

public class CaptureImageHoloLens : MonoBehaviour
{
    private PhotoCapture photoCaptureObject = null;
    private GestureRecognizer gestureRecognizer;

    void Start()
    {
        // ��������ʶ����
        
    }

    public void OnTapped()
    {
        // ����⵽�������ʱ,��ʼ��׽ͼƬ
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        // ����PhotoCapture����
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        // �ȴ�PhotoCapture���󴴽����
        yield return new WaitUntil(() => photoCaptureObject != null);

        // ��������ͷ����
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        CameraParameters cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        // ��ʼ��׽ͼƬ
        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
    }

    void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            // ��׽ͼƬ
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // ����һ���յ�Texture2D���洢�����ͼ������
            Texture2D capturedTexture = new Texture2D(2, 2);

            // �������ͼ�������ϴ���Texture2D��
            photoCaptureFrame.UploadImageDataToTexture(capturedTexture);

            // ��Texture2D����ΪJPG��ʽ���ֽ�����
            byte[] jpgBytes = capturedTexture.EncodeToJPG();

            // ��JPG�ֽ����鱣�浽����
            string filename = string.Format(@"C:\Users\{0}\Desktop\CapturedImage.jpg", System.Environment.UserName);
            File.WriteAllBytes(filename, jpgBytes);
            Debug.Log("Captured image saved to " + filename);
        }
        else
        {
            Debug.LogError("Failed to capture photo!");
        }

        // ֹͣPhotoCapture
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}