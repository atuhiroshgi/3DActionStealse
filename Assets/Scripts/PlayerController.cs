using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayers;        //�n�ʔ�������邽�߂̃��C���[
    [SerializeField] private float moveSpeedIn;             //�v���C���[�̈ړ����x�����
    [SerializeField] private float maxAngVelo;              //�ő�̉�]�p���x
    [SerializeField] private float smoothTime = 0.1f;       //�i�s�����ɂ����鎞��
    [SerializeField] private float jumpForce;          //�W�����v�̍��������
    #endregion

    #region private�ϐ�
    private Rigidbody rb;
    private Vector3 moveSpeed;          //�v���C���[�̈ړ����x
    private Vector3 currentPos;         //�v���C���[�̌��݂̈ʒu
    private Vector3 pastPos;            //�v���C���[�̉ߋ��̈ʒu
    private Vector3 delta;              //�v���C���[�̈ړ���
    private Quaternion playerRot;       //�v���C���[�̊p�x
    private Quaternion nextRot;         //�ǂꂾ����]���邩
    private float currentAngVelo;       //���݂̉�]�p���x
    private float diffAngle;            //���݂̌����Ɛi�s�����̊p�x
    private float rotAngle;             //���݂̉�]����p�x
    private float attackDuration = 0.8f;//�U���A�j���[�V�����̒���
    private float chargeDuration = 0.6f;//�`���[�W�A�j���[�V�����̒���
    private float damageDuration = 0.6f;//�_���[�W�A�j���[�V�����̒���
    private bool isGround;              //�ڒn���Ă��邩�ǂ���
    private bool isAttack = false;      //�U�������ǂ���
    private bool isCharging = false;    //�`���[�W�����ǂ���
    private bool isDamage = false;      //�_���[�W���󂯂Ă��邩�ǂ���
    private bool isRun = false;         //�ړ������ǂ���
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pastPos = transform.position;
    }

    private void Update()
    {
        RotatePlayer();
        Jump();
        PlayAnim();
        MovePlayer();
        //�f�o�b�O�p
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage();
        }
    }

    /// <summary>
    /// �v���C���[�̓����𐧌�
    /// </summary>
    private void MovePlayer()
    {
        //�J�����ɑ΂��đO���擾
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //�J�����ɑ΂��ĉE���擾
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        moveSpeed = Vector3.zero;

        //�ړ�����
        Vector3 moveDirection = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            moveDirection += cameraForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= cameraForward;
        }
        if(Input.GetKey(KeyCode.A))
        {
            moveDirection -= cameraRight;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += cameraRight;
        }

        //�ړ������ɑ��x�������Ĉړ�
        rb.MovePosition(transform.position + moveDirection.normalized * moveSpeedIn * Time.deltaTime);

        isRun = moveDirection != Vector3.zero;
    }

    /// <summary>
    /// �v���C���[�̌����̐���
    /// </summary>
    private void RotatePlayer()
    {
        //���݂̈ʒu
        currentPos = transform.position;

        //�ړ��ʂ̌v�Z
        delta = currentPos - pastPos;
        delta.y = 0;

        //�ߋ��̈ʒu�̍X�V
        pastPos = currentPos;

        if (delta == Vector3.zero)
        {
            return;
        }

        playerRot = Quaternion.LookRotation(delta, Vector3.up);

        diffAngle = Vector3.Angle(transform.forward, delta);

        //Vector3.SmoothDamp�Œl�����X�ɕω�
        rotAngle = Mathf.SmoothDampAngle(0, diffAngle, ref currentAngVelo, smoothTime, maxAngVelo);

        nextRot = Quaternion.RotateTowards(transform.rotation, playerRot, rotAngle);

        transform.rotation = nextRot;
    }

    /// <summary>
    /// �W�����v����Ƃ��ɌĂԊ֐�
    /// </summary>
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGround = false;
            Debug.Log("�W�����v");
        }
    }

    /// <summary>
    /// �A�j���[�V�����̍Đ�
    /// </summary>
    private void PlayAnim()
    {
        if (Input.GetMouseButtonDown(0) && !isAttack && !isCharging)
        {
            Attack();
        }

        if (Input.GetMouseButton(1) && !isAttack && !isCharging)
        {
            ChargeAttack();
        }

        if (isDamage)
        {
            TakeDamage();
        }

        animator.SetBool("isRun", isRun);
    }

    /// <summary>
    /// �U���̏���
    /// </summary>
    private void Attack()
    {
        isAttack = true;
        Debug.Log("�U��");
        animator.SetTrigger("Attack");
        StartCoroutine(ResetBoolAfterDelay("isAttack", attackDuration));
    }

    /// <summary>
    /// ���ߍU���̏���
    /// </summary>
    private void ChargeAttack()
    {
        isCharging = true;
        animator.SetTrigger("Charge");
        StartCoroutine(ResetBoolAfterDelay("isCharging", chargeDuration));

        Debug.Log("���ߍU��");
    }

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��̏���
    /// </summary>
    private void TakeDamage()
    {
        if (!isDamage)
        {
            isDamage = true;
            animator.SetTrigger("Damage");
            StartCoroutine(ResetBoolAfterDelay("isDamage", damageDuration));

            Debug.Log("�_���[�W");
        }
    }

    /// <summary>
    /// �A�j���[�V�����̎��ԑ҂��߂̊֐�
    /// </summary>
    /// <param name="boolName">�A�j���[�V�������Ǘ�����bool�ϐ�</param>
    /// <param name="delay">�o�߂����鎞��</param>
    /// <returns></returns>
    private IEnumerator ResetBoolAfterDelay(string boolName, float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (boolName)
        {
            case "isAttack":
                isAttack = false;
                break;

            case "isCharging":
                isCharging = false;
                break;

            case "isDamage":
                isDamage = false;
                break;
        }

        animator.SetTrigger("Idle");
    }

    #region �ڒn����
    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayers & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("�ڒn");
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //if ((groundLayers & (1 << collision.gameObject.layer)) != 0)
        //{
          //  Debug.Log("����");
            //isGround = false;
        //}
    }
    #endregion
}
