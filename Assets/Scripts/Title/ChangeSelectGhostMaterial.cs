using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSelectGhostMaterial : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer SMRenderer;
    [SerializeField] private Material[] materials;

    private void Update()
    {
        int selectedIndex = GameManager.Instance.GetSelectedIndex();

        if(selectedIndex >= 0 && selectedIndex < materials.Length)
        {
            SMRenderer.material = materials[selectedIndex];
        }
    }
}
