using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Text countdownText;

    private float countdownTime;

    private void Update()
    {
        countdownTime = GameManager.Instance.GetTime();

        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        countdownText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
}
