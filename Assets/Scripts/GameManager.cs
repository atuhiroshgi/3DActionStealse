using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //GameManagerのインスタンスを取得するためのプロパティ
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // GameManagerがアタッチされているGameObjectを作成し、それにGameManagerコンポーネントをアタッチする
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go); // シーン切替時に破棄されないように設定
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 既にインスタンスが存在する場合は、自身を破棄する
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region タイトルシーン
    public void ToGameScene()
    {
        SceneManager.LoadScene("Game");
    }
    #endregion

    #region メインシーン
    [SerializeField] private SlideUIController slideUIController;
    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    private float AlertLevel = 0;
    private float countdownTime = 180;
    private bool onceSlide = false;

    private void Update()
    {
        UpdateCountdownTimer();
    }

    /// <summary>
    /// チェックポイントを登録する
    /// </summary>
    public void RegisterCheckPoint(CheckPoint checkPoint)
    {
        if (!checkPoints.Contains(checkPoint))
        {
            checkPoints.Add(checkPoint);
        }
    }

    /// <summary>
    /// 全てのチェックポイントが制覇されたか確認する
    /// </summary>
    public void CheckAllCheckPointsCaptured()
    {
        bool allCaptured = true;
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (!checkPoint.IsCaptured)
            {
                allCaptured = false;
                break;
            }
        }

        if (allCaptured)
        {
            ToClearScene();
        }
    }

    /// <summary>
    /// カウントダウンする処理
    /// </summary>
    private void UpdateCountdownTimer()
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;

            if(countdownTime <= 60 && !onceSlide)
            {
                onceSlide = true;
                StartCoroutine(SlideUI());
            }

            if(countdownTime <= 0)
            {
                ToFailedScene();
            }
        }
    }

    /// <summary>
    /// 外部から残り時間を取得
    /// </summary>
    /// <returns>残り時間</returns>
    public float GetTime()
    {
        return countdownTime;
    }

    /// <summary>
    /// スライドするUIを制御する処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator SlideUI()
    {
        yield return new WaitForSeconds(1.0f);
        slideUIController.state = 1;
        yield return new WaitForSeconds(3.0f);
        slideUIController.state = 2;
        yield return new WaitForSeconds(1.0f);
    }

    /// <summary>
    /// 警戒度レベルを上げる
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseAlertLevel(float amount)
    {
        AlertLevel += amount;

        if(AlertLevel >= 100)
        {
            ToFailedScene();
        }
    }

    /// <summary>
    /// 警戒度を外部から取得するときに使うメソッド
    /// </summary>
    /// <returns></returns>
    public float GetAlertLevel()
    {
        return AlertLevel;
    }

    /// <summary>
    /// クリアシーンに移動するときの処理
    /// </summary>
    public void ToClearScene()
    {
        SceneManager.LoadScene("Clear");
    }

    /// <summary>
    /// ミスシーンに移動するときの処理
    /// </summary>
    public void ToFailedScene()
    {
        SceneManager.LoadScene("Failed");
    }
    #endregion

    #region 設定
    private float volume;
    private float cameraSpeed;
    private float bright;

    public void SetVolume(float volume)
    {
        this.volume = volume;
    }
    public float GetVolume()
    {
        return volume;
    }

    public void SetCameraSpeed(float cameraSpeed)
    {
        this.cameraSpeed = cameraSpeed;
    }
    public float GetCameraSpeed()
    {
        return cameraSpeed;
    }

    public void SetBright(float bright)
    {
        this.bright = bright;
    }
    public float GetBright()
    {
        return bright;
    }
    #endregion
}
