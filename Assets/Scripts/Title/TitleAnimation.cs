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
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject setSkillUI;
    [SerializeField] private GameObject selectGhost;
    
    public int pushSpaceCount = 0;

    private Vector3 fixedPosition;
    private Vector3 zoomPosition = new Vector3(0f, -5.9f, 8.2f);
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
        startUI.SetActive(true);
        setSkillUI.SetActive(false);
        selectGhost.SetActive(false);
        fixedPosition = new Vector3(-30f, -13.5f, 40.7f);
        this.transform.position = fixedPosition;
        AudioManager.Instance.PlayBGM("TitleBGM");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pushSpaceCount++;

            switch (pushSpaceCount)
            {
                case 1:
                    titleLogoAnimation.isAnimating = true;
                    return;
                case 2:
                    setSkillUI.SetActive(true);
                    startUI.SetActive(false);
                    selectGhost.SetActive(true);
                    skinnedMR.enabled = false;
                    return;
                case 3:
                    setSkillUI.SetActive(false);
                    selectGhost.SetActive(false);
                    skinnedMR.enabled = true;
                    this.transform.position = new Vector3(0f, -5.9f, 8.2f);
                    this.transform.Rotate(0, 60, 0);
                    AudioManager.Instance.StopBGM();
                    break;
            }
            AudioManager.Instance.PlaySFX("StartGame");
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

        // Attack��Surprised�̏�Ԃɉ����ăA�j���[�V������ݒ�
        animator.SetBool("Attack", isAttacking);
        animator.SetBool("Surprised", isSurprised);
    }

    /// <summary>
    /// �����_���ɃA�j���[�V�������Đ����鏈��
    /// </summary>
    private void PlayRandomAnimation()
    {
        int randomIndex = Random.Range(0, 2);
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
    /// �A�j���[�V������Idle�ɖ߂�����
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
        if(!settingUI.isOpen)
        {
            // 0.5�b������Y=24�܂ňړ�
            Vector3 targetPosition = new Vector3(transform.position.x, 24, transform.position.z);
            while (elapsedTime < 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, (elapsedTime / 0.5f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }
        else if(settingUI.isOpen)
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
        // 0.5�b�ҋ@
        //yield return new WaitForSeconds(0.5f);

        // 0.5�b������FixedPosition�ɖ߂�
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
        StartCoroutine(ChangeSceneAfterDelay(1f));
    }

    public void MenuAnimation()
    {
        StartCoroutine(MoveUpAndBack());
    }
}
