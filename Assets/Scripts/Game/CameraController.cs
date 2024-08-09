using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float mouseInsensity = 1f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private const float Y_ANGLE_MIN = -20.0f;
    private const float Y_ANGLE_MAX = 80.0f;

    private Vector3 cameraOffset = new Vector3(0, 2, 0);
    private float distanceToPlayer = 7.0f; // �v���C���[�Ƃ̋���

    private void Start()
    {
        // �}�E�X�J�[�\�������b�N���A��\���ɂ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float settingValue = GameManager.Instance.GetCameraSpeed();

        mouseInsensity = settingValue * settingValue + 0.5f * settingValue + 0.5f;
    }

    void Update()
    {
        // �}�E�X�̈ړ��ʂ��擾
        currentX += Input.GetAxis("Mouse X") * mouseInsensity;
        currentY -= Input.GetAxis("Mouse Y") * mouseInsensity;

        // Y�����̉�]�p�x�𐧌�
        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        // �v���C���[�̕���������
        Vector3 direction = new Vector3(0, 0, -distanceToPlayer);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = player.transform.position + rotation * direction;

        // �v���C���[�̕���������
        transform.LookAt(player.transform.position + cameraOffset);
    }
}
