using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 currentPos;
    private Vector3 pastPos;
    private Vector3 diff;
    private float distanceToPlayer = 46.25f;

    private void Start()
    {
        pastPos = player.transform.position;
    }

    void Update()
    {
        currentPos = player.transform.position;
        diff = currentPos - pastPos;

        // �J�����ʒu�̍X�V
        transform.position = Vector3.Lerp(transform.position, transform.position + diff, 1.0f);

        // �}�E�X�̈ړ��ʂ��擾
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // X�����Ɉ��ʈړ����Ă���Ή���]
        if (Mathf.Abs(mx) > 0.01f)
        {
            // ��]���̓��[���h���W��Y��
            transform.RotateAround(player.transform.position, Vector3.up, mx);
        }

        // Y�����Ɉ��ʈړ����Ă���Ώc��]
        if (Mathf.Abs(my) > 0.01f)
        {
            // ��]���̓J�������g��X��
            transform.RotateAround(player.transform.position, transform.right, -my);
        }

        // Player����̋��������ɂ���
        Vector3 cameraToPlayer = transform.position - player.transform.position;
        float desiredDistance = Mathf.Sqrt(distanceToPlayer);
        if (cameraToPlayer.magnitude > desiredDistance)
        {
            cameraToPlayer = cameraToPlayer.normalized * desiredDistance;
            transform.position = player.transform.position + cameraToPlayer;
        }

        pastPos = currentPos;
    }
}
