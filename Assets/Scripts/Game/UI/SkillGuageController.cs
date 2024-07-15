using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillGuageController : MonoBehaviour
{
    #region SerializeField
    [Header("スキルゲージ")]
    [SerializeField] private Image progressRing;
    [Header("スタックを表示するためのテキスト")]
    [SerializeField] private TextMeshProUGUI stackText;
    [Header("スキルゲージが溜まるまでの時間(秒)")]
    [SerializeField] private float waitTime = 1.0f;
    [Header("スキルゲージが溜まった時リセットまでの待機時間(秒)")]
    [SerializeField] private float finishDelay = 1.0f;
    [HideInInspector]public Action<int> OnStackAccumulated;
    #endregion

    #region private変数
    private bool progress;
    private int stackCount = 3;
    #endregion

    private void Start()
    {
        CancelTimer();
        ResetTimer();
        UpdateStackUI();
        StartTimer();
    }

    private void Update()
    {
        if(progress)
        {
            progressRing.fillAmount += 1.0f / waitTime * Time.deltaTime;

            if(1 <= progressRing.fillAmount)
            {
                progress = false;

                if(stackCount < 9)
                {
                    StartCoroutine(ResetProgressRing());
                }
                else
                {
                    progressRing.fillAmount = 1f;
                }
            }
        }
    }

    /// <summary>
    /// スキルゲージが溜まった時に呼ぶ処理
    /// </summary>
    /// <returns>スタック数(Invoke)</returns>
    private IEnumerator ResetProgressRing()
    {
        yield return new WaitForSeconds(finishDelay);
        stackCount++;
        UpdateStackUI();
        OnStackAccumulated?.Invoke(stackCount);
        
        if(stackCount < 9)
        {
            ResetTimer();
            progress = true;
        }
    }

    /// <summary>
    /// スタックのUIの更新
    /// </summary>
    private void UpdateStackUI()
    {
        if (stackText != null)
        {
            stackText.text = $"{stackCount}";
        }
    }

    /// <summary>
    /// 他のクラスがスタック数を得るときに使う関数
    /// </summary>
    /// <returns>スタック数</returns>
    public int GetStackCount()
    {
        return stackCount;
    }

    /// <summary>
    /// スタック数を他のクラスから減らすときに呼ぶ関数
    /// </summary>
    public bool DecreasedStackCount(int decreaseNum)
    {
        bool stackDecreased = false;

        if(stackCount > decreaseNum - 1)
        {
            stackCount -= decreaseNum;
            stackDecreased = true;
        }

        if(stackDecreased)
        {
            UpdateStackUI();
        }
        else
        {
            Debug.Log("スタックが足りません！");
        }

        if(stackCount < 9 && !progress)
        {
            ResetTimer();
            progress = true;
        }

        return stackDecreased;
    }

    /// <summary>
    /// タイマーをスタートさせるときに呼ぶ処理
    /// </summary>
    public void StartTimer()
    {
        if(stackCount < 9)
        {
            progress = true;
        }
    }

    /// <summary>
    /// タイマーを止めるときに呼ぶ処理
    /// </summary>
    public void CancelTimer()
    {
        progress = false;
    }

    /// <summary>
    /// タイマーをリセットするときに呼ぶ処理
    /// </summary>
    public void ResetTimer()
    {
        progressRing.fillAmount = 0;
    }
}
