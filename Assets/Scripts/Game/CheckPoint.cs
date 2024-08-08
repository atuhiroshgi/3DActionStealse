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
    [Header("進捗表示のための円形ゲージ")]
    [SerializeField] private Image progressCircleUI;
    [Header("制覇前にUIに表示するアイコン画像")]
    [SerializeField] private Sprite defaultIcon;
    [Header("制覇後にUIに表示するアイコン画像")]
    [SerializeField] private Sprite captureIcon;
    [Header("チェックポイントの上の丸いやつ")]
    [SerializeField] private GameObject sphereObject;
    [Header("プレイヤー接触時に出す補助UI")]
    [SerializeField] private GameObject recommendUI;
    [Header("制覇前のマテリアル")]
    [SerializeField] private Material beforeMaterial;
    [Header("制覇後のマテリアル")]
    [SerializeField] private Material afterMaterial;
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

    //IsCapturedをisCapturedの読み取り専用プロパティにする
    public bool IsCaptured => isCaptured;
    #endregion

    #region private
    private MeshRenderer meshRenderer;      //丸いやつのMeshRendererを参照するため
    private CheckpointState currentState = CheckpointState.Idle;
    private float captureProgress = 0f;     //チェックポイントの制覇進捗
    private bool isCaptured = false;        //チェックポイントが制覇されているかどうか
    #endregion

    private void Start()
    {
        meshRenderer = sphereObject.GetComponent<MeshRenderer>();

        //GameManagerにこのチェックポイントを登録
        GameManager.instance.RegisterCheckPoint(this);

        Init();
    }

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

        //進捗ゲージのUI更新
        UpdateProgressUI();
    }

    private void Init()
    {
        //初期状態ではおすすめUIを非表示に設定
        recommendUI.SetActive(false);
        //初期状態では進捗ゲージを非表示に設定
        progressCircleUI.gameObject.SetActive(false);

        checkPointUI.sprite = defaultIcon;
        meshRenderer.material = beforeMaterial;
        progressCircleUI.fillAmount = 0f;
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
            Debug.Log($"進捗中:{captureProgress / captureDuration * 100}%");
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
            Debug.Log($"リセット中: {captureProgress / captureDuration * 100}%");
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
        AudioManager.Instance.PlaySFX("Capture");

        //チェックポイントのマテリアルを黄色に更新
        meshRenderer.material = afterMaterial;

        //進捗UIを非表示に設定
        progressCircleUI.gameObject.SetActive(false);
        
        checkPointUI.sprite = captureIcon;

        //GameManagerにチェックポイントが制覇されたことを通知
        GameManager.instance.CheckAllCheckPointsCaptured();
    }

    /// <summary>
    /// 進捗UIを更新
    /// </summary>
    private void UpdateProgressUI()
    {
        if (isCaptured)
        {
            return;
        }

        if(currentState == CheckpointState.Capturing || currentState == CheckpointState.Releasing)
        {
            progressCircleUI.gameObject.SetActive(true);
            progressCircleUI.fillAmount = captureProgress / captureDuration;
        }
    }

    #region 接触判定
    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(isCaptured)
            {
                recommendUI.gameObject.SetActive(false);
                return;
            }

            //プレイヤー接触時にUIを表示
            recommendUI.SetActive(true);

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
            recommendUI.SetActive(false);

            currentState = CheckpointState.Releasing;
        }
    }
    #endregion
}
