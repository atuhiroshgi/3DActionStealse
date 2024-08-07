using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public enum State
    {
        VOLUME,
        CAMERA,
        BRIGHT,
    }

    [SerializeField] private SettingUI settingUI;
    [Header("ゲージの上の文字")]
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private TextMeshProUGUI cameraSpeedText;
    [SerializeField] private TextMeshProUGUI brightText;
    [Header("ゲージ中の数字")]
    [SerializeField] private TextMeshProUGUI volumeValue;
    [SerializeField] private TextMeshProUGUI cameraValue;
    [SerializeField] private TextMeshProUGUI brightValue;
    [Header("ゲージ")]
    [SerializeField] private Image volumeGauge;
    [SerializeField] private Image cameraGauge;
    [SerializeField] private Image brightGauge;

    private State currentState;

    private float volumeFill = 1f;
    private float cameraSpeedFill = 1f;
    private float brightFill = 1f;


    private void Start()
    {
        currentState = State.VOLUME;
        UpdateUI();
    }

    private void Update()
    {
        if (settingUI.isOpen)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeState(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeState(1);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                ChangeSetting(0.01f);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                ChangeSetting(-0.01f);
            }


            UpdateUI();
        }
    }

    private void ChangeState(int direction)
    {
        currentState = (State)(((int)currentState + direction + 3) % 3);
    }

    private void ChangeSetting(float delta)
    {
        switch (currentState)
        {
            case State.VOLUME:
                volumeFill = Mathf.Clamp(volumeFill + delta, 0, 1);
                break;

            case State.CAMERA:
                cameraSpeedFill = Mathf.Clamp(cameraSpeedFill + delta, 0, 1);
                break;

            case State.BRIGHT:
                brightFill = Mathf.Clamp(brightFill + delta, 0, 1);
                break;

            default:
                Debug.LogError("MenuController.ChangeSetting内のswitch文で存在しないcurrentStateが参照されようとしています");
                break;
        }
    }

    private void UpdateUI()
    {
        volumeGauge.fillAmount = volumeFill;
        cameraGauge.fillAmount = cameraSpeedFill;
        brightGauge.fillAmount = brightFill;

        volumeValue.text = ((int)(volumeFill * 100)).ToString();
        cameraValue.text = ((int)(cameraSpeedFill * 100)).ToString();
        brightValue.text = ((int)(brightFill * 100)).ToString();

        volumeText.color = currentState == State.VOLUME ? Color.yellow : Color.white;
        cameraSpeedText.color = currentState == State.CAMERA ? Color.yellow : Color.white;
        brightText.color = currentState == State.BRIGHT ? Color.yellow : Color.white;
    }
}