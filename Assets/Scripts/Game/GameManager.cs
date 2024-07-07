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
        //�V���O���g���p�^�[���̎���
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
    /// �`�F�b�N�|�C���g���ĂԂƂ��̏���
    /// </summary>
    /// <param name="checkPoint">�`�F�b�N�|�C���g�̃C���X�^���X</param>
    public void RegisterCheckPoint(CheckPoint checkPoint)
    {
        if(!checkPoints.Contains(checkPoint))
        {
            checkPoints.Add(checkPoint);
        }
    }

    /// <summary>
    /// �S�Ẵ`�F�b�N�|�C���g�����e���ꂽ���ǂ������m�F����
    /// </summary>
    public void CheckAllCheckPointsCaptured()
    {
        //checkPoints���X�g�ɓ����Ă���`�F�b�N�|�C���g�S
        foreach(CheckPoint checkPoint in checkPoints)
        {
            if(!checkPoint.IsCaptured)
            {
                return;
            }
        }
        Debug.Log("�N���A");
    }
}
