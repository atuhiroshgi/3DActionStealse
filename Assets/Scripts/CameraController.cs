using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private Vector3 currentPos;
    private Vector3 pastPos;
    private Vector3 diff;
    private float distanceToPlayer = 46.25f;

    private void Start()
    {
        pastPos = player.transform.position;
    }

    void Update()
    {
        currentPos = player.transform.position;
        diff = currentPos - pastPos;

        // カメラ位置の更新
        transform.position = Vector3.Lerp(transform.position, transform.position + diff, 1.0f);

        // マウスの移動量を取得
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        // X方向に一定量移動していれば横回転
        if (Mathf.Abs(mx) > 0.01f)
        {
            // 回転軸はワールド座標のY軸
            transform.RotateAround(player.transform.position, Vector3.up, mx);
        }

        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(my) > 0.01f)
        {
            // 回転軸はカメラ自身のX軸
            transform.RotateAround(player.transform.position, transform.right, -my);
        }

        // Playerからの距離を一定にする
        Vector3 cameraToPlayer = transform.position - player.transform.position;
        float desiredDistance = Mathf.Sqrt(distanceToPlayer);
        if (cameraToPlayer.magnitude > desiredDistance)
        {
            cameraToPlayer = cameraToPlayer.normalized * desiredDistance;
            transform.position = player.transform.position + cameraToPlayer;
        }

        pastPos = currentPos;
    }
}
