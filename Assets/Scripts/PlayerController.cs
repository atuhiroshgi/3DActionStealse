using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [Header("3Dモデル")]
    [SerializeField] private GameObject SkinObject;         //透明化のときにオブジェクトにアクセスするため
    [Header("アニメーター")]
    [SerializeField] private Animator animator;         //アニメーション制御に必要
    [Header("通常時のマテリアル")]
    [SerializeField] private Material normalMaterial;       //通常時のマテリアル
    [Header("半透明のマテリアル")]
    [SerializeField] private Material hiddenMaterial;       //透明化時のマテリアル
    [Header("プレイヤーの初期スポーン")]
    [SerializeField] private Transform startPos;            //プレイヤーがスポーンする座標
    [Header("地面判定をつけるレイヤー")]
    [SerializeField] private LayerMask groundLayers;        //地面判定をするためのレイヤー
    [Header("おばけが進行方向に向く速度")]
    [SerializeField] private float smoothTime = 0.1f;       //進行方向にかかる時間
    [Header("ジャンプの高さ")]
    [SerializeField] private float jumpForce;               //ジャンプするとき上向きにかける力
    #endregion

    #region private変数
    private Rigidbody rb;
    private SkinnedMeshRenderer skinnedMR;  //SkinnedMeshRendererを参照するため
    private Vector3 moveSpeed;          //プレイヤーの移動速度
    private Vector3 currentPos;         //プレイヤーの現在の位置
    private Vector3 pastPos;            //プレイヤーの過去の位置
    private Vector3 delta;              //プレイヤーの移動量
    private Quaternion playerRot;       //プレイヤーの角度
    private Quaternion nextRot;         //どれだけ回転するか
    private float currentAngVelo;       //現在の回転角速度
    private float diffAngle;            //現在の向きと進行方向の角度
    private float rotAngle;             //現在の回転する角度
    private float maxAngVelo = float.PositiveInfinity;     //最大の回転角
    private float moveSpeedIn;          //スピード管理用
    private float defaultMoveSpeed = 5f;//通常時
    private float hiddenMoveSpeed = 15f;//隠れ時
    private float attackDuration = 0.8f;//攻撃アニメーションの長さ
    private float chargeDuration = 1.6f;//チャージアニメーションの長さ
    private float damageDuration = 0.6f;//ダメージアニメーションの長さ
    private bool isGround;              //接地しているかどうか
    private bool isAttack = false;      //攻撃中かどうか
    private bool isCharging = false;    //チャージ中かどうか
    private bool isDamage = false;      //ダメージを受けているかどうか
    private bool isRun = false;         //移動中かどうか
    private bool isHidden = false;      //透明化中かどうか
    private bool jumpRequested = false; //ジャンプが要求されたかどうか
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

        //ジャンプ入力の取得
        if(Input.GetKeyDown(KeyCode.Space)&& isGround)
        {
            jumpRequested = true;
        }

        //デバッグ用
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
        //プレイヤーの速度の初期化
        moveSpeedIn = defaultMoveSpeed;

        //Playerが湧く位置の初期化
        if (startPos != null)
        {
            transform.position = startPos.position;
            transform.rotation = startPos.rotation;
        }
        else
        {
            Debug.LogError("StartPosが設定されていません。");
        }

        //マテリアルの初期化
        skinnedMR.material = normalMaterial;
    }

    /// <summary>
    /// プレイヤーの動きを制限
    /// </summary>
    private void MovePlayer()
    {
        //カメラに対して前を取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        //カメラに対して右を取得
        Vector3 cameraRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        moveSpeed = Vector3.zero;

        //移動入力
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

        //移動方向に速度をかけて移動
        rb.MovePosition(transform.position + moveDirection.normalized * moveSpeedIn * Time.deltaTime);

        isRun = moveDirection != Vector3.zero;
    }

    /// <summary>
    /// プレイヤーの向きの制御
    /// </summary>
    private void RotatePlayer()
    {
        //現在の位置
        currentPos = transform.position;

        //移動量の計算
        delta = currentPos - pastPos;
        delta.y = 0;

        //過去の位置の更新
        pastPos = currentPos;

        if (delta == Vector3.zero)
        {
            return;
        }

        playerRot = Quaternion.LookRotation(delta, Vector3.up);

        diffAngle = Vector3.Angle(transform.forward, delta);

        //Vector3.SmoothDampで値を徐々に変化
        rotAngle = Mathf.SmoothDampAngle(0, diffAngle, ref currentAngVelo, smoothTime, maxAngVelo);

        nextRot = Quaternion.RotateTowards(transform.rotation, playerRot, rotAngle);

        transform.rotation = nextRot;
    }

    /// <summary>
    /// 隠れるモードに関する処理
    /// </summary>
    private void Hidden()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isHidden = !isHidden;
            //skinnedMR.enabled = !isHidden;  //isHiddenに基づいて透明化のオンオフを決定
            skinnedMR.material = isHidden ? hiddenMaterial : normalMaterial;
            
            moveSpeedIn = isHidden ? hiddenMoveSpeed : defaultMoveSpeed;
            jumpForce = isHidden ? 12f : 7f;
        }
    }

    /// <summary>
    /// ジャンプに関する処理
    /// </summary>
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isGround = false;
            Debug.Log("ジャンプ");
        }
    }

    /// <summary>
    /// アニメーションの再生
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
    /// 攻撃の処理
    /// </summary>
    private void Attack()
    {
        if (!isHidden)
        {
            isAttack = true;
            Debug.Log("攻撃");
            animator.SetTrigger("Attack");
            StartCoroutine(ResetBoolAfterDelay("isAttack", attackDuration));
        }
    }

    /// <summary>
    /// 溜め攻撃の処理
    /// </summary>
    private void ChargeAttack()
    {
        if (!isHidden)
        {
            isCharging = true;
            animator.SetTrigger("Charge");
            StartCoroutine(ResetBoolAfterDelay("isCharging", chargeDuration));

            Debug.Log("溜め攻撃");
        }
    }

    /// <summary>
    /// ダメージを受けたときの処理
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
                //ダメージを受けたら透明化解除
                isHidden = false;
                skinnedMR.enabled = true;
                moveSpeedIn = 50f;
                jumpForce = 7f; ;
            }

            Debug.Log("ダメージ");
        }
    }

    /// <summary>
    /// アニメーションの時間待つための関数
    /// </summary>
    /// <param name="boolName">アニメーションを管理するbool変数</param>
    /// <param name="delay">経過させる時間</param>
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

    #region 接地判定
    private void OnCollisionEnter(Collision collision)
    {
        if ((groundLayers & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log("接地");
            isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundLayers & (1 << collision.gameObject.layer)) != 0)
        {
            //Debug.Log("離陸");
            //isGround = false;
        }
    }
    #endregion

    /// <summary>
    /// デバッグ用
    /// </summary>
    private void ForDebug() {

        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage();
        }

        Debug.Log(moveSpeedIn);
    }
}
