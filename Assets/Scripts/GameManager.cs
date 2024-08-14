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

    #region ���C���V�[��
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

    /// <summary>
    /// �J�E���g�_�E�����鏈��
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
    /// �O������c�莞�Ԃ��擾
    /// </summary>
    /// <returns>�c�莞��</returns>
    public float GetTime()
    {
        return countdownTime;
    }

    /// <summary>
    /// �x���x���x�����グ��
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
    /// �x���x���O������擾����Ƃ��Ɏg�����\�b�h
    /// </summary>
    /// <returns></returns>
    public float GetAlertLevel()
    {
        return AlertLevel;
    }

    /// <summary>
    /// �N���A�V�[���Ɉړ�����Ƃ��̏���
    /// </summary>
    public void ToClearScene()
    {
        SceneManager.LoadScene("Clear");
    }

    /// <summary>
    /// �~�X�V�[���Ɉړ�����Ƃ��̏���
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

    #region ���U���g�V�[��
    
    public void ToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }

    #endregion

    #region �ݒ�
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
