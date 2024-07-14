using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    private float AlertLevel = 0;

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

    public void IncreaseAlertLevel(float amount)
    {
        AlertLevel += amount;
        //Debug.Log($"警戒度:{AlertLevel}");
    }

    public float GetAlertLevel()
    {
        return AlertLevel;
    }

    public void ToClearScene()
    {
        SceneManager.LoadScene("Clear");
    }

    public void ToFailedScene()
    {
        SceneManager.LoadScene("Failed");
    }
    #endregion
}
