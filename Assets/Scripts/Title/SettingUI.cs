using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject settingWindow;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image volumeImage;
    [SerializeField] private Image cameraSpeedImage;
    [SerializeField] private Image brightImage;
    [SerializeField] private TitleAnimation titleAnimation;
    [SerializeField] private MenuController menuController;
    [SerializeField] private float startYPosition = 10f;
    [SerializeField] private float endYPosition = 0f;
    [SerializeField] private float speed = 5f;

    public bool isOpen = false;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Coroutine currentCoroutine;

    private void Start()
    {
        // 初期位置を設定
        startPosition = new Vector3(rectTransform.localPosition.x, startYPosition, rectTransform.localPosition.z);
        // 定位置を設定
        endPosition = new Vector3(rectTransform.localPosition.x, endYPosition, rectTransform.localPosition.z);
        // ロゴの位置を初期位置に設定
        rectTransform.localPosition = startPosition;

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