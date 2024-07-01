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
    [SerializeField] private LayerMask groundLayers;        //地面判定をするためのレイヤー
    #endregion

    #region private変数
    private Quaternion cameraRot;       //カメラの角度
    private Quaternion characterRot;    //プレイヤーの角度
    private float playerX;              //プレイヤーのX座標
    private float playerZ;              //プレイヤーのZ座標
    private float camMinX = -30f;       //カメラが向ける角度の最小値
    private float camMaxX = 30f;        //カメラが向ける角度の最大値
    private float Xsensityvity = 3f;    //カメラの感度
    private float Ysensityvity = 3f;    //カメラの感度
    private float moveSpeed = 5f;
    private float jumpForce = 6f;
    private float attackDuration = 0.8f;//攻撃アニメーションの長さ
    private float attackTimer;          //攻撃時間の計算用
    private float chargeDuration = 0.6f;//チャージアニメーションの長さ
    private float chargeTimer;           //チャージ時間の計算用
    private float damageDuration = 0.6f;//ダメージアニメーションの長さ
    private float damageTimer;          //ダメージ時間の計算用
    private bool isGround;              //接地しているかどうか
    private bool isAttack = false;      //攻撃中かどうか
    private bool isCharging = false;    //チャージ中かどうか
    private bool isDamage = false;      //ダメージを受けているかどうか
    private bool isRun = false;         //移動中かどうか
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

        //デバッグ用
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
    /// プレイヤーの動きを制限
    /// </summary>
    private void MovePlayer()
    {
        playerX = 0;
        playerZ = 0;

        playerX = Input.GetAxisRaw("Horizontal");
        playerZ = Input.GetAxisRaw("Vertical");

        //カメラの向きに応じた移動ベクトルを計算
        Vector3 moveDirection = cam.transform.forward * playerZ + cam.transform.right * playerX;

        //Y軸の移動は無視する(地面に沿って移動する)
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
    /// カメラの向きの制御
    /// </summary>
    private void RotateCamera()
    {
        float xRot = Input.GetAxis("Mouse X") * Xsensityvity;
        float yRot = Input.GetAxis("Mouse Y") * Ysensityvity;

        //カメラの回転
        cameraRot *= Quaternion.Euler(-yRot, 0, 0);
        cameraRot = ClampRotation(cameraRot);
        cam.transform.localRotation = cameraRot;

        //プレイヤーの回転
        characterRot *= Quaternion.Euler(0, xRot, 0);
        transform.localRotation = characterRot;
    }

    /// <summary>
    /// ジャンプするときに呼ぶ関数
    /// </summary>
    private void Jump()
    {
        if(isGround && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// カメラの角度制限
    /// </summary>
    /// <param name="q">カメラの角度</param>
    /// <returns>制限後のカメラの角度</returns>
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
    /// アニメーションの再生
    /// </summary>
    private void PlayAnim()
    {
        //攻撃アニメーションの管理
        if (Input.GetMouseButton(0) && !isAttack && !isCharging)
        {
            Attack();
            isAttack = true;
            animator.SetTrigger("Attack");
            attackTimer = 0f;
        }
        if(isAttack)
        {
            attackTimer += Time.deltaTime;

            if(attackTimer >= attackDuration)
            {
                isAttack = false;
                animator.SetTrigger("Idle");
            }
        }


        //溜め攻撃アニメーションの管理
        if(Input.GetMouseButton(1) && !isAttack && !isCharging)
        {
            isCharging = true;
            animator.SetTrigger("Charge");
            chargeTimer = 0f;
        }
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            //溜め攻撃が完了したら攻撃を実行
            if(chargeTimer >= chargeDuration)
            {
                ExecuteChargeAttack();
                isCharging = false;
                animator.SetTrigger("Idle");
            }
        }


        //被ダメージアニメーションの管理
        if (isDamage)
        {
            damageTimer += Time.deltaTime;

            if(damageTimer >= damageDuration)
            {
                isDamage = false;
                animator.SetTrigger("Idle");
            }
        }

        animator.SetBool("isRun", isRun);
    }

    private void Attack()
    {
        Debug.Log("攻撃");
    }

    private void ExecuteChargeAttack()
    {
        Debug.Log("溜め攻撃");
    }

    private void TakeDamage()
    {
        if(!isDamage)
        {
            isDamage = true;
            animator.SetTrigger("Damage");
            damageTimer = 0f;

            Debug.Log("ダメージ");
        }
    }

    #region 接地判定
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
