using DG.Tweening;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSnap : MonoBehaviour
{
    public GameObject snapPoint; // ��ǰͼƬ��Ŀ��λ��
    private Vector3 initialPosition; // ͼƬ�ĳ�ʼλ��
    private bool isDragging = false; // ����Ƿ������϶�ͼƬ
    public float disableDuration = 1f; // ���ò�������ĳ���ʱ��(��)
    private ObjectManipulator objectManipulator; // ͼƬ�Ĳ������
    private void Start()
    {
        // ��¼ͼƬ�ĳ�ʼλ��
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
            
            isDragging = false; // ֹͣ�϶�״̬,��ֹͼƬ����������ײ�¼�
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

        // ��ָ��ʱ�����������ObjectManipulator���
        if (objectManipulator != null)
        {
            objectManipulator.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
            Debug.Log("LEAVE!");
            // ��ͼƬ�뿪Ŀ��λ�õ���ײ��ʱ,���������϶�״̬
            isDragging = true;
        
    }
}
