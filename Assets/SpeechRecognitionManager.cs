using Dummiesman;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Vuplex.WebView;
using Unity.VisualScripting;
using QRTracking;
using UnityEngine.Video;
using TMPro;

public class SpeechRecognitionManager : MonoBehaviour
{
    public TextMeshPro textMeshPro;
    public string predictionEndpoint = "http://10.53.18.1:8001/recognitiongetinfo";
    public string predictionEndpoint2 = "http://10.53.18.1.196:8001/recognitiongetinfo";
    public AudioClip recordedClip;
    public int id=0;
    public GameObject rotatingOrbs;
    public ProgressIndicatorOrbsRotator progressIndicator;

    private string error = string.Empty;

    public OpenPics openPics;
    public string audioFilePath = "C:\\Users\\yfy\\Downloads\\figure 1.wav";

    private void Start()
    {
        // ������������Ѿ�ͨ��ĳ�ַ�ʽ¼��������������洢��recordedClip��
        //StartCoroutine(GetSpeechRecognitionResult(recordedClip));
        Debug.Log(predictionEndpoint);
        Debug.Log(predictionEndpoint2);
    }

    public IEnumerator GetSpeechRecognitionResult(byte[] audioData)
    {
        Debug.Log("��ʼִ��GetSpeechRecognitionResult!!!");
        //textMeshPro.SetText("��ʼִ�У�");
        WWWForm webForm = new WWWForm();
        webForm.AddBinaryData("speech_cont", audioData, "audio.wav", "audio/wav");
        Debug.Log("22222!!!");
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(predictionEndpoint, webForm))
        {
            //progressIndicator.OpenAsync();
            //progressIndicator.Message = "Loading...";
            Debug.Log(predictionEndpoint);
            //unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.isHttpError || unityWebRequest.isNetworkError)
            {
                if (unityWebRequest.isHttpError==true)
                {
                    Debug.Log("1111");

                }
                if (unityWebRequest.isNetworkError == true)
                {
                    Debug.Log("2222");
                }
                Debug.Log(unityWebRequest.error);

                //textMeshPro.SetText("false");
            }
            else
            {
                string responseJson = unityWebRequest.downloadHandler.text;
                Debug.Log("Python response: " + responseJson);
                //
                //textMeshPro.SetText("true");
                // ����JSON��Ӧ
                SpeechRecognitionResponse response = JsonUtility.FromJson<SpeechRecognitionResponse>(responseJson);

                if (response.error != null)
                {
                    Debug.LogError("Speech recognition error: " + response.error);
                }
                else
                {
                    string command = response.command;
                    string number = response.number;

                    if (command == "figure")
                    {
                        // �����ͼƬ���߼�
                        Debug.Log("Open figure: " + number+"id:::"+id);
                        openPics.availablePosition(command,number);
                        id++;
                        Debug.Log("newid:::" + id);
                    }
                    else if (command == "table")
                    {
                        Debug.Log("Open table: " + number);
                        openPics.availablePosition(command, number);
                        id++;
                    }
                    else if (command == "reference")
                    {
                        Debug.Log("Reference:" + number);
                        openPics.addreferenceAsync(number);
                        id++;
                    }
                    else if (command == "model")
                    {
                        string objData = response.obj.data;
                        string mtlData = response.mtl.data;

                        // ��Base64�����ģ�����ݽ���Ϊ�ֽ�����
                        byte[] objBytes = System.Convert.FromBase64String(objData);
                        byte[] mtlBytes = System.Convert.FromBase64String(mtlData);

                        // �������ģ�͵��߼�
                        LoadModel(number, objBytes, mtlBytes);
                        id++;
                    }
                }
            }
        }
    }

    private void LoadModel(string modelId, byte[] objBytes, byte[] mtlBytes)
    {
        GameObject loadedObject;

        // ���ֽ����鱣��Ϊ��ʱ�ļ�
        string tempObjPath = Path.Combine(Application.persistentDataPath, $"{modelId}.obj");
        string tempMtlPath = Path.Combine(Application.persistentDataPath, $"{modelId}.mtl");
        File.WriteAllBytes(tempObjPath, objBytes);
        File.WriteAllBytes(tempMtlPath, mtlBytes);

        if (!File.Exists(tempObjPath) || !File.Exists(tempMtlPath))
        {
            error = "Failed to save temporary files.";
        }
        else
        {
            loadedObject = new OBJLoader().Load(tempObjPath);
            loadedObject.transform.localPosition = new Vector3(-0.06f, 0.12f, 0.34f);
            loadedObject.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
            Transform defaultTransform = loadedObject.transform.Find("default");
            MeshFilter mf = defaultTransform.GetComponent<MeshFilter>();
            loadedObject.AddComponent<ObjectManipulator>();
            loadedObject.AddComponent<NearInteractionGrabbable>();

            GameObjectManager.Instance.AddObject("model"+modelId, ObjectType.Model, loadedObject);
            GetObjectId getObjectId = loadedObject.AddComponent<GetObjectId>();
            getObjectId.setid(id);
            GameObjectManager.Instance.PrintObjectInfo();

            rotatingOrbs.SetActive(false);

            // �޸�messageText���ı�����
            progressIndicator.Message = "Loading Success!";

            // ��һ�������messageText
            Invoke("HideMessageText", 1.5f);
            Bounds bounds = mf.mesh.bounds;
            BoxCollider bc = loadedObject.AddComponent<BoxCollider>();
            bc.size = bounds.size;
            Debug.Log(bc.size);
            error = string.Empty;

            // ɾ����ʱ�ļ�
            File.Delete(tempObjPath);
            File.Delete(tempMtlPath);
        }
    }

    private void HideMessageText()
    {
        progressIndicator.CloseAsync();
    }

    public void startspeech()
    {
        byte[] audioData=ReadWAVFile(audioFilePath);
        StartCoroutine(GetSpeechRecognitionResult(audioData));
    }
    private byte[] ReadWAVFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            // ��ȡWAV�ļ��������ֽ�
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // ���WAV�ļ�ͷ�Ƿ���Ч
            if (IsValidWAVFile(fileBytes))
            {
                // ȥ��WAV�ļ�ͷ(ǰ44���ֽ�)
                byte[] audioBytes = new byte[fileBytes.Length - 44];
                System.Array.Copy(fileBytes, 44, audioBytes, 0, audioBytes.Length);

                return audioBytes;
            }
            else
            {
                Debug.LogError("Invalid WAV file format.");
                return null;
            }
        }
        else
        {
            Debug.LogError("Audio file not found: " + filePath);
            return null;
        }
    }

    private bool IsValidWAVFile(byte[] fileBytes)
    {
        // ���WAV�ļ���RIFFͷ
        return fileBytes[0] == 'R' && fileBytes[1] == 'I' && fileBytes[2] == 'F' && fileBytes[3] == 'F'
            && fileBytes[8] == 'W' && fileBytes[9] == 'A' && fileBytes[10] == 'V' && fileBytes[11] == 'E';
    }
}

[System.Serializable]
public class SpeechRecognitionResponse
{
    public string error;
    public string command;
    public string number;
    public ModelData obj;
    public ModelData mtl;
}

[System.Serializable]
public class ModelData
{
    public string name;
    public string data;
}