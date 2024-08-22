using UnityEngine;

public class MyWebView : MonoBehaviour
{
    public int webViewId;  // WebView 的唯一标识符
    public bool isBeingDragged = false;  // 标志位，表示 WebView 是否正在被拖拽

    // 其他成员和方法...

    void Update()
    {
        // 在 Update 方法中检测拖拽状态，可以根据具体情况调整
        if (isBeingDragged)
        {
            // 处理 WebView 正在被拖拽时的逻辑
        }
    }

    // 开始拖拽时调用
    public void StartDrag()
    {
        isBeingDragged = true;
        // 其他开始拖拽时的逻辑
    }

    // 停止拖拽时调用
    public void StopDrag()
    {
        isBeingDragged = false;
        // 其他停止拖拽时的逻辑
    }

    // 关闭 WebView 的方法
    public void CloseWebView()
    {
        // 关闭 WebView 的逻辑，可以是 Destroy 或其他方法
        Destroy(gameObject);
    }
}
