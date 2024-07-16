using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShakeUIObject : MonoBehaviour
{
    [Header("揺らしたいUIオブジェクト")]
    [SerializeField] private GameObject uiObject;
    [Header("揺れの強さ")]
    [SerializeField] private float shakeAmount = 10f;
    [Header("揺れの速さ")]
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
            //ランダムなオフセットを計算
            float randomOffsetX = Random.Range(-1f, 1f);
            float randomOffsetY = Random.Range(-1f, 1f);

            //揺れの計算
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float shakeY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            //UIオブジェクトを揺らす
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(shakeX, shakeY);
        }
    }
}