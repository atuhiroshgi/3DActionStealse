using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private GameObject cam;
    [SerializeField] private Transform groundCheckPoint;    //接地判定のための座標
    [SerializeField] private Rigidbody rb;
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
    private float jumpForce = 10f;
    private bool isGround;
    #endregion

    private void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RotateCamera();
        Jump();
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

        Vector3 moveDirection = cam.transform.forward * playerZ + cam.transform.right * playerX;
        moveDirection.y = 0f;   //Y軸の移動は無視する(地面に沿って移動する)

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
        //characterRot *= Quaternion.Euler(0, xRot, 0);
        //transform.localRotation = characterRot;
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
    /// カメラ角度制限
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
