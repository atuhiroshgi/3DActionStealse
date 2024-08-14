using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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
    private string skillName;
    private int selectedIndex;
    private bool isMoving;

    public void ToGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void SetSkill(string skillName)
    {
        this.skillName = skillName;
    }

    public string GetSkill()
    {
        return skillName;
    }

    public void SetSelectedIndex(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public void SetGhostMoving(bool isMoving)
    {
        this.isMoving = isMoving;
    }

    public bool GetGhostMoving()
    {
        return isMoving;
    }

    #endregion

    #region メインシーン
    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    private float AlertLevel = 0;
    private float countdownTime = 180;
    private bool playerInSight = false;
    private bool startFlag = false;

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
        if (countdownTime >= 1 && startFlag)
        {
            countdownTime -= Time.deltaTime;
        }
    }

    public void SetTime(float time)
    {
        countdownTime = time;
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

    public void SetInSight(bool playerInSight)
    {
        this.playerInSight = playerInSight;
    }

    public bool GetInSight()
    {
        return playerInSight;
    }

    public void SetStartFlag(bool startFlag)
    {
        this.startFlag = startFlag;
    }

    public bool GetStartFlag()
    {
        return startFlag;
    }

    #endregion

    #region リザルトシーン
    
    public void ToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    #endregion

    #region 設定
    public event Action<float> OnVolumeChanged;
    private float volume = 1;
    private float cameraSpeed;
    private float bright;

    public void SetVolume(float volume)
    {
        this.volume = volume;
        OnVolumeChanged?.Invoke(volume);
    }
    public float GetVolume()
    {
        return volume;
    }

    public void SetCameraSpeed(float cameraSpeed)
    {
        this.cameraSpeed = cameraSpeed;
        Debug.Log(this.cameraSpeed);
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
