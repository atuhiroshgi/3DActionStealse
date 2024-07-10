using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [Header("3Dモデル")]
    [SerializeField] private GameObject SkinObject;
    [Header("アニメーター")]
    [SerializeField] private Animator animator;
    [Header("通常時のマテリアル")]
    [SerializeField] private Material normalMaterial;
    [Header("半透明のマテリアル")]
    [SerializeField] private Material hiddenMaterial;
    [Header("プレイヤーの初期スポーン")]
    [SerializeField] private Transform startPos;
    [Header("スキルゲージの管理")]
    [SerializeField] private SkillGuageController skillGuageController;
    [Header("地面判定をつけるレイヤー")]
    [SerializeField] private LayerMask groundLayers;
    [Header("おばけが進行方向に向く速度")]
    [SerializeField] private float smoothTime = 0.1f;
    [Header("ジャンプの高さ")]
    [SerializeField] private float jumpForce;
    [Header("透明化を維持できる時間")]
    [SerializeField] private float hideDuration = 3f;
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
    private float defaultMoveSpeed = 5f;//通常時の移動速度
    private float hiddenMoveSpeed = 15f;//隠れ時の移動速度
    private float defaultJumpForce = 7f;//通常時のジャンプする力
    private float hiddenJumpForce = 12f;//隠れ時のジャンプする力
    private float attackDuration = 0.8f;//攻撃アニメーションの長さ
    private float chargeDuration = 1.6f;//チャージアニメーションの長さ
    private float damageDuration = 0.6f;//ダメージアニメーションの長さ
    private float currentHideTime = 0f;
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
        Raycast();

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
            if(isHidden)
            {
                //透明化を任意のタイミングで解除
                EndHidden();
            }
            else
            {
                if (skillGuageController.DecreasedStackCount((2)))
                {
                    //スタックが2あるなら減らして透明化を開始する
                    StartCoroutine(StartHideTimer());
                }
                else
                {
                    Debug.Log("スタックが足りません");
                }
            }
        }

        if(isHidden && currentHideTime > hideDuration)
        {
            //制限時間が終わったら透明化を解除
            EndHidden();
        }
    }

    /// <summary>
    /// 透明化のタイマーを開始するコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartHideTimer()
    {
        isHidden = true;
        //半透明のマテリアルに変更
        skinnedMR.material = hiddenMaterial;
        moveSpeedIn = hiddenMoveSpeed;
        jumpForce = 12f;

        currentHideTime = 0;

        while(currentHideTime < hideDuration)
        {
            //一定時間ループしてディレイをかける
            yield return null;
            currentHideTime += Time.deltaTime;
        }

        EndHidden();
    }

    /// <summary>
    /// 透明化を終了する処理
    /// </summary>
    private void EndHidden()
    {
        isHidden = false;
        skinnedMR.material = normalMaterial;
        moveSpeedIn = defaultMoveSpeed;
        jumpForce = 7f;
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
    /// Rayを飛ばすときの処理
    /// </summary>
    private void Raycast()
    {
        // カメラから20ユニット奥にプレイヤーが配置されていると仮定
        Vector3 playerPosition = Camera.main.transform.position + Camera.main.transform.forward * 20f;
        Ray ray = new Ray(playerPosition, Camera.main.transform.forward);

        // Rayがヒットしたすべてのオブジェクトを取得
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        if (hits.Length > 0)
        {
            // ヒットしたオブジェクトを距離順にソート
            System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

            // 最も近いオブジェクトを取得
            GameObject closestHitObject = hits[0].collider.gameObject;

            // 最も近いオブジェクトの名前をコンソールに表示
            Debug.Log("Closest hit object: " + closestHitObject.name);
        }
        else
        {
            Debug.Log("No objects hit by the ray.");
        }
    }

    /// <summary>
    /// 攻撃の処理
    /// </summary>
    private void Attack()
    {
        if (!isHidden)
        {
            if (skillGuageController.DecreasedStackCount(1))
            {
                isAttack = true;
                Debug.Log("攻撃");
                animator.SetTrigger("Attack");
                StartCoroutine(ResetBoolAfterDelay("isAttack", attackDuration));
            }
        }
    }

    /// <summary>
    /// 溜め攻撃の処理
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

                Debug.Log("溜め攻撃");
            }
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
    private void ForDebug() 
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeDamage();
        }
    }
}
