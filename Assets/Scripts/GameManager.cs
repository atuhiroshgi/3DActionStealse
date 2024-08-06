using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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
    /// �O������c�莞�Ԃ��擾
    /// </summary>
    /// <returns>�c�莞��</returns>
    public float GetTime()
    {
        return countdownTime;
    }

    /// <summary>
    /// �X���C�h����UI�𐧌䂷�鏈��
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
    #endregion

    #region �ݒ�
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
