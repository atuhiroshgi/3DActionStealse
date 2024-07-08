using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBlinker : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float blinkInterval = 0.5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > blinkInterval)
        {
            text.enabled = !text.enabled;
            timer = 0f;
        }
    }
}
