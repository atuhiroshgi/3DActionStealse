using System.Collections;
using UnityEngine;

public class TitleLogoAnimation : MonoBehaviour
{
    [SerializeField] private float startYPosition = 10f;
    [SerializeField] private float endYPosition = 0f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fallTime = 6f;

    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private bool isAnimating = false;

    private void Start()
    {
        // RectTransform�R���|�[�l���g���擾
        rectTransform = GetComponent<RectTransform>();

        // �����ʒu��ݒ�
        startPosition = new Vector3(rectTransform.localPosition.x, startYPosition, rectTransform.localPosition.z);
        // ��ʒu��ݒ�
        endPosition = new Vector3(rectTransform.localPosition.x, endYPosition, rectTransform.localPosition.z);
        // ���S�̈ʒu�������ʒu�ɐݒ�
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
