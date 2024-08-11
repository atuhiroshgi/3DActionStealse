using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject menuWindow;
    [SerializeField] private GameObject settingElement;
    [SerializeField] private GameObject playerInfoElement;
    [SerializeField] private GameObject explainElement;
    [SerializeField] private Image settingTab;
    [SerializeField] private Image playerInfoTab;
    [SerializeField] private Image explainTab;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image volumeImage;
    [SerializeField] private Image cameraSpeedImage;
    [SerializeField] private Image brightImage;
    [SerializeField] private RectTransform supportArrowRectTransform;
    [SerializeField] private TitleAnimation titleAnimation;
    [SerializeField] private MenuController menuController;
    [SerializeField] private float startYPosition = 10f;
    [SerializeField] private float endYPosition = 0f;
    [SerializeField] private float speed = 5f;

    public bool isOpen = false;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 settingPosition = new Vector3(-645, 345, 0);
    private Vector3 playerInfoPosition = new Vector3(0, 345, 0);
    private Vector3 explainPosition = new Vector3(645, 345, 0);
    private Color defaultColor = new Color(1f, 1f, 1f);
    private Color decideColor = new Color(166f / 255f, 166f / 255f, 166f / 255f);

    private Coroutine currentCoroutine;

    private enum State
    {
        SETTING,
        PLAYERINFO,
        EXPLAIN,
    }

    private State currentState;

    private void Start()
    {
        // 初期位置を設定
        startPosition = new Vector3(rectTransform.localPosition.x, startYPosition, rectTransform.localPosition.z);
        // 定位置を設定
        endPosition = new Vector3(rectTransform.localPosition.x, endYPosition, rectTransform.localPosition.z);
        // ロゴの位置を初期位置に設定
        rectTransform.localPosition = startPosition;

        currentState = State.SETTING;
        supportArrowRectTransform.anchoredPosition = settingPosition;

        LoadSetting();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && isOpen == false)
        {
            LoadSetting();
            StartCoroutine(OpenWindowWithDelay(0.5f));
        }
        if (Input.GetKeyDown(KeyCode.Return) && isOpen == true)
        {
            SaveSetting();
        }
        if (Input.GetKeyDown(KeyCode.Backspace) && isOpen == true)
        {
            StartCoroutine(CloseWindowWithDelay(0.5f));
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            MoveTag(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveTag(1);
        }
    }

    public void SaveSetting()
    {
        // 設定をGameManagerに保存
        GameManager.Instance.SetVolume(volumeImage.fillAmount);
        GameManager.Instance.SetCameraSpeed(cameraSpeedImage.fillAmount);
        GameManager.Instance.SetBright(brightImage.fillAmount);

        // 必要に応じて設定をPlayerPrefsなどに保存
        PlayerPrefs.SetFloat("Volume", volumeImage.fillAmount);
        PlayerPrefs.SetFloat("CameraSpeed", cameraSpeedImage.fillAmount);
        PlayerPrefs.SetFloat("Bright", brightImage.fillAmount);

        AudioManager.Instance.PlaySFX("DecideMenu");

        StartCoroutine(CloseWindowWithDelay(0.5f));
    }

    private void LoadSetting()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            volumeImage.fillAmount = PlayerPrefs.GetFloat("Volume");
        }
        if (PlayerPrefs.HasKey("CameraSpeed"))
        {
            cameraSpeedImage.fillAmount = PlayerPrefs.GetFloat("CameraSpeed");
        }
        if (PlayerPrefs.HasKey("Bright"))
        {
            brightImage.fillAmount = PlayerPrefs.GetFloat("Bright");
        }

        // MenuControllerの設定値を更新
        menuController.SetSettings(volumeImage.fillAmount, cameraSpeedImage.fillAmount, brightImage.fillAmount);
        menuController.UpdateUI();

        //GameManagerにロードした情報を渡す
        GameManager.Instance.SetVolume(PlayerPrefs.GetFloat("Volume"));
        GameManager.Instance.SetCameraSpeed(PlayerPrefs.GetFloat("CameraSpeed"));
        GameManager.Instance.SetBright(PlayerPrefs.GetFloat("Bright"));
    }

    private IEnumerator OpenWindowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenWindow();
    }
    private IEnumerator CloseWindowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseWindow();
    }

    public void OpenWindow()
    {
        AudioManager.Instance.PlaySFX("DownMenu");

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(MoveWindow(startPosition, endPosition));
        isOpen = true;
        //ghost.enabled = false;
    }

    public void CloseWindow()
    {
        AudioManager.Instance.PlaySFX("UpMenu");

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(MoveWindow(endPosition, startPosition));
        isOpen = false;
        titleAnimation.MenuAnimation();
        //ghost.enabled = true;
    }

    private void MoveTag(int direction)
    {
        currentState = (State)(((int)currentState + direction + 3) % 3);

        settingTab.color = currentState == State.SETTING ? decideColor : defaultColor;
        playerInfoTab.color = currentState == State.PLAYERINFO ? decideColor : defaultColor;
        explainTab.color = currentState == State.EXPLAIN ? decideColor : defaultColor;

        switch (currentState)
        {
            case State.SETTING:
                settingElement.SetActive(true);
                playerInfoElement.SetActive(false);
                explainElement.SetActive(false);
                supportArrowRectTransform.anchoredPosition = settingPosition;
                break;

            case State.PLAYERINFO:
                settingElement.SetActive(false);
                playerInfoElement.SetActive(true);
                explainElement.SetActive(false);
                supportArrowRectTransform.anchoredPosition = playerInfoPosition;
                break;

            case State.EXPLAIN:
                settingElement.SetActive(false);
                playerInfoElement.SetActive(false);
                explainElement.SetActive(true);
                supportArrowRectTransform.anchoredPosition = explainPosition;
                break;

            default:
                break;
        }
    }

    private IEnumerator MoveWindow(Vector3 fromPosition, Vector3 toPosition)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            rectTransform.localPosition = Vector3.Lerp(fromPosition, toPosition, elapsedTime);
            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }
        rectTransform.localPosition = toPosition;
    }
}