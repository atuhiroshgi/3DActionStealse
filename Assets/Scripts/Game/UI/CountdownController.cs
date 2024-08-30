using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    [SerializeField] private SlideUIController slideUIController;
    [SerializeField] private GameObject startCountdownWindow;
    [SerializeField] private Text startCountdownText;
    [SerializeField] private Text finishCountdownText;

    private float startCountdown = 3f;
    private bool onceSlide;

    private void Start()
    {
        onceSlide = false;

        GameManager.Instance.SetStartFlag(false);
        finishCountdownText.gameObject.SetActive(false);

        StartCoroutine(StartGameAfterCountdown());
    }

    private void Update()
    {
        if(GameManager.Instance.GetTime() <= 6)
        {
            finishCountdownText.gameObject.SetActive(true);
            finishCountdownText.text = ((int)GameManager.Instance.GetTime()).ToString();

            if((int)GameManager.Instance.GetTime() == 0)
            {
                finishCountdownText.gameObject.SetActive(false);
            }
        }

        if (GameManager.Instance.GetAllCaptured() && !onceSlide)
        {
            Debug.Log("’Ê‚Á‚Ä‚ñ‚Å");
            onceSlide = true;
            StartCoroutine(FinishUI(true));
        }

        if(GameManager.Instance.GetTime() <= 1 && !onceSlide)
        {
            onceSlide = true;
            StartCoroutine(FinishUI(false));
        }
    }

    private IEnumerator StartGameAfterCountdown()
    {
        float currentTime = startCountdown;
        startCountdownWindow.SetActive(true);

        while(currentTime > 0)
        {
            startCountdownText.text = ((int)currentTime).ToString("0");
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        startCountdownText.fontSize = 200;
        startCountdownText.text = "Let's surprise!";
        yield return new WaitForSeconds(1f);
        startCountdownWindow.SetActive(false);

        GameManager.Instance.SetStartFlag(true);
    }

    private IEnumerator FinishUI(bool result)
    {
        GameManager.Instance.SetStartFlag(false);
        yield return new WaitForSeconds(1.0f);
        slideUIController.state = 1;
        yield return new WaitForSeconds(3.0f);

        if (result)
        {
            GameManager.Instance.ToClearScene();
        }
        else
        {
            GameManager.Instance.ToFailedScene();
        }
    }
}
