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
        //WASDキーの入力を取得してArrowを移動
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        this.transform.position += movement * moveSpeed * Time.deltaTime;

        //最も近いアイコンを探す
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
        //全てのアイコンの背景をデフォルト色にリセット
        foreach(GameObject icon in characterIcons)
        {
            icon.GetComponent<Image>().color = defaultColor;
        }

        //選択中のアイコンの背景色をハイライト色に変更
        characterIcons[selectedIndex].GetComponent<Image>().color = highlightColor;
    }

}
