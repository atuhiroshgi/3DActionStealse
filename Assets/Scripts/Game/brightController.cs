using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brightController : MonoBehaviour
{
    [SerializeField] private Light directionalLight;

    private void Start()
    {
        directionalLight.intensity = GameManager.Instance.GetBright();
    }
}