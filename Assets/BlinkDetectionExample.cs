using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.EyeTracking;
using System.IO;
using System.Text;
using TMPro;

public class BlinkDetectionExample : MonoBehaviour
{
    private EyeGazeTrackerWatcher eyeGazeTrackerWatcher;
    private EyeGazeTracker eyeGazeTracker;
    public TextMeshPro textMeshPro;
    private string filePath;
    private List<string> dataLines = new List<string>();

    async void Start()
    {
        // ��ʼ�� EyeGazeTrackerWatcher ��������
        eyeGazeTrackerWatcher = new EyeGazeTrackerWatcher();
        eyeGazeTrackerWatcher.EyeGazeTrackerAdded += OnEyeGazeTrackerAdded;
        eyeGazeTrackerWatcher.EyeGazeTrackerRemoved += OnEyeGazeTrackerRemoved;
        await eyeGazeTrackerWatcher.StartAsync();

        // �����ļ�·��
        filePath = Application.persistentDataPath + "/EyeOpennessData.csv";
        dataLines.Add("Timestamp,LeftEyeOpenness,RightEyeOpenness");

        if (eyeGazeTracker != null)
        {
            textMeshPro.SetText("111111");
        }
        else
        {
            textMeshPro.SetText("22222");
        }
    }

    private void OnEyeGazeTrackerAdded(object sender, EyeGazeTracker tracker)
    {
        // �����۶�׷��������¼�
        eyeGazeTracker = tracker;
        Debug.Log("EyeGazeTracker added.");
    }

    private void OnEyeGazeTrackerRemoved(object sender, EyeGazeTracker tracker)
    {
        // �����۶�׷�����Ƴ��¼�
        if (eyeGazeTracker == tracker)
        {
            eyeGazeTracker = null;
            Debug.Log("EyeGazeTracker removed.");
        }
    }

    void Update()
    {
        if (eyeGazeTracker != null)
        {
            // ��ȡ��ǰʱ���
            var timestamp = DateTime.Now;

            // ���Ի�ȡ�۶�׷������
            var reading = eyeGazeTracker.TryGetReadingAtTimestamp(timestamp);
            if (reading != null)
            {
                // ��ȡ���ۺ����۵Ŀ���״̬
                if (reading.TryGetLeftEyeOpenness(out float leftEyeOpenness) &&
                    reading.TryGetRightEyeOpenness(out float rightEyeOpenness))
                {
                    // ��������ӵ��������б���
                    string dataLine = $"{timestamp},{leftEyeOpenness},{rightEyeOpenness}";
                    dataLines.Add(dataLine);

                    // �ж��Ƿ�գ��
                    bool isBlinking = leftEyeOpenness < 0.2f && rightEyeOpenness < 0.2f;

                    if (isBlinking)
                    {
                        Debug.Log("User is blinking.");
                    }
                    else
                    {
                        Debug.Log("User is not blinking.");
                    }
                }
            }
        }
    }
    public void save()
    {
        // ���������б�ת��Ϊ�ֽ����鲢д���ļ�
        string dataString = string.Join("\n", dataLines);
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);
        File.WriteAllBytes(filePath, dataBytes);
        textMeshPro.SetText(dataLines.Count.ToString());
        Debug.Log($"Count:{dataLines.Count}  Eye openness data saved to: {filePath}");
        
    }
    void OnDestroy()
    {
        Debug.Log("������");
        // ֹͣ EyeGazeTrackerWatcher ��ȡ���¼�����
        if (eyeGazeTrackerWatcher != null)
        {
            eyeGazeTrackerWatcher.Stop();
            eyeGazeTrackerWatcher.EyeGazeTrackerAdded -= OnEyeGazeTrackerAdded;
            eyeGazeTrackerWatcher.EyeGazeTrackerRemoved -= OnEyeGazeTrackerRemoved;
        }

        // ���������б�ת��Ϊ�ֽ����鲢д���ļ�
        string dataString = string.Join("\n", dataLines);
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);
        File.WriteAllBytes(filePath, dataBytes);
        Debug.Log($"Eye openness data saved to: {filePath}");
    }
}