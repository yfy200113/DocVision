using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class DictationHandlerTest : BaseInputHandler, IMixedRealityDictationHandler
    {
        [SerializeField]
        [Tooltip("Length in seconds for the audio clip")]
        private int recordingTime = 2;

        [SerializeField]
        [Tooltip("Whether recording should start automatically on start")]
        private bool startRecordingOnStart = true;

        private IMixedRealityDictationSystem dictationSystem;
        private bool isRecording = false;
        private AudioClip audioClip;

        private void StartRecording()
        {
            Debug.Log("1111");
            if (dictationSystem != null && !isRecording)
            {
                isRecording = true;
                dictationSystem.StartRecording(gameObject, initialSilenceTimeout: 0, autoSilenceTimeout: 0, recordingTime);
            }
        }

        private void StopRecording()
        {
            if (dictationSystem != null && dictationSystem.IsListening)
            {
                dictationSystem.StopRecording();
                isRecording = false;
            }
        }

        private IEnumerator SendAudioToPython(AudioClip clip)
        {
            string fileName = "temp.wav";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            // ��AudioClip����ΪWAV�ļ�
            //SavWav.Save(filePath, clip);

            // ʹ��UnityWebRequest����Ƶ�ļ����͵�Python������
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://your-python-server-url/process-audio", UnityWebRequest.kHttpVerbPOST))
            {
                www.uploadHandler = new UploadHandlerFile(filePath);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Audio sent to Python successfully!");
                }
                else
                {
                    Debug.LogError("Error sending audio to Python: " + www.error);
                }
            }

            // ɾ����ʱ��Ƶ�ļ�
            File.Delete(filePath);
        }

        protected override void Start()
        {
            base.Start();

            dictationSystem = CoreServices.GetInputSystemDataProvider<IMixedRealityDictationSystem>();
            Debug.Assert(dictationSystem != null, "No dictation system found. In order to use dictation, add a dictation system like 'Windows Dictation Input Provider' to the Data Providers in the Input System profile");

            if (startRecordingOnStart)
            {
                Debug.Log("2222");
                StartRecording();
            }
        }

        protected override void OnDisable()
        {
            StopRecording();
            base.OnDisable();
        }

        #region InputSystemGlobalHandlerListener Implementation

        /// <inheritdoc />
        protected override void RegisterHandlers()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityDictationHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityDictationHandler>(this);
        }

        #endregion InputSystemGlobalHandlerListener Implementation

        #region IMixedRealityDictationHandler implementation

        void IMixedRealityDictationHandler.OnDictationHypothesis(DictationEventData eventData)
        {
            // ʵʱ�������
            if (!isRecording)
            {
                StartRecording();
            }
        }

        void IMixedRealityDictationHandler.OnDictationResult(DictationEventData eventData)
        {
            if (isRecording)
            {
                // 获取音频剪辑
                audioClip = dictationSystem.AudioClip;
                StartCoroutine(SendAudioToPython(audioClip));
                StopRecording();
            }
        }

        void IMixedRealityDictationHandler.OnDictationComplete(DictationEventData eventData)
        {
            if (isRecording)
            {
                StopRecording();
            }
        }

        void IMixedRealityDictationHandler.OnDictationError(DictationEventData eventData)
        {
            if (isRecording)
            {
                StopRecording();
            }
        }

        #endregion IMixedRealityDictationHandler implementation
    }
}