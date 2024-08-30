using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMR;
    [SerializeField] private TitleLogoAnimation titleLogoAnimation;
    [SerializeField] private SettingUI settingUI;
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject setSkillUI;
    [SerializeField] private GameObject selectGhost;
    [SerializeField] private GameObject backArrowImage;
    [SerializeField] private GameObject RecommendStartImage;
    [SerializeField] private Image backArrowGauge;
    [SerializeField] private Material[] materials;
    
    public int pushSpaceCount = 0;

    private Vector3 fixedPosition;
    private Vector3 zoomPosition = new Vector3(0f, -5.9f, 8.2f);
    private Animator animator;
    private int animateTime = 6;
    private float timer;
    private float qKeyHoldTime;
    private float decayRate = 0.5f;
    private float holdDuration = 2f;
    private bool isAnimating = false;
    private bool isAttacking = false;
    private bool isSurprised = false;
    private int selectedIndex;

    private void Start()
    {
        pushSpaceCount = 0;
        selectedIndex = -1;
        animator = GetComponent<Animator>();
        timer = animateTime;
        skinnedMR.enabled = true;
        startUI.SetActive(true);
        setSkillUI.SetActive(false);
        selectGhost.SetActive(false);
        backArrowImage.SetActive(false);
        RecommendStartImage.SetActive(false);
        fixedPosition = new Vector3(-30f, -13.5f, 40.7f);
        this.transform.position = fixedPosition;
        AudioManager.Instance.PlayBGM("TitleBGM");
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && !settingUI.isOpen)
        {
            AudioManager.Instance.PlaySFX("DecideMenu");
            pushSpaceCount++;

            switch (pushSpaceCount)
            {
                case 1:
                    MoveTitle();
                    return;
                case 2:
                    setSkillUI.SetActive(true);
                    startUI.SetActive(false);
                    selectGhost.SetActive(true);
                    backArrowImage.SetActive(true);
                    skinnedMR.enabled = false;
                    backArrowGauge.fillAmount = 0;
                    return;

                case 3:
                    selectedIndex = GameManager.Instance.GetSelectedIndex();
                    RecommendStartImage.SetActive(true);
                    return;

                case 4:
                    setSkillUI.SetActive(false);
                    selectGhost.SetActive(false);
                    backArrowImage.SetActive(false);
                    skinnedMR.enabled = true;
                    this.transform.position = new Vector3(0f, -5.9f, 8.2f);
                    this.transform.Rotate(0, 60, 0);
                    AudioManager.Instance.StopBGM();

                    if(selectedIndex >= 0 && selectedIndex < materials.Length)
                    {
                        skinnedMR.material = materials[selectedIndex];
                    }

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

        if (Input.GetKey(KeyCode.Q) && pushSpaceCount == 2 || Input.GetKey(KeyCode.Q) && pushSpaceCount == 3)
        {
            qKeyHoldTime += Time.deltaTime;
            qKeyHoldTime = Mathf.Clamp(qKeyHoldTime, 0f, holdDuration);

            if(qKeyHoldTime >= holdDuration)
            {
                AudioManager.Instance.PlaySFX("CancelMenu");
                pushSpaceCount = 1;
                MoveTitle();
            }
        }
        else
        {
            if(pushSpaceCount != 2)
            {
                qKeyHoldTime = 0;
            }
            qKeyHoldTime -= Time.deltaTime * decayRate;
            qKeyHoldTime = Mathf.Clamp(qKeyHoldTime, 0f, holdDuration);
        }

        if(Input.GetKeyDown(KeyCode.Q) && pushSpaceCount == 3)
        {
            AudioManager.Instance.PlaySFX("CancelMenu");
            pushSpaceCount = 2;
            RecommendStartImage.SetActive(false);
        }

        backArrowGauge.fillAmount = qKeyHoldTime / holdDuration;

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
        if(!settingUI.isOpen)
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
        StartCoroutine(ChangeSceneAfterDelay(1f));
    }

    public void MenuAnimation()
    {
        StartCoroutine(MoveUpAndBack());
    }

    private void MoveTitle()
    {
        setSkillUI.SetActive(false);
        startUI.SetActive(true);
        selectGhost.SetActive(false);
        backArrowImage.SetActive(false);
        skinnedMR.enabled = true;
        titleLogoAnimation.isAnimating = true;
    }
}
