using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    #region SerializeField
    [Header("UIのImageを取得")]
    [SerializeField] private Image checkPointUI;
    [Header("どのチェックポイントかを識別する番号")]
    [SerializeField] private int checkPointindex;
    [Header("制覇するのにかかる時間")]
    [SerializeField] private float captureDuration = 5f;
    #endregion

    #region public
    public enum CheckpointState
    {
        Idle,
        Capturing,
        Releasing
    }
    #endregion

    #region private
    private CheckpointState currentState = CheckpointState.Idle;
    private float captureProgress = 0f; //チェックポイントの制覇進捗
    private bool isCaptured = false;    //チェックポイントが制覇されているかどうか
    #endregion

    private void Update()
    {
        switch(currentState)
        {
            case CheckpointState.Idle:
                break;

            case CheckpointState.Capturing:
                HandleCaptureProgress();
                break;

            case CheckpointState.Releasing:
                HandleReleaseProgress();
                break;
        }
    }

    /// <summary>
    /// プレイヤーがEキーを押している間の進捗処理
    /// </summary>
    private void HandleCaptureProgress()
    {
        //Eキーが押されている間のみ進捗を加算
        if (Input.GetKey(KeyCode.E))
        {
            captureProgress += Time.deltaTime;
        }

        //進捗が制覇時間を超えたら制覇完了
        if(captureProgress > captureDuration)
        {
            CaptureCheckPoint();
            currentState = CheckpointState.Idle;
        }
    }

    /// <summary>
    /// Eキーを離したときのリセット処理
    /// </summary>
    private void HandleReleaseProgress()
    {
        //Eキーが離されたらリセット開始
        if (!Input.GetKey(KeyCode.E))
        {
            captureProgress -= Time.deltaTime;
        }

        //進捗が0以下になったらリセット完了
        if(captureProgress <= 0f)
        {
            currentState = CheckpointState.Idle;
            captureProgress = 0f;
        }
    }

    /// <summary>
    /// チェックポイントが制覇されたときの処理
    /// </summary>
    private void CaptureCheckPoint()
    {
        isCaptured = true;
        checkPointUI.color = new Color32(242, 108, 216, 255);
    }

    #region 接触判定
    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                currentState = CheckpointState.Capturing;
            }
            else
            {
                currentState = CheckpointState.Releasing;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        //接触終了時の処理
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            currentState = CheckpointState.Releasing;
        }
    }
    #endregion
}
