using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raytest : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*GameObject cube = GameObject.Find("testcube");
        //Debug.Log(cube.name);
        BoxCollider boxCollider = cube.GetComponent<BoxCollider>();
        Vector3 center = boxCollider.bounds.center;
        int layerMask = 1 << LayerMask.NameToLayer("Default");
        Ray ray = new Ray(center, Vector3.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, layerMask))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            Debug.Log(hitInfo.collider.name);
            if (hitInfo.collider != null)
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                Debug.Log(hitObject);
                // 在这里可以执行删除或其他操作
                Destroy(hitObject);
            }
        }*/

        /*MixedRealityPose middleTipPose;
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out middleTipPose))
        {
            Ray ray = new Ray(middleTipPose.Position, middleTipPose.Forward);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.Log(hitInfo.collider.name);
                Destroy(hitInfo.collider.gameObject);
            }
        }*/


    }
}
