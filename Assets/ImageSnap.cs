using DG.Tweening;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSnap : MonoBehaviour
{
    public GameObject snapPoint; // 当前图片的目标位置
    private Vector3 initialPosition; // 图片的初始位置
    private bool isDragging = false; // 标记是否正在拖动图片
    public float disableDuration = 1f; // 禁用操作组件的持续时间(秒)
    private ObjectManipulator objectManipulator; // 图片的操作组件
    private void Start()
    {
        // 记录图片的初始位置
        initialPosition = transform.position;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if ( isDragging)
        {
            
            objectManipulator = other.gameObject.GetComponent<ObjectManipulator>();
            Debug.Log("image: " + other.gameObject.name);
            Transform parentTransform = other.transform.parent;
            int positionid=GameObjectManager.Instance.GetId(parentTransform.gameObject);

            other.transform.eulerAngles = transform.eulerAngles;
            other.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            other.transform.localPosition = new Vector3(0, 0, 0);
            other.transform.DOMove(transform.position, 2f).SetEase(Ease.OutQuad);
            parentTransform.localPosition = initialPosition;
            
            isDragging = false; // 停止拖动状态,防止图片继续触发碰撞事件
            if (objectManipulator != null)
            {
                objectManipulator.enabled = false;
                StartCoroutine(EnableObjectManipulatorAfterDelay());
            }
        }
    }

    private IEnumerator EnableObjectManipulatorAfterDelay()
    {
        yield return new WaitForSeconds(disableDuration);

        // 在指定时间后重新启用ObjectManipulator组件
        if (objectManipulator != null)
        {
            objectManipulator.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
            Debug.Log("LEAVE!");
            // 当图片离开目标位置的碰撞体时,重新启用拖动状态
            isDragging = true;
        
    }
}
