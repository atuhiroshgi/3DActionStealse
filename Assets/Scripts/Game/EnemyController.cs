using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Character
{
    #region SerializeField
    [Header("パトロールポイント")]
    [SerializeField] private Transform[] patrolPoints;
    [Header("Playerを格納")]
    [SerializeField] private PlayerController player;
    [Header("パトロール速度")]
    [SerializeField] private float patrolSpeed = 2f;
    [Header("パトロールポイントでの待機時間")]
    [SerializeField] private float waitTimeAtPoint = 2f;
    [Header("ターゲットに対する回転の滑らかさ")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [Header("最大回転速度")]
    [SerializeField] private float maxRotationSpeed = 360f;
    [Header("視認距離")]
    [SerializeField] private float sightRange = 10f;
    [Header("視認角度")]
    [SerializeField] private float sightAngle = 45f;
    [Header("警戒度増加量")]
    [SerializeField] private float alertIncreaseAmount;
    #endregion

    #region private
    private int currentPatrolIndex;
    private float waitTimer;
    private float currentAngularVelocity;
    private bool playerInSight;
    private bool isWaiting;
    #endregion

    protected override void Init()
    {
        currentPatrolIndex = 0;
        waitTimer = 0f;
        currentAngularVelocity = 0f;
        isWaiting = false;
        playerInSight = false;
    }

    private void Update()
    {
        CheckPlayerInSight();
        GameManager.Instance.SetInSight(playerInSight);

        animator.SetBool("isMoving", !isWaiting);

        //プレイヤーが視界に入っているときパトロールしない
        if(playerInSight || isDead)
        {
            return;
        }

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

        if(player.isCharging && !player.onceAttack && player.isChargeFound)
        {
            TakeDamage(50);
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        Vector3 directionToTarget = (targetPoint.position - transform.position).normalized;

        // 回転処理
        Rotate(directionToTarget, rotationSmoothTime, ref currentAngularVelocity, maxRotationSpeed);

        // 移動処理
        Move(directionToTarget, patrolSpeed);

        // ターゲットポイントに到達したかどうかのチェック
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            isWaiting = true;
        }
    }

    private void CheckPlayerInSight()
    {
        if (player == null)
        {
            return;
        }

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
                    isWaiting = true;
                    playerInSight = true;

                    if (player.isHuge)
                    {
                        Die();
                        return;
                    }

                    if(isDead)
                    {
                        return;
                    }

                    float adjustedIncrease = 0;
                    adjustedIncrease += alertIncreaseAmount;
                    GameManager.Instance.IncreaseAlertLevel(adjustedIncrease);
                    //Debug.Log("プレイヤーを発見！ 警戒度が増加しました。");

                    if (player.isAttack && !player.onceAttack)
                    {
                        TakeDamage(50);
                    }
                }
            }
        }
        else
        {
            playerInSight = false;
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if(currentHP > 0)
        {
            animator.SetTrigger("Damage");
        }
    }

    protected override void Die()
    {
        base.Die();
        StartCoroutine(PlayDownAndWakeUpAnimation());
    }

    private IEnumerator PlayDownAndWakeUpAnimation()
    {
        animator.SetTrigger("DownBack");
        yield return new WaitForSeconds(0.8f);

        animator.SetTrigger("Down");
        yield return new WaitForSeconds(8f);
        
        animator.SetTrigger("WakeUp");
        yield return new WaitForSeconds(0.8f);

        CancelAnimation();
        currentHP = maxHP;
        isDead = false;
    }

    public void CancelAnimation()
    {
        animator.SetTrigger("Idle");
    }
}
