using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour
{
    #region SerializeField
    [Header("UI��Image���擾")]
    [SerializeField] private Image checkPointUI;
    [Header("�ǂ̃`�F�b�N�|�C���g�������ʂ���ԍ�")]
    [SerializeField] private int checkPointindex;
    [Header("���e����̂ɂ����鎞��")]
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
    private float captureProgress = 0f; //�`�F�b�N�|�C���g�̐��e�i��
    private bool isCaptured = false;    //�`�F�b�N�|�C���g�����e����Ă��邩�ǂ���
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
    /// �v���C���[��E�L�[�������Ă���Ԃ̐i������
    /// </summary>
    private void HandleCaptureProgress()
    {
        //E�L�[��������Ă���Ԃ̂ݐi�������Z
        if (Input.GetKey(KeyCode.E))
        {
            captureProgress += Time.deltaTime;
        }

        //�i�������e���Ԃ𒴂����琧�e����
        if(captureProgress > captureDuration)
        {
            CaptureCheckPoint();
            currentState = CheckpointState.Idle;
        }
    }

    /// <summary>
    /// E�L�[�𗣂����Ƃ��̃��Z�b�g����
    /// </summary>
    private void HandleReleaseProgress()
    {
        //E�L�[�������ꂽ�烊�Z�b�g�J�n
        if (!Input.GetKey(KeyCode.E))
        {
            captureProgress -= Time.deltaTime;
        }

        //�i����0�ȉ��ɂȂ����烊�Z�b�g����
        if(captureProgress <= 0f)
        {
            currentState = CheckpointState.Idle;
            captureProgress = 0f;
        }
    }

    /// <summary>
    /// �`�F�b�N�|�C���g�����e���ꂽ�Ƃ��̏���
    /// </summary>
    private void CaptureCheckPoint()
    {
        isCaptured = true;
        checkPointUI.color = new Color32(242, 108, 216, 255);
    }

    #region �ڐG����
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
        //�ڐG�I�����̏���
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            currentState = CheckpointState.Releasing;
        }
    }
    #endregion
}
