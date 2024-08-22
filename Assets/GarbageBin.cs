using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class GarbageBin : MonoBehaviour
{
    public GameObject DialogSmall;
    public test1 test11;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("112233 " + other.gameObject.name);
        Transform parentTransform = other.transform.parent;
        // 检测到碰撞
        GameObjectManager.Instance.DeleteObject(parentTransform.gameObject);
       
    }

    public void OpenDialogSmall(MyWebView webView)
    {
        Dialog myDialog = Dialog.Open(DialogSmall, DialogButtonType.Yes | DialogButtonType.No, "", "是否要关闭此页面", false);
        if (myDialog != null)
        {
            myDialog.OnClosed += (result) => OnClosedDialogEvent(result, webView);
        }
    }

    private void OnClosedDialogEvent(DialogResult obj, MyWebView webView)
    {
        if (obj.Result == DialogButtonType.Yes)
        {
            Debug.Log(obj.Result);
            // Call CloseWebView when the user selects Yes
            webView.CloseWebView();
        }
    }
}
