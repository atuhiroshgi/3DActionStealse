using System.Collections;
using UnityEngine;

public class TitleLogoAnimation : MonoBehaviour
{
    [SerializeField] private TitleAnimation titleAnimation;
    [SerializeField] private float startYPosition = 10f;
    [SerializeField] private float endYPosition = 0f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fallTime = 6f;
    
    public bool isAnimating = false;

    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        // RectTransformコンポーネントを取得
        rectTransform = GetComponent<RectTransform>();

        // 初期位置を設定
        startPosition = new Vector3(rectTransform.localPosition.x, startYPosition, rectTransform.localPosition.z);
        // 定位置を設定
        endPosition = new Vector3(rectTransform.localPosition.x, endYPosition, rectTransform.localPosition.z);
        // ロゴの位置を初期位置に設定
        rectTransform.localPosition = startPosition;

        StartCoroutine(FallTiming());
    }

    private void Update()
    {
        if (isAnimating)
        {
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, endPosition, speed * Time.deltaTime);

            if (Vector3.Distance(rectTransform.localPosition, endPosition) < 0.01f)
            {
                rectTransform.localPosition = endPosition;
                titleAnimation.pushSpaceCount = 1;
                isAnimating = false;
            }
        }
    }

    IEnumerator FallTiming()
    {
        yield return new WaitForSeconds(fallTime);
        isAnimating = true;
    }
}
