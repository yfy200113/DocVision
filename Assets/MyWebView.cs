using UnityEngine;

public class MyWebView : MonoBehaviour
{
    public int webViewId;  // WebView ��Ψһ��ʶ��
    public bool isBeingDragged = false;  // ��־λ����ʾ WebView �Ƿ����ڱ���ק

    // ������Ա�ͷ���...

    void Update()
    {
        // �� Update �����м����ק״̬�����Ը��ݾ����������
        if (isBeingDragged)
        {
            // ���� WebView ���ڱ���קʱ���߼�
        }
    }

    // ��ʼ��קʱ����
    public void StartDrag()
    {
        isBeingDragged = true;
        // ������ʼ��קʱ���߼�
    }

    // ֹͣ��קʱ����
    public void StopDrag()
    {
        isBeingDragged = false;
        // ����ֹͣ��קʱ���߼�
    }

    // �ر� WebView �ķ���
    public void CloseWebView()
    {
        // �ر� WebView ���߼��������� Destroy ����������
        Destroy(gameObject);
    }
}
