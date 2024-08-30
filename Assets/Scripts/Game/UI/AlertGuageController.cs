using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertGuageController : MonoBehaviour
{
    [Header("�x���x�Q�[�W")]
    [SerializeField] private Image alertGaugeImage;
    [Header("�x���x�̐F")]
    [SerializeField] private Color whiteColor = Color.white;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color redColor = Color.red;

    private float maxAlertLevel = 100f;
    private float currentAlertLevel = 0f;
    private bool isGameOver = false;

    private void Start()
    {
        UpdateAlertLevel(GameManager.Instance.GetAlertLevel());
        Debug.Log("���̃X�N���v�g�� " + gameObject.name + " �ɃA�^�b�`����Ă��܂��B");
        isGameOver = false;
    }

    private void Update()
    {
        UpdateAlertLevel(GameManager.Instance.GetAlertLevel());
    }

    public void UpdateAlertLevel(float currentAlertLevel)
    {
        float fillAmount = currentAlertLevel / maxAlertLevel * 0.75f;
        alertGaugeImage.fillAmount = fillAmount;

        if(currentAlertLevel <= 40)
        {
            alertGaugeImage.color = whiteColor;
        }
        else if(currentAlertLevel <= 80)
        {
            alertGaugeImage.color = yellowColor;
        }
        else
        {
            alertGaugeImage.color = redColor;
        }

        if(currentAlertLevel >= 100)
        {
            isGameOver = true;
        }
    }

    public bool GetIsGameOver() {
        return isGameOver;
    }
}
