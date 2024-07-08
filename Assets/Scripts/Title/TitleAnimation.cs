using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMR;
    
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
        skinnedMR.enabled = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isAnimating = true;
            animateTime = -1;
            animator.SetBool("Dissolved", true);
        }

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

    /// <summary>
    /// ランダムにアニメーションを再生する処理
    /// </summary>
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

    /// <summary>
    /// アニメーションをIdleに戻す処理
    /// </summary>
    public void CancelAnimation()
    {
        isAttacking = false;
        isSurprised = false;
    }
    private IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.ToGameScene();
    }

    public void Hidden()
    {
        skinnedMR.enabled = false;
        StartCoroutine(ChangeSceneAfterDelay(0f));
    }

}
