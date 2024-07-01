using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private GameObject cam;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayers;        //�n�ʔ�������邽�߂̃��C���[
    #endregion

    #region private�ϐ�
    private Quaternion cameraRot;       //�J�����̊p�x
    private Quaternion characterRot;    //�v���C���[�̊p�x
    private float playerX;              //�v���C���[��X���W
    private float playerZ;              //�v���C���[��Z���W
    private float camMinX = -30f;       //�J������������p�x�̍ŏ��l
    private float camMaxX = 30f;        //�J������������p�x�̍ő�l
    private float Xsensityvity = 3f;    //�J�����̊��x
    private float Ysensityvity = 3f;    //�J�����̊��x
    private float moveSpeed = 5f;
    private float jumpForce = 6f;
    private float attackDuration = 0.8f;//�U���A�j���[�V�����̒���
    private float attackTimer;          //�U�����Ԃ̌v�Z�p
    private float chargeDuration = 0.6f;//�`���[�W�A�j���[�V�����̒���
    private float chargeTimer;           //�`���[�W���Ԃ̌v�Z�p
    private float damageDuration = 0.6f;//�_���[�W�A�j���[�V�����̒���
    private float damageTimer;          //�_���[�W���Ԃ̌v�Z�p
    private bool isGround;              //�ڒn���Ă��邩�ǂ���
    private bool isAttack = false;      //�U�������ǂ���
    private bool isCharging = false;    //�`���[�W�����ǂ���
    private bool isDamage = false;      //�_���[�W���󂯂Ă��邩�ǂ���
    private bool isRun = false;         //�ړ������ǂ���
    #endregion

    private void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;
    }

    private void Update()
    {
        RotateCamera();
        Jump();
        PlayAnim();

        //�f�o�b�O�p
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// �v���C���[�̓����𐧌�
    /// </summary>
    private void MovePlayer()
    {
        playerX = 0;
        playerZ = 0;

        playerX = Input.GetAxisRaw("Horizontal");
        playerZ = Input.GetAxisRaw("Vertical");

        //�J�����̌����ɉ������ړ��x�N�g�����v�Z
        Vector3 moveDirection = cam.transform.forward * playerZ + cam.transform.right * playerX;

        //Y���̈ړ��͖�������(�n�ʂɉ����Ĉړ�����)
        moveDirection.y = 0f;   

        if(moveDirection.magnitude > 0)
        {
            isRun = true;
            transform.forward = moveDirection.normalized;
        }
        else
        {
            isRun = false;
        }

        rb.MovePosition(transform.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// �J�����̌����̐���
    /// </summary>
    private void RotateCamera()
    {
        float xRot = Input.GetAxis("Mouse X") * Xsensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Ysensityvity;

        //�J�����̉�]
        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        cameraRot = ClampRotation(cameraRot);
        cam.transform.localRotation = cameraRot;

        //�v���C���[�̉�]
        characterRot *= Quaternion.Euler(0, xRot, 0);
        transform.localRotation = characterRot;
    }

    /// <summary>
    /// �W�����v����Ƃ��ɌĂԊ֐�
    /// </summary>
    private void Jump()
    {
        if(isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// �J�����̊p�x����
    /// </summary>
    /// <param name="q">�J�����̊p�x</param>
    /// <returns>������̃J�����̊p�x</returns>
    private Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX,camMinX, camMaxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    /// <summary>
    /// �A�j���[�V�����̍Đ�
    /// </summary>
    private void PlayAnim()
    {
        if(Input.GetMouseButtonDown(0) && !isAttack && !isCharging)
        {
            Attack();
        }

        if(Input.GetMouseButton(1) && !isAttack && !isCharging)
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
        if(!isDamage)
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
        if((groundLayers & (1 << collision.gameObject.layer)) != 0)
        {
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if((groundLayers & (1 << collision.gameObject.layer)) != 0)
        {
            isGround = false;
        }
    }
    #endregion
}
