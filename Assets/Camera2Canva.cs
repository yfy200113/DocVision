using UnityEngine;
using UnityEngine.UI;

public class Camera2Canva : MonoBehaviour
{
    public Camera cameraToProject; // 指定需要投影的摄像头
    public RawImage rawImage; // UI面板，用于显示摄像头图像
    private RenderTexture renderTexture;

    void Start()
    {
        // 创建RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        rawImage.texture = cameraToProject.targetTexture;
    }

    void OnDestroy()
    {
        // 清理，在不需要时释放RenderTexture
        cameraToProject.targetTexture = null;
        renderTexture.Release();
        Destroy(renderTexture);
    }
}
