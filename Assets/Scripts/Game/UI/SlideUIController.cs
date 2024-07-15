using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUIController : MonoBehaviour
{
    public int state = 0;
    public bool loop = false;

    [Header("Text")]
    [SerializeField] private Vector3 outPos01 = new Vector3(-1920, 0, 0);
    [SerializeField] private Vector3 outPos02 = new Vector3(1920, 0, 0);
    [SerializeField] private Vector3 inPos = new Vector3(0, 0, 0);

    private void Update()
    {
        if (state == 0)
        {
            //初期位置
            if(transform.localPosition != outPos01)
            {
                transform.localPosition = outPos01;
            }
        }
        else if(state == 1)
        {
            //スライドイン
            if (transform.localPosition.x > inPos.x - 1.0f
                && transform.localPosition.y > inPos.y- 1.0f
                && transform.localPosition.z > inPos.z - 1.0f)
            {
                inPos = transform.localPosition;
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, inPos, 4.0f * Time.unscaledDeltaTime);
            }
        }
        else if(state == 2)
        {
            //スライドアウト
            if(transform.localPosition != outPos02)
            {
                if(transform.localPosition.x > outPos02.x - 1.0f
                    && transform.localPosition.y > outPos02.y - 1.0f
                    && transform.localPosition.z > outPos02.z - 1.0f)
                {
                    transform.localPosition = outPos02;
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, outPos02, 2.0f * Time.unscaledDeltaTime);
                }
            }
            else
            {
                if (loop)
                {
                    state = 0;
                }
            }
        }
    }
}
