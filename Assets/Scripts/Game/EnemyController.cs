using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Character
{
    #region SerializeField
    [Header("�p�g���[���|�C���g")]
    [SerializeField] private Transform[] patrolPoints;
    [Header("Player���i�[")]
    [SerializeField] private PlayerController player;
    [Header("�p�g���[�����x")]
    [SerializeField] private float patrolSpeed = 2f;
    [Header("�p�g���[���|�C���g�ł̑ҋ@����")]
    [SerializeField] private float waitTimeAtPoint = 2f;
    [Header("�^�[�Q�b�g�ɑ΂����]�̊��炩��")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [Header("�ő��]���x")]
    [SerializeField] private float maxRotationSpeed = 360f;
    [Header("���F����")]
    [SerializeField] private float sightRange = 10f;
    [Header("���F�p�x")]
    [SerializeField] private float sightAngle = 45f;
    [Header("�x���x������")]
    [SerializeField] private float alertIncreaseAmount;
    #endregion

    #region private
    private int currentPatrolIndex;
    private bool isWaiting;
    private float waitTimer;
    private float currentAngularVelocity;
    #endregion

    protected override void Init()
    {
        currentPatrolIndex = 0;
        isWaiting = false;
        waitTimer = 0f;
        currentAngularVelocity = 0f;
    }

    private void Update()
    {
        if (!isWaiting)
        {
            Patrol();
        }
        else
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtPoint)
            {
                isWaiting = false;
                waitTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
        }

        animator.SetBool("isMoving", !isWaiting);
        CheckPlayerInSight();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector3 directionToTarget = (targetPoint.position - transform.position).normalized;

        // ��]����
        Rotate(directionToTarget, rotationSmoothTime, ref currentAngularVelocity, maxRotationSpeed);

        // �ړ�����
        Move(directionToTarget, patrolSpeed);

        // �^�[�Q�b�g�|�C���g�ɓ��B�������ǂ����̃`�F�b�N
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            isWaiting = true;
        }
    }

    private void CheckPlayerInSight()
    {
        if (player == null)
            return;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (angleToPlayer < sightAngle / 2 && distanceToPlayer < sightRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
            {
                if (hit.transform.GetComponent<PlayerController>() && !player.isHidden)
                {
                    float adjustedIncrease = 0;
                    adjustedIncrease += alertIncreaseAmount;
                    GameManager.Instance.IncreaseAlertLevel(adjustedIncrease);
                    Debug.Log("�v���C���[�𔭌��I �x���x���������܂����B");
                }
            }
        }
    }
}
