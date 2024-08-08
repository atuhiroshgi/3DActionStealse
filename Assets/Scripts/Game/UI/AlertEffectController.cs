using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertEffectController : MonoBehaviour
{
    [SerializeField] private GameObject alertEffect;
    [SerializeField] private Image topAlertEffect;
    [SerializeField] private Image downAlertEffect;
    [SerializeField] private Image leftAlertEffect;
    [SerializeField] private Image rightAlertEffect;
    [SerializeField] private float colorA = 0.5f;

    private Color greenColor;
    private Color yellowColor;
    private Color redColor;
    private float alertLevel = 0;
    private float lastAlertLevel;
    private float blinkInterval = 0.2f;
    private bool isBlinking = false;

    private void Start()
    {
        if(topAlertEffect == null)
        {
            topAlertEffect = GetComponent<Image>();
            downAlertEffect = GetComponent<Image>();
            leftAlertEffect = GetComponent<Image>();
            rightAlertEffect = GetComponent<Image>();
        }

        alertEffect.gameObject.SetActive(true);

        greenColor = new Color(0, 1, 0, colorA);
        yellowColor = new Color(1, 1, 0, colorA);
        redColor = new Color(1, 0, 0, colorA);

        UpdateAlertEffect();
    }

    private void Update()
    {
        alertLevel = GameManager.Instance.GetAlertLevel();
        UpdateAlertEffect();

        if (!isBlinking && alertLevel > 70)
        {
            isBlinking = true;
            StartCoroutine(BlinkEffect());
        }
    }

    public void UpdateAlertEffect()
    {
        Color newColor = Color.clear;

        if(alertLevel >= 0 && alertLevel <= 40)
        {
            float t = alertLevel / 40f;
            newColor = Color.Lerp(greenColor, yellowColor, t);
        }
        else if(alertLevel > 40 && alertLevel <= 100)
        {
            float t = (alertLevel - 40) / 60f;
            newColor = Color.Lerp(yellowColor, redColor, t);

        }

        topAlertEffect.color = newColor;
        downAlertEffect.color = newColor;
        leftAlertEffect.color = newColor;
        rightAlertEffect.color = newColor;
    }

    private IEnumerator BlinkEffect()
    {
        while(true)
        {
            alertEffect.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
            alertEffect.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
