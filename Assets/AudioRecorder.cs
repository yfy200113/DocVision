using UnityEngine;
using System.Collections;
using System.IO;

public class AudioRecorder : MonoBehaviour
{
    public float threshold = 0.1f;
    public float recordDuration = 2f;

    private AudioClip recordedClip;
    private bool isRecording = false;

    void Update()
    {
        //if (!isRecording && MicrophoneLevel() > threshold)
        //{
        //    StartCoroutine(RecordAudio());
        //}
    }

    IEnumerator RecordAudio()
    {
        Debug.Log("Startrecongize");
        isRecording = true;
        recordedClip = Microphone.Start(null, false, (int)recordDuration, 44100);
        yield return new WaitForSeconds(recordDuration);
        Microphone.End(null);
        isRecording = false;

        float[] samples = new float[recordedClip.samples];
        recordedClip.GetData(samples, 0);
        byte[] wavBytes = EncodeAsWAV(samples, recordedClip.frequency, recordedClip.channels);

        File.WriteAllBytes(Application.persistentDataPath + "/test.wav", wavBytes);
        // 在这里将wavBytes发送到Python端进行语音识别
        // 例如，可以使用Unity的UnityWebRequest将数据发送到Python服务器
        // 然后等待Python端返回识别结果，并在Unity中进行处理

        Destroy(recordedClip);
    }

    float MicrophoneLevel()
    {
        float levelMax = 0;
        float[] waveData = new float[1024];
        int micPosition = Microphone.GetPosition(null) - (1024 + 1);
        //Debug.Log("micPosition:"+micPosition);
        if (micPosition < 0) return 0;
        recordedClip.GetData(waveData, micPosition);
        for (int i = 0; i < 1024; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        Debug.Log(Mathf.Sqrt(Mathf.Sqrt(levelMax)));
        return Mathf.Sqrt(Mathf.Sqrt(levelMax));
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