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
    [Header("�i���\���̂��߂̉~�`�Q�[�W")]
    [SerializeField] private Image progressCircleUI;
    [Header("�`�F�b�N�|�C���g�̏�̊ۂ����")]
    [SerializeField] private GameObject sphereObject;
    [Header("�v���C���[�ڐG���ɏo���⏕UI")]
    [SerializeField] private GameObject recommendUI;
    [Header("���e�O�̃}�e���A��")]
    [SerializeField] private Material beforeMaterial;
    [Header("���e��̃}�e���A��")]
    [SerializeField] private Material afterMaterial;
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

    //IsCaptured��isCaptured�̓ǂݎ���p�v���p�e�B�ɂ���
    public bool IsCaptured => isCaptured;
    #endregion

    #region private
    private MeshRenderer meshRenderer;      //�ۂ����MeshRenderer���Q�Ƃ��邽��
    private CheckpointState currentState = CheckpointState.Idle;
    private float captureProgress = 0f;     //�`�F�b�N�|�C���g�̐��e�i��
    private bool isCaptured = false;        //�`�F�b�N�|�C���g�����e����Ă��邩�ǂ���
    private bool isPlayerInContact = false; //�v���C���[���ڐG���Ă��邩�ǂ���
    #endregion

    private void Start()
    {
        meshRenderer = sphereObject.GetComponent<MeshRenderer>();
        Init();
        recommendUI.SetActive(false);                   //������Ԃł͂�������UI���\���ɐݒ�
        progressCircleUI.gameObject.SetActive(false);   //������Ԃł͐i���Q�[�W���\���ɐݒ�

        //GameManager�ɂ��̃`�F�b�N�|�C���g��o�^
        GameManager.instance.RegisterCheckPoint(this);
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

        //�i���Q�[�W��UI�X�V
        UpdateProgressUI();
    }

    private void Init()
    {
        meshRenderer.material = beforeMaterial;
        progressCircleUI.fillAmount = 0f;
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
            Debug.Log($"�i����:{captureProgress / captureDuration * 100}%");
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
            Debug.Log($"���Z�b�g��: {captureProgress / captureDuration * 100}%");
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
        
        //�`�F�b�N�|�C���g��UI���s���N�ɍX�V
        checkPointUI.color = new Color32(242, 108, 216, 255);

        //�`�F�b�N�|�C���g�̃}�e���A�������F�ɍX�V
        meshRenderer.material = afterMaterial;

        //�i��UI���\���ɐݒ�
        progressCircleUI.gameObject.SetActive(false);

        //GameManager�Ƀ`�F�b�N�|�C���g�����e���ꂽ���Ƃ�ʒm
        GameManager.instance.CheckAllCheckPointsCaptured();
    }

    /// <summary>
    /// �i��UI���X�V
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

    #region �ڐG����
    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(isCaptured)
            {
                recommendUI.gameObject.SetActive(false);
                return;
            }

            //�v���C���[�ڐG����UI��\��
            isPlayerInContact = true;
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
        //�ڐG�I�����̏���
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //�v���C���[�����ꂽ��UI���\��
            isPlayerInContact = false;
            recommendUI.SetActive(false);

            currentState = CheckpointState.Releasing;
        }
    }
    #endregion
}
