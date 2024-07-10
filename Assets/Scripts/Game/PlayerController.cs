using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [Header("3D���f��")]
    [SerializeField] private GameObject SkinObject;
    [Header("�A�j���[�^�[")]
    [SerializeField] private Animator animator;
    [Header("�ʏ펞�̃}�e���A��")]
    [SerializeField] private Material normalMaterial;
    [Header("�������̃}�e���A��")]
    [SerializeField] private Material hiddenMaterial;
    [Header("�v���C���[�̏����X�|�[��")]
    [SerializeField] private Transform startPos;
    [Header("�X�L���Q�[�W�̊Ǘ�")]
    [SerializeField] private SkillGuageController skillGuageController;
    [Header("�n�ʔ�������郌�C���[")]
    [SerializeField] private LayerMask groundLayers;
    [Header("���΂����i�s�����Ɍ������x")]
    [SerializeField] private float smoothTime = 0.1f;
    [Header("�W�����v�̍���")]
    [SerializeField] private float jumpForce;
    [Header("���������ێ��ł��鎞��")]
    [SerializeField] private float hideDuration = 3f;
    #endregion

    #region private�ϐ�
    private Rigidbody rb;
    private SkinnedMeshRenderer skinnedMR;  //SkinnedMeshRenderer���Q�Ƃ��邽��
    private Vector3 moveSpeed;          //�v���C���[�̈ړ����x
    private Vector3 currentPos;         //�v���C���[�̌��݂̈ʒu
    private Vector3 pastPos;            //�v���C���[�̉ߋ��̈ʒu
    private Vector3 delta;              //�v���C���[�̈ړ���
    private Quaternion playerRot;       //�v���C���[�̊p�x
    private Quaternion nextRot;         //�ǂꂾ����]���邩
    private float currentAngVelo;       //���݂̉�]�p���x
    private float diffAngle;            //���݂̌����Ɛi�s�����̊p�x
    private float rotAngle;             //���݂̉�]����p�x
    private float maxAngVelo = float.PositiveInfinity;     //�ő�̉�]�p
    private float moveSpeedIn;          //�X�s�[�h�Ǘ��p
    private float defaultMoveSpeed = 5f;//�ʏ펞�̈ړ����x
    private float hiddenMoveSpeed = 15f;//�B�ꎞ�̈ړ����x
    private float defaultJumpForce = 7f;//�ʏ펞�̃W�����v�����
    private float hiddenJumpForce = 12f;//�B�ꎞ�̃W�����v�����
    private float attackDuration = 0.8f;//�U���A�j���[�V�����̒���
    private float chargeDuration = 1.6f;//�`���[�W�A�j���[�V�����̒���
    private float damageDuration = 0.6f;//�_���[�W�A�j���[�V�����̒���
    private float currentHideTime = 0f;
    private bool isGround;              //�ڒn���Ă��邩�ǂ���
    private bool isAttack = false;      //�U�������ǂ���
    private bool isCharging = false;    //�`���[�W�����ǂ���
    private bool isDamage = false;      //�_���[�W���󂯂Ă��邩�ǂ���
    private bool isRun = false;         //�ړ������ǂ���
    private bool isHidden = false;      //�����������ǂ���
    private bool jumpRequested = false; //�W�����v���v�����ꂽ���ǂ���
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        skinnedMR = SkinObject.GetComponent<SkinnedMeshRenderer>();
        pastPos = transform.position;

        Init();
    }

    private void Update()
    {
        RotatePlayer();
        Jump();
        PlayAnim();
        Hidden();
        Raycast();

        //�W�����v���͂̎擾
        if(Input.GetKeyDown(KeyCode.Space)&& isGround)
        {
            jumpRequested = true;
        }

        //�f�o�b�O�p
        ForDebug();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if(jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }

    private void Init()
    {
        //�v���C���[�̑��x�̏�����
        moveSpeedIn = defaultMoveSpeed;

        //Player���N���ʒu�̏�����
        if (startPos != null)
        {
            transform.position = startPos.position;
            transform.rotation = startPos.rotation;
        }
        else
        {
            Debug.LogError("StartPos���ݒ肳��Ă��܂���B");
        }

        //�}�e���A���̏�����
        skinnedMR.material = normalMaterial;
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
    /// �B��郂�[�h�Ɋւ��鏈��
    /// </summary>
    private void Hidden()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(isHidden)
            {
                //��������C�ӂ̃^�C�~���O�ŉ���
                EndHidden();
            }
            else
            {
                if (skillGuageController.DecreasedStackCount((2)))
                {
                    //�X�^�b�N��2����Ȃ猸�炵�ē��������J�n����
                    StartCoroutine(StartHideTimer());
                }
                else
                {
                    Debug.Log("�X�^�b�N������܂���");
                }
            }
        }

        if(isHidden && currentHideTime > hideDuration)
        {
            //�������Ԃ��I������瓧����������
            EndHidden();
        }
    }

    /// <summary>
    /// �������̃^�C�}�[���J�n����R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartHideTimer()
    {
        isHidden = true;
        //�������̃}�e���A���ɕύX
        skinnedMR.material = hiddenMaterial;
        moveSpeedIn = hiddenMoveSpeed;
        jumpForce = 12f;

        currentHideTime = 0;

        while(currentHideTime < hideDuration)
        {
            //��莞�ԃ��[�v���ăf�B���C��������
            yield return null;
            currentHideTime += Time.deltaTime;
        }

        EndHidden();
    }

    /// <summary>
    /// ���������I�����鏈��
    /// </summary>
    private void EndHidden()
    {
        isHidden = false;
        skinnedMR.material = normalMaterial;
        moveSpeedIn = defaultMoveSpeed;
        jumpForce = 7f;
    }

    /// <summary>
    /// �W�����v�Ɋւ��鏈��
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
    /// Ray���΂��Ƃ��̏���
    /// </summary>
    private void Raycast()
    {
        // �J��������20���j�b�g���Ƀv���C���[���z�u����Ă���Ɖ���
        Vector3 playerPosition = Camera.main.transform.position + Camera.main.transform.forward * 20f;
        Ray ray = new Ray(playerPosition, Camera.main.transform.forward);

        // Ray���q�b�g�������ׂẴI�u�W�F�N�g���擾
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        if (hits.Length > 0)
        {
            // �q�b�g�����I�u�W�F�N�g���������Ƀ\�[�g
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            // �ł��߂��I�u�W�F�N�g���擾
            GameObject closestHitObject = hits[0].collider.gameObject;

            // �ł��߂��I�u�W�F�N�g�̖��O���R���\�[���ɕ\��
            Debug.Log("Closest hit object: " + closestHitObject.name);
        }
        else
        {
            Debug.Log("No objects hit by the ray.");
        }
    }

    /// <summary>
    /// �U���̏���
    /// </summary>
    private void Attack()
    {
        if (!isHidden)
        {
            if (skillGuageController.DecreasedStackCount(1))
            {
                isAttack = true;
                Debug.Log("�U��");
                animator.SetTrigger("Attack");
                StartCoroutine(ResetBoolAfterDelay("isAttack", attackDuration));
            }
        }
    }

    /// <summary>
    /// ���ߍU���̏���
    /// </summary>
    private void ChargeAttack()
    {
        if (!isHidden)
        {
            if (skillGuageController.DecreasedStackCount(1))
            {
                isCharging = true;
                animator.SetTrigger("Charge");
                StartCoroutine(ResetBoolAfterDelay("isCharging", chargeDuration));

                Debug.Log("���ߍU��");
            }
        }
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

            if (isHidden)
            {
                //�_���[�W���󂯂��瓧��������
                isHidden = false;
                skinnedMR.enabled = true;
                moveSpeedIn = 50f;
                jumpForce = 7f; ;
            }

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
        if ((groundLayers & (1 << collision.gameObject.layer)) != 0)
        {
            //Debug.Log("����");
            //isGround = false;
        }
    }
    #endregion

    /// <summary>
    /// �f�o�b�O�p
    /// </summary>
    private void ForDebug() 
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage();
        }
    }
}
