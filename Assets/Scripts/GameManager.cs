using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //GameManager�̃C���X�^���X���擾���邽�߂̃v���p�e�B
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // GameManager���A�^�b�`����Ă���GameObject���쐬���A�����GameManager�R���|�[�l���g���A�^�b�`����
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go); // �V�[���ؑ֎��ɔj������Ȃ��悤�ɐݒ�
            }
            return instance;
        }
    }

    private void Awake()
    {
        // ���ɃC���X�^���X�����݂���ꍇ�́A���g��j������
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region �^�C�g���V�[��
    public void ToGameScene()
    {
        SceneManager.LoadScene("Game");
    }
    #endregion

    #region ���C���V�[��
    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    private float AlertLevel = 0;

    /// <summary>
    /// �`�F�b�N�|�C���g��o�^����
    /// </summary>
    public void RegisterCheckPoint(CheckPoint checkPoint)
    {
        if (!checkPoints.Contains(checkPoint))
        {
            checkPoints.Add(checkPoint);
        }
    }

    /// <summary>
    /// �S�Ẵ`�F�b�N�|�C���g�����e���ꂽ���m�F����
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
        //Debug.Log($"�x���x:{AlertLevel}");
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
