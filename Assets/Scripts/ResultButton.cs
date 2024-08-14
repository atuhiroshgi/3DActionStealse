using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultButton : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Continue();
        }
    }

    public void Continue()
    {
        GameManager.Instance.ToTitleScene();
    }
}
