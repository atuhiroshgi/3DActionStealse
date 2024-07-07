using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    private Animator animator;
    private int animateTime = 6;
    private float timer;
    private bool isAnimating = false;
    private bool isAttacking = false;
    private bool isSurprised = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        timer = animateTime;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 1f)
        {
            animateTime--;
            timer = 0f;
        }

        if (animateTime % 6 == 0 && !isAnimating)
        {
            isAnimating = true;
            PlayRandomAnimation();
        }
        else if(animateTime == 0)
        {
            animateTime = 6;
            isAnimating = false;
        }

        // AttackとSurprisedの状態に応じてアニメーションを設定
        animator.SetBool("Attack", isAttacking);
        animator.SetBool("Surprised", isSurprised);
    }

    private void PlayRandomAnimation()
    {
        int randomIndex = Random.Range(0, 2);
        Debug.Log(randomIndex);
        if(randomIndex == 0)
        {
            isAttacking = true;
        }
        else if(randomIndex == 1)
        {
            isSurprised = true;
        }
    }

    public void CancelAnimation()
    {
        isAttacking = false;
        isSurprised = false;
    }
}
