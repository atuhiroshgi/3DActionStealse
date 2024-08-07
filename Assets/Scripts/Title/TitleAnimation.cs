using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMR;
    [SerializeField] private TitleLogoAnimation titleLogoAnimation;
    [SerializeField] private SettingUI settingUI;

    private Vector3 fixedPosition;
    private Animator animator;
    private int animateTime = 6;
    private int pushSpaceCount = 0;
    private float timer;
    private bool isAnimating = false;
    private bool isAttacking = false;
    private bool isSurprised = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        timer = animateTime;
        skinnedMR.enabled = true;
        fixedPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pushSpaceCount++;

            if (pushSpaceCount == 1)
            {
                titleLogoAnimation.isAnimating = true;
                return;
            }

            isAnimating = true;
            animateTime = -1;
            animator.SetBool("Dissolved", true);
        }

        if ((Input.GetKeyDown(KeyCode.M) && !settingUI.isOpen) || (Input.GetKeyDown(KeyCode.Return) && settingUI.isOpen) || (Input.GetKeyDown(KeyCode.Backspace) && settingUI.isOpen))
        {
            MenuAnimation();
        }

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            animateTime--;
            timer = 0f;
        }

        if (animateTime % 6 == 0 && !isAnimating)
        {
            isAnimating = true;
            PlayRandomAnimation();
        }
        else if (animateTime == 0)
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
        if (randomIndex == 0)
        {
            isAttacking = true;
        }
        else if (randomIndex == 1)
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

    private IEnumerator MoveUpAndBack()
    {
        skinnedMR.enabled = true;

        if(settingUI.isOpen == true)
        {
            this.transform.position = new Vector3(this.transform.position.x, -40, this.transform.position.z);
        }

        float elapsedTime = 0f;
        if(settingUI.isOpen == false)
        {
            // 0.5秒かけてY=24まで移動
            Vector3 targetPosition = new Vector3(transform.position.x, 24, transform.position.z);
            while (elapsedTime < 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, (elapsedTime / 0.5f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            Vector3 targetPosition = new Vector3(transform.position.x, 24, transform.position.z);
            while (elapsedTime < 0.7f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, (elapsedTime / 0.7f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }
        // 0.5秒待機
        //yield return new WaitForSeconds(0.5f);

        // 0.5秒かけてFixedPositionに戻る
        elapsedTime = 0f;
        while (elapsedTime < 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, fixedPosition, (elapsedTime / 0.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = fixedPosition;

        if(settingUI.isOpen == true)
        {
            skinnedMR.enabled = false;
        }
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

    public void MenuAnimation()
    {
        StartCoroutine(MoveUpAndBack());
    }
}
