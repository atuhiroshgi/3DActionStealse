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
            Debug.Log("“–‚½‚Á‚Ä‚ñ‚Å");
        }
        else
        {
            Debug.Log("“–‚½‚Á‚Ä‚Ö‚ñ‚Å");
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
