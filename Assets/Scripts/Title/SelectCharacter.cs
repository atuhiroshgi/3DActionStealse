using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    [SerializeField] private GameObject[] characterIcons;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float moveSpeed = 5f;

    private int selectedIndex = 0;

    private void Start()
    {
        UpdateSelection();
    }

    private void Update()
    {
        MoveArrow();
        UpdateSelection();
    }

    private void MoveArrow()
    {
        //WASD�L�[�̓��͂��擾����Arrow���ړ�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        this.transform.position += movement * moveSpeed * Time.deltaTime;

        //�ł��߂��A�C�R����T��
        float closestDistance = float.MaxValue;
        for (int i = 0; i < characterIcons.Length; i++)
        {
            float distance = Vector3.Distance(this.transform.position, characterIcons[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                selectedIndex = i;
            }
        }
    }

    private void UpdateSelection()
    {
        //�S�ẴA�C�R���̔w�i���f�t�H���g�F�Ƀ��Z�b�g
        foreach(GameObject icon in characterIcons)
        {
            icon.GetComponent<Image>().color = defaultColor;
        }

        //�I�𒆂̃A�C�R���̔w�i�F���n�C���C�g�F�ɕύX
        characterIcons[selectedIndex].GetComponent<Image>().color = highlightColor;
    }

}
