using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;
using QRTracking;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using Vuplex.WebView;
using Unity.VisualScripting;

public class Download : MonoBehaviour
{
    string urlPath = "https://test-unity.oss-cn-beijing.aliyuncs.com/demo_speedup_withvoice.mp4?Expires=1710334918&OSSAccessKeyId=TMP.3KkJGRB3oZPtXMYkuQrVQ3yUb7bW7zFik1u6wWHLgBfCy2SPVyKEzuoqYr3y6RWNQwZvUhvTbsrzZhV1gqhA859eCv4cxR&Signature=mrvyRUqF2OygsHRbyxb1gF2YaPc%3D";//资源网络路径  
    string file_SaveUrl;//资源保路径  
    FileInfo file;
    private bool down;
    public QRCodesVisualizer qRCodesVisualizer;
    public Getcontent getcontent;

    public VideoPlayer _videoPlayer;
    //private void Awake()
    //{
    //    _videoPlayer = GetComponent<VideoPlayer>();
    //}
    void Start()
    {

    }
    IEnumerator Startts()
    {
        Debug.Log("开始执行");
        _videoPlayer = GetComponent<VideoPlayer>();

        //GameObjectManager objectManager = _videoPlayer.AddComponent<GameObjectManager>();

        GameObjectManager.Instance.AddObject("11", ObjectType.Video,_videoPlayer.gameObject);
        GetObjectId getObjectId = _videoPlayer.AddComponent<GetObjectId>();
        getObjectId.setid(getcontent.id);
        GameObjectManager.Instance.PrintObjectInfo();

        string path = Application.dataPath;
        Debug.Log(path);
        file_SaveUrl = path + "/myvideo2.mp4";

        file = new FileInfo(file_SaveUrl);
        AdjustPosition();
        if (File.Exists(file_SaveUrl))
        {
            PlayVideo();
        }
        else
        {
            yield return StartCoroutine(DownFile());
        }
    }
    public void startdown()
    {
        getcontent.id++;
        StartCoroutine(Startts());
    }
    IEnumerator DownFile()
    {

        _videoPlayer.Stop();

        WWW www = new WWW(urlPath);
        yield return www;

        File.WriteAllBytes(file_SaveUrl, www.bytes);

        Debug.Log("Download Completed");

        yield return new WaitForSeconds(1f);

        PlayVideo();

    }
    void AdjustPosition()
    {
        // 获取当前物体的 Transform 组件
        Transform objTransform = transform;
        Vector3 pos = qRCodesVisualizer.posxyz;
        Debug.Log("原点坐标："+pos);
        // 设置新的位置
        //Vector3 newPosition = new Vector3((float)(pos.x+0.18), (float)(pos.y + 0.085), (float)(pos.z - 0.05));
        Vector3 newPosition = new Vector3((float)(pos.x + 0.75), (float)(pos.y - 0.06), (float)(pos.z - 0.05));
        Debug.Log("新坐标:" + newPosition);
        objTransform.position = newPosition;

        // 如果视频是在 UI 元素上播放，你可能需要更新 Canvas 的位置
        // 如果视频在 3D 空间中播放，还需考虑摄像机视野等因素

        // 在这里可以添加其他你需要的逻辑
    }
    void PlayVideo()
    {

        _videoPlayer.prepareCompleted += OnPrepareCompleted;
        _videoPlayer.errorReceived += OnError;

        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = file_SaveUrl;
        _videoPlayer.Play();
    }

    void OnPrepareCompleted(VideoPlayer vp)
    {
        Debug.Log("Prepare Completed");
    }

    void OnError(VideoPlayer vp, string error)
    {
        Debug.LogError("Play Error: " + error);
    }
}
