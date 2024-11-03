using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChecker : MonoBehaviour
{
    public static bool tatchWall = false;

    private void Update()
    {
        if (tatchWall)
        {
            Debug.Log("�������Ă��");
        }
        else
        {
            Debug.Log("�������Ăւ��");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Ground")
        {
            tatchWall = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "Ground")
        {
            tatchWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag != "Ground")
        {
            tatchWall = false;
        }
    }
}
