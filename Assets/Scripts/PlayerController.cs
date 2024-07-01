using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private GameObject cam;
    [SerializeField] private Transform groundCheckPoint;    //�ڒn����̂��߂̍��W
    [SerializeField] private Rigidbody rb;
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
    /// �v���C���[�̓����𐧌�
    /// </summary>
    private void MovePlayer()
    {
        playerX = 0;
        playerZ = 0;

        playerX = Input.GetAxisRaw("Horizontal");
        playerZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = cam.transform.forward * playerZ + cam.transform.right * playerX;
        moveDirection.y = 0f;   //Y���̈ړ��͖�������(�n�ʂɉ����Ĉړ�����)

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
        //characterRot *= Quaternion.Euler(0, xRot, 0);
        //transform.localRotation = characterRot;
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
    /// �J�����p�x����
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
