using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShakeUIObject : MonoBehaviour
{
    [Header("�h�炵����UI�I�u�W�F�N�g")]
    [SerializeField] private GameObject uiObject;
    [Header("�h��̋���")]
    [SerializeField] private float shakeAmount = 10f;
    [Header("�h��̑���")]
    [SerializeField] private float shakeSpeed = 5f;

    private Vector2 originalPosition;

    private void Start()
    {
        if(uiObject != null)
        {
            RectTransform rectTranform = uiObject.GetComponent<RectTransform>();
            originalPosition = rectTranform.anchoredPosition;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GetIsIncrease())
        {
            //�����_���ȃI�t�Z�b�g���v�Z
            float randomOffsetX = Random.Range(-1f, 1f);
            float randomOffsetY = Random.Range(-1f, 1f);

            //�h��̌v�Z
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float shakeY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            //UI�I�u�W�F�N�g��h�炷
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(shakeX, shakeY);
        }
    }
}