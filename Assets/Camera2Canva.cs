using UnityEngine;
using UnityEngine.UI;

public class Camera2Canva : MonoBehaviour
{
    public Camera cameraToProject; // ָ����ҪͶӰ������ͷ
    public RawImage rawImage; // UI��壬������ʾ����ͷͼ��
    private RenderTexture renderTexture;

    void Start()
    {
        // ����RenderTexture
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        rawImage.texture = cameraToProject.targetTexture;
    }

    void OnDestroy()
    {
        // �����ڲ���Ҫʱ�ͷ�RenderTexture
        cameraToProject.targetTexture = null;
        renderTexture.Release();
        Destroy(renderTexture);
    }
}
