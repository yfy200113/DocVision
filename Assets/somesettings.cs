using QRTracking;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class somesettings : MonoBehaviour
{
    public int state = 1;
    // Start is called before the first frame update
    public Getcontent getcontent;
    public test1 test11;
    void Start()
    {
        state = 1;
        mMicrophoneRecode = Microphone.Start(null, true, 999, 16000);
    }
    public string[] url = {
        "https://picdl.sunbangyan.cn/2023/12/07/8c30e411929ee022b00fc5a203994391.jpeg",
        "https://www.freeimg.cn/i/2023/12/08/65726f2b0b6b2.png",
        "https://www.freeimg.cn/i/2023/12/08/65726fc42aa90.png",
        "https://picdl.sunbangyan.cn/2023/12/07/8c30e411929ee022b00fc5a203994391.jpeg",
        "https://www.freeimg.cn/i/2023/12/08/65726f2b0b6b2.png",
        "https://www.freeimg.cn/i/2023/12/08/65726fc42aa90.png",
            "https://picdl.sunbangyan.cn/2023/12/07/8c30e411929ee022b00fc5a203994391.jpeg",
        "https://www.freeimg.cn/i/2023/12/08/65726f2b0b6b2.png",
        "https://www.freeimg.cn/i/2023/12/08/65726fc42aa90.png"
         }
        ;
    public QRCodesVisualizer qRCodesVisualizer;

    public TextMeshPro ResultText;
    public TextMeshPro AudioText;
    public TextMeshPro AudioText2;
    private AudioClip clip;

    private AudioClip mMicrophoneRecode;  //录制的音频
    private static int VOLUME_DATA_LENGTH = 128;    //录制的声音长度
    private string mDeviceName;           //设备名称
    private float volume;

    private byte[] bytes;
    private bool recording=false;
    //private VoiceResultAnalysis vra = new VoiceResultAnalysis();
    private string VoiceResult;
    public SpeechRecognitionManager SpeechRecognitionManager;

    private float startRecordingTime;
    private const float RECORDING_DURATION = 3f;
    private const float RECORDING_DELAY = 0.5f;
    public GameObject computerObject;
    public void settings()
    {
        

        GameObject scrollingcollection = GameObject.Find("ScrollingCollection");
        
        GameObject scrollingpics = scrollingcollection.transform.Find("ScrollingPics").gameObject;
        GameObject speechparentobj = GameObject.Find("FSpeechPanel");
        GameObject speechpanel = speechparentobj.transform.Find("SpeechPanel").gameObject;
        speechpanel.SetActive(true);

        GameObject garbageparentobj = GameObject.Find("GarbageCanvas");
        GameObject garbageobj = garbageparentobj.transform.Find("RawImage").gameObject;

        Vector3 pos = qRCodesVisualizer.posxyz;
        Vector3 newpos = qRCodesVisualizer.posxyz;
        newpos.x = (float)(pos.x + 0.3);
        newpos.y = (float)(pos.y-0.33);
        newpos.z = (float)(pos.z - 0.10);
        Debug.Log("newpos:" + newpos);

        Vector3 newpos2 = qRCodesVisualizer.posxyz;
        newpos2.x = (float)(pos.x + 0.5);
        newpos2.y = (float)(pos.y - 0.31);
        newpos2.z = (float)(pos.z - 0.10);

        Quaternion currentRotation = scrollingcollection.transform.rotation;
        currentRotation.x = Mathf.Deg2Rad * 28;
        Vector3 picspos = qRCodesVisualizer.posxyz;
        picspos.x = (float)(pos.x);
        picspos.y = (float)(pos.y - 0.32);
        picspos.z = (float)(pos.z - 0.18);
        scrollingcollection.transform.position = picspos;
        scrollingcollection.transform.eulerAngles = new Vector3(50, 0, 0);
        scrollingpics.SetActive(false);

        garbageobj.transform.position = newpos2;
        speechpanel.transform.position = newpos;


        //设置电脑桌面碰撞体
        //GameObject newObject = new GameObject("computer");


        //ComputerHandler computerHandler=newObject.AddComponent<ComputerHandler>();
        //Rigidbody rigibody=newObject.AddComponent<Rigidbody>();
        //rigibody.useGravity = false;
        //newObject.transform.position = new Vector3(pos.x+0.02f,pos.y-0.0125f,pos.z);
        //BoxCollider collider = newObject.AddComponent<BoxCollider>();
        //collider.isTrigger = true;
        //collider.size= new Vector3(0.4f, 0.25f, 0.01f);

        //computerObject = GameObject.Find("computer");
        computerObject.SetActive(true);
        computerObject.transform.position = new Vector3(pos.x + 0.26f, pos.y - 0.155f, pos.z);
        computerObject.transform.eulerAngles = new Vector3(11, 0, 0);


    }

    // Update is called once per frame
    void Update()
    {
        volume = GetMaxVolume();

        if (!recording && volume > 0.5f)
        {
            // 开始录制音频
            startRecordingTime = Time.time;
            recording = true;
        }

        if (recording && Time.time - startRecordingTime >= RECORDING_DURATION)
        {
            // 停止录制音频并保存
            recording = false;
            SaveAudioClip();
        }
    }

    private void SaveAudioClip()
    {
        int position = Microphone.GetPosition(null);
        int startSample = (int)((position - (RECORDING_DELAY + RECORDING_DURATION) * mMicrophoneRecode.frequency) * mMicrophoneRecode.channels);
        int endSample = (int)((position - RECORDING_DELAY * mMicrophoneRecode.frequency) * mMicrophoneRecode.channels);

        // 确保起始位置不小于0
        startSample = Mathf.Max(0, startSample);

        int sampleCount = endSample - startSample;
        float[] samples = new float[sampleCount];

        // 从持续录制的音频中截取指定区间的样本数据
        mMicrophoneRecode.GetData(samples, startSample);

        AudioClip clip = AudioClip.Create("RecordedAudio", sampleCount / mMicrophoneRecode.channels, mMicrophoneRecode.channels, mMicrophoneRecode.frequency, false);
        clip.SetData(samples, 0);

        bytes = EncodeAsWAV(samples, mMicrophoneRecode.frequency, mMicrophoneRecode.channels);
        AudioText2.SetText(bytes.Length.ToString());
        Debug.Log("Application.persistentDataPath:::" + Application.persistentDataPath);
        File.WriteAllBytes(Application.persistentDataPath + "/test11.wav", bytes);

        StartCoroutine(SpeechRecognitionManager.GetSpeechRecognitionResult(bytes));

        AudioText.SetText(Application.persistentDataPath);
    }



    /// <summary>
    /// 获取最大的音量
    /// </summary>
    /// 
    /// <returns>
    /// 音量大小
    /// </returns>
    private float GetMaxVolume()
    {
        float maxVolume = 0f;

        //用于储存一段时间内的音频信息
        float[] volumeData = new float[VOLUME_DATA_LENGTH];

        int offset;
        //获取录制的音频的开头位置
        offset = Microphone.GetPosition(null) - VOLUME_DATA_LENGTH + 1;

        if (offset < 0)
        {
            //Debug.Log("offset:"+offset);
            return 0f;
        }

        //获取数据
        mMicrophoneRecode.GetData(volumeData, offset);

        //解析数据
        for (int i = 0; i < VOLUME_DATA_LENGTH; i++)
        {
            float tempVolume = volumeData[i];
            if (tempVolume > maxVolume)
            {
                maxVolume = tempVolume;
            }
        }

        return maxVolume;
    }

    public void StartRecording()
    {
        clip = Microphone.Start(null, false, 10, 16000);
        recording = true;
    }
    public void stop2()
    {
        Debug.Log("jinlaile11111111111111111111111111");
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * mMicrophoneRecode.channels];
        mMicrophoneRecode.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, mMicrophoneRecode.frequency, mMicrophoneRecode.channels);
        AudioText2.SetText(bytes.Length.ToString());
        //Debug.Log("bytes:::"+ Convert.ToBase64String(bytes));
       
        Debug.Log("Application.datapath:::" + Application.dataPath);
        File.WriteAllBytes(Application.dataPath + "/testtttt.wav", bytes);
    }

    public void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        AudioText2.SetText(bytes.Length.ToString());
        //Debug.Log("bytes:::"+ Convert.ToBase64String(bytes));
        recording = false;
        Debug.Log("Application.persistentDataPath:::" + Application.persistentDataPath);
        File.WriteAllBytes(Application.persistentDataPath + "/test11.wav", bytes);
        //StartCoroutine(getcontent.GetPdfContent(Application.persistentDataPath + "/test11.wav"));

        StartCoroutine(SpeechRecognitionManager.GetSpeechRecognitionResult(bytes));

        AudioText.SetText(Application.persistentDataPath);
        //getResult(bytes);
    }

    

    public void CloseAllWebViews()
    {
        foreach(var kv in test11.webViewPrefabs)
        {
            if (kv.Value != null)
            {
                kv.Value.gameObject.SetActive(false);
            }
            
        }
    }


    public void OpenAllWebViews()
    {
        foreach (var kv in test11.webViewPrefabs)
        {
            if (kv.Value != null)
            {
                kv.Value.gameObject.SetActive(true);
            }

        }
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}


class ResultData
{
    public int err_no;
    public string err_msg;
    public string[] result;
}