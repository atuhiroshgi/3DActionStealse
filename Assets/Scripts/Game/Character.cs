using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    #region SerializeField
    [Header("�}�e���A����ύX����I�u�W�F�N�g")]
    [SerializeField] protected GameObject SkinObject;
    [Header("�A�j���[�^�[")]
    [SerializeField] protected Animator animator;
    [Header("HP")]
    [SerializeField] protected int maxHP;
    #endregion

    #region Protected Variables
    protected Rigidbody rb;
    protected SkinnedMeshRenderer skinnedMR;
    protected int currentHP;
    protected bool isDead = false;
    protected bool isDamage = false;
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        skinnedMR = SkinObject.GetComponent<SkinnedMeshRenderer>();
        currentHP = maxHP;

        Init();
    }

    protected abstract void Init();

    public virtual void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHP -= damage;
        
        if (currentHP <= 0)
        {
            Debug.Log("��[��[��[���[");
            Die();
        }
        else
        {
            animator.SetTrigger("Damage");
            isDamage = true;
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        // �����Ŏ��S������ǉ�
    }

    protected void Move(Vector3 direction, float speed)
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    protected void Rotate(Vector3 targetDirection, float smoothTime, ref float currentAngVelo, float maxAngVelo)
    {
        if (targetDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        float diffAngle = Vector3.Angle(transform.forward, targetDirection);
        float rotAngle = Mathf.SmoothDampAngle(0, diffAngle, ref currentAngVelo, smoothTime, maxAngVelo);
        Quaternion nextRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotAngle);

        transform.rotation = nextRotation;
    }
}
