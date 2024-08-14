using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGhostAnimation : MonoBehaviour
{
    [SerializeField] private Animator ghostAnimator;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    private Vector3 targetPosition;
    private int selectedIndex = 0;
    private bool isMoving = false;

    private void OnEnable()
    {
        ghostAnimator.SetTrigger("Idle");
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && selectedIndex != 2)
        {
            selectedIndex++;
            targetPosition += new Vector3(0, 12, 0);
            isMoving = true;
            GameManager.Instance.SetGhostMoving(isMoving);
        }
        if (Input.GetKeyDown(KeyCode.W) && selectedIndex != 0)
        {
            selectedIndex--;
            targetPosition -= new Vector3(0, 12, 0);
            isMoving = true;
            GameManager.Instance.SetGhostMoving(isMoving);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    
        if(Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
        {
            OnReachedTargetPosition();
        }
    }

    private void OnReachedTargetPosition()
    {
        isMoving = false;
        GameManager.Instance.SetGhostMoving(isMoving);
        GameManager.Instance.SetSelectedIndex(selectedIndex);
    }
}
