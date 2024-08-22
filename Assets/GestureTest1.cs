using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GestureTest1 : MonoBehaviour
    //, IMixedRealityHandJointHandler
{
    MixedRealityHandTrackingProfile handTrackingProfile;
    // Start is called before the first frame update

    //MixedRealityPose middleTipPose, indexTipPose, ringTipPose, pinkyTipPose,palmPose,wristPose;
    /*void IMixedRealityHandJointHandler.OnHandJointsUpdated(Microsoft.MixedReality.Toolkit.Input.InputEventData<System.Collections.Generic.IDictionary<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint, Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>> eventData)
    {
       
        if (eventData.Handedness == Handedness.Right)
        {
            if(eventData.InputData.TryGetValue(TrackedHandJoint.MiddleTip,out  middleTipPose)&&
                eventData.InputData.TryGetValue(TrackedHandJoint.IndexTip, out  indexTipPose) &&
                eventData.InputData.TryGetValue(TrackedHandJoint.PinkyTip, out  pinkyTipPose) &&
                eventData.InputData.TryGetValue(TrackedHandJoint.RingTip, out  ringTipPose)&&
                eventData.InputData.TryGetValue(TrackedHandJoint.Palm, out  palmPose) &&
                eventData.InputData.TryGetValue(TrackedHandJoint.Wrist, out MixedRealityPose wristPose))
            {
                Debug.Log("0000");
                Debug.Log(indexTipPose.Position.ToString());
                Debug.Log(middleTipPose.Position.ToString());
                Debug.Log(ringTipPose.Position.ToString());
                Debug.Log(pinkyTipPose.Position.ToString());
            }
        }
    }*/
    private float flatHandThreshold = 45.0f;
    private float facingCameraTrackingThreshold = 80.0f;
    private bool isTanFlag = false;
    private List<Vector3> palmPoss=new List<Vector3>();
    private List<MixedRealityPose> palmPoses = new List<MixedRealityPose>();
    private List<MixedRealityPose> wristPoses = new List<MixedRealityPose>();
    private List<MixedRealityPose> middlePoses = new List<MixedRealityPose>();
    private Vector3 lastPalmPos=new Vector3(0f,0f,0f);
    public float timer = 0f;
    public MixedRealityPose lastPalmPose;
    public bool ifRecognized=false;
    public int number = 0;
    void Start()
    {
        IsTan();
        
        lastPalmPose = palmPose;
        Vector3 palmPosition1 = new Vector3(0.30f, -0.20f, 0.37f) ;
        Vector3 wristPosition1 = new Vector3(0.34f, -0.21f, 0.34f);

        Vector3 palmPosition2 = new Vector3(0.34f, -0.21f, 0.38f);
        Vector3 wristPosition2 = new Vector3(0.32f, -0.22f, 0.34f);

        float rotationAngle = CalculateRotationAngle(palmPosition1, wristPosition1, palmPosition2, wristPosition2);

        Debug.Log("Rotation angle: " + rotationAngle + " degrees");
    }
    float CalculateRotationAngle(Vector3 palmPosition1, Vector3 wristPosition1, Vector3 palmPosition2, Vector3 wristPosition2)
    {
        // Calculate the initial and final directions
        Vector3 initialDirection = (palmPosition1 - wristPosition1).normalized;
        Vector3 finalDirection = (palmPosition2 - wristPosition2).normalized;

        // Calculate the rotation axis using cross product
        Vector3 rotationAxis = Vector3.Cross(initialDirection, finalDirection);

        // Calculate the dot product to get the cosine of the angle
        float dotProduct = Vector3.Dot(initialDirection, finalDirection);

        // Calculate the rotation angle in radians
        float rotationAngleRadians = Mathf.Atan2(rotationAxis.magnitude, dotProduct);

        // Convert the angle to degrees
        float rotationAngleDegrees = rotationAngleRadians * Mathf.Rad2Deg;

        return rotationAngleDegrees;
    }
    // Update is called once per frame
    void Update()
    {
        //IsTan();
        ////timer += Time.deltaTime;
        //if (ifRecognized == false)
        //{
        //    ifRecognized = ifStartRecognized();
        //}
        //else
        //{
        //    number++;
        //    if (number == 1)
        //    {
        //        lastPalmPose = palmPose;
        //    }
        //    else if (number > 1 && number < 60)
        //    {
        //        float angle = Vector3.Angle(palmPose.Up, lastPalmPose.Up);
        //        if (angle >= 65)
        //        {
        //            float time = (float)((number - 1) * 0.01667);

        //            Debug.Log("时间差：" + time);
        //            Debug.Log("角速度变化值：" + 40 / time);
        //            Ray ray = new Ray(indexTipPose.Position, indexTipPose.Forward);
        //            RaycastHit hitInfo;
        //            int layerMask = 1 << LayerMask.NameToLayer("Default");
        //            if (Physics.Raycast(ray, out hitInfo, layerMask))
        //            {
        //                if (hitInfo.collider.gameObject != null)
        //                {
        //                    Debug.Log("GestureTest1:::" + hitInfo.collider.gameObject.name);
        //                    Destroy(hitInfo.collider.gameObject);

        //                }

        //            }
        //            number = 0; ifRecognized = false;
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("超时");
        //        number = 0; ifRecognized = false;
        //    }
        //}
    }

    //判断是否面向摄像机
    public bool ifStartRecognized()
    {
        float anglePalmToCamera = Vector3.Angle(palmPose.Up, CameraCache.Main.transform.forward);
        var handNormal = Vector3.Cross(indexTipPose.Position - palmPose.Position, ringTipPose.Position - indexTipPose.Position).normalized;
        float angleFlatHand = Vector3.Angle(palmPose.Up, handNormal);
        
        if (anglePalmToCamera < facingCameraTrackingThreshold&&angleFlatHand<flatHandThreshold)
        {
            Debug.Log("开始触发手势识别");

            return true;
        }
        return false;
    }

    
    MixedRealityPose palmPose, thumbTipPose, thumbDistalPose, middleTipPose, middleDistalPose, wristPose,indexTipPose,ringTipPose;
    
    public bool SavePalmGesture(List<Vector3> vector3s, List<MixedRealityPose>poses, List<MixedRealityPose> wristposes, List<MixedRealityPose> middleposes)
    {
        Vector3 vec1 = vector3s[0];
        Vector3 vec2 = vector3s[vector3s.Count - 1];
        MixedRealityPose pose1 = poses[0];
        MixedRealityPose pose2 = poses[poses.Count - 1];
        Debug.Log(vector3s.Count);
        Debug.Log("angle : " + Vector3.Angle(pose1.Up, pose2.Up));
        Vector3 palmwristpose1 = wristposes[0].Position - poses[0].Position;
        Vector3 palmwristpose2 = wristposes[vector3s.Count-1].Position - poses[vector3s.Count - 1].Position;
        float angle1 = Vector3.Angle(palmwristpose1, Vector3.up);
        float angle2 = Vector3.Angle(palmwristpose2, Vector3.up);
        float angleChange = Mathf.Abs(angle1 - angle2);
        Debug.Log("angleChange:::" + angleChange);
        if (Vector3.Angle(pose1.Up, pose2.Up) > 45.0f&&angleChange<20.0f)
        {
            Debug.Log("是删除手势");
            /*float maxRayLength = 0.5f;
            Vector3 rayDirection = (wristposes[vector3s.Count - 1].Position - poses[vector3s.Count - 1].Position).normalized;
            Vector3 rayEndpoint = wristposes[vector3s.Count - 1].Position + rayDirection * maxRayLength;

            int layerMask = 1 << LayerMask.NameToLayer("Default");
            Ray ray = new Ray(wristposes[vector3s.Count-1].Position, rayDirection);
            Ray ray2 = new Ray(middlePoses[vector3s.Count - 1].Position, middlePoses[vector3s.Count - 1].Forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray2, out hitInfo,layerMask))
            {
                Debug.Log(hitInfo.ToString());
                if (hitInfo.collider != null)
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    Debug.Log(hitObject);
                    // 在这里可以执行删除或其他操作
                    Destroy(hitObject);
                }
            }*/
            return true;
            //Debug.DrawLine(wristposes[vector3s.Count - 1].Position, rayEndpoint, Color.red, 1.0f);
        }

        return false;
    }

    public bool IsTan()
    {
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out middleTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleDistalJoint, Handedness.Right, out middleDistalPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palmPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, Handedness.Right, out wristPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTipPose);
        HandJointUtils.TryGetJointPose(TrackedHandJoint.RingTip, Handedness.Right, out ringTipPose);
        if (
           HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleTip, Handedness.Right, out middleTipPose) &&
           HandJointUtils.TryGetJointPose(TrackedHandJoint.MiddleDistalJoint, Handedness.Right, out middleDistalPose) &&
           HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out palmPose) &&
           HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, Handedness.Right, out wristPose) &&
           HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out indexTipPose))
        {
            return true;
        }
        return false;
    }
}
