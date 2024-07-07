using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<CheckPoint> checkPoints = new List<CheckPoint>();

    private void Awake()
    {
        //シングルトンパターンの実装
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// チェックポイントを呼ぶときの処理
    /// </summary>
    /// <param name="checkPoint">チェックポイントのインスタンス</param>
    public void RegisterCheckPoint(CheckPoint checkPoint)
    {
        if(!checkPoints.Contains(checkPoint))
        {
            checkPoints.Add(checkPoint);
        }
    }

    /// <summary>
    /// 全てのチェックポイントが制覇されたかどうかを確認する
    /// </summary>
    public void CheckAllCheckPointsCaptured()
    {
        //checkPointsリストに入っているチェックポイント全
        foreach(CheckPoint checkPoint in checkPoints)
        {
            if(!checkPoint.IsCaptured)
            {
                return;
            }
        }
        Debug.Log("クリア");
    }
}
