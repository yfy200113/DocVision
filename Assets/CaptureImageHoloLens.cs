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
        // 创建手势识别器
        
    }

    public void OnTapped()
    {
        // 当检测到点击手势时,开始捕捉图片
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        // 创建PhotoCapture对象
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        // 等待PhotoCapture对象创建完成
        yield return new WaitUntil(() => photoCaptureObject != null);

        // 配置摄像头参数
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        CameraParameters cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        // 开始捕捉图片
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
            // 捕捉图片
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
            // 创建一个空的Texture2D来存储捕获的图像数据
            Texture2D capturedTexture = new Texture2D(2, 2);

            // 将捕获的图像数据上传到Texture2D中
            photoCaptureFrame.UploadImageDataToTexture(capturedTexture);

            // 将Texture2D编码为JPG格式的字节数组
            byte[] jpgBytes = capturedTexture.EncodeToJPG();

            // 将JPG字节数组保存到桌面
            string filename = string.Format(@"C:\Users\{0}\Desktop\CapturedImage.jpg", System.Environment.UserName);
            File.WriteAllBytes(filename, jpgBytes);
            Debug.Log("Captured image saved to " + filename);
        }
        else
        {
            Debug.LogError("Failed to capture photo!");
        }

        // 停止PhotoCapture
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}