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
    [Header("�X�L���Q�[�W")]
    [SerializeField] private Image progressRing;
    [Header("�X�^�b�N��\�����邽�߂̃e�L�X�g")]
    [SerializeField] private TextMeshProUGUI stackText;
    [Header("�X�L���Q�[�W�����܂�܂ł̎���(�b)")]
    [SerializeField] private float waitTime = 1.0f;
    [Header("�X�L���Q�[�W�����܂��������Z�b�g�܂ł̑ҋ@����(�b)")]
    [SerializeField] private float finishDelay = 1.0f;
    [HideInInspector]public Action<int> OnStackAccumulated;
    #endregion

    #region private�ϐ�
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
    /// �X�L���Q�[�W�����܂������ɌĂԏ���
    /// </summary>
    /// <returns>�X�^�b�N��(Invoke)</returns>
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
    /// �X�^�b�N��UI�̍X�V
    /// </summary>
    private void UpdateStackUI()
    {
        if (stackText != null)
        {
            stackText.text = $"{stackCount}";
        }
    }

    /// <summary>
    /// ���̃N���X���X�^�b�N���𓾂�Ƃ��Ɏg���֐�
    /// </summary>
    /// <returns>�X�^�b�N��</returns>
    public int GetStackCount()
    {
        return stackCount;
    }

    /// <summary>
    /// �X�^�b�N���𑼂̃N���X���猸�炷�Ƃ��ɌĂԊ֐�
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
            Debug.Log("�X�^�b�N������܂���I");
        }

        if(stackCount < 9 && !progress)
        {
            ResetTimer();
            progress = true;
        }

        return stackDecreased;
    }

    /// <summary>
    /// �^�C�}�[���X�^�[�g������Ƃ��ɌĂԏ���
    /// </summary>
    public void StartTimer()
    {
        if(stackCount < 9)
        {
            progress = true;
        }
    }

    /// <summary>
    /// �^�C�}�[���~�߂�Ƃ��ɌĂԏ���
    /// </summary>
    public void CancelTimer()
    {
        progress = false;
    }

    /// <summary>
    /// �^�C�}�[�����Z�b�g����Ƃ��ɌĂԏ���
    /// </summary>
    public void ResetTimer()
    {
        progressRing.fillAmount = 0;
    }
}
