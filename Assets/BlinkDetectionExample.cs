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
        // 初始化 EyeGazeTrackerWatcher 并启动它
        eyeGazeTrackerWatcher = new EyeGazeTrackerWatcher();
        eyeGazeTrackerWatcher.EyeGazeTrackerAdded += OnEyeGazeTrackerAdded;
        eyeGazeTrackerWatcher.EyeGazeTrackerRemoved += OnEyeGazeTrackerRemoved;
        await eyeGazeTrackerWatcher.StartAsync();

        // 创建文件路径
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
        // 处理眼动追踪器添加事件
        eyeGazeTracker = tracker;
        Debug.Log("EyeGazeTracker added.");
    }

    private void OnEyeGazeTrackerRemoved(object sender, EyeGazeTracker tracker)
    {
        // 处理眼动追踪器移除事件
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
            // 获取当前时间戳
            var timestamp = DateTime.Now;

            // 尝试获取眼动追踪数据
            var reading = eyeGazeTracker.TryGetReadingAtTimestamp(timestamp);
            if (reading != null)
            {
                // 获取左眼和右眼的开闭状态
                if (reading.TryGetLeftEyeOpenness(out float leftEyeOpenness) &&
                    reading.TryGetRightEyeOpenness(out float rightEyeOpenness))
                {
                    // 将数据添加到数据行列表中
                    string dataLine = $"{timestamp},{leftEyeOpenness},{rightEyeOpenness}";
                    dataLines.Add(dataLine);

                    // 判断是否眨眼
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
        // 将数据行列表转换为字节数组并写入文件
        string dataString = string.Join("\n", dataLines);
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);
        File.WriteAllBytes(filePath, dataBytes);
        textMeshPro.SetText(dataLines.Count.ToString());
        Debug.Log($"Count:{dataLines.Count}  Eye openness data saved to: {filePath}");
        
    }
    void OnDestroy()
    {
        Debug.Log("被销毁");
        // 停止 EyeGazeTrackerWatcher 并取消事件订阅
        if (eyeGazeTrackerWatcher != null)
        {
            eyeGazeTrackerWatcher.Stop();
            eyeGazeTrackerWatcher.EyeGazeTrackerAdded -= OnEyeGazeTrackerAdded;
            eyeGazeTrackerWatcher.EyeGazeTrackerRemoved -= OnEyeGazeTrackerRemoved;
        }

        // 将数据行列表转换为字节数组并写入文件
        string dataString = string.Join("\n", dataLines);
        byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);
        File.WriteAllBytes(filePath, dataBytes);
        Debug.Log($"Eye openness data saved to: {filePath}");
    }
}