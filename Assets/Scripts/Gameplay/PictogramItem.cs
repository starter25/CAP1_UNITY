using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PictogramItem : MonoBehaviour
{
    [Header("Refs")]
    public Image image;  // 이 오브젝트에 붙은 Image 컴포넌트

    RectTransform rectTransform;

    Vector2 startPos;
    Vector2 stopPos;
    float moveDuration;
    float holdDuration;
    float fadeDuration;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 스폰 시 외부에서 호출해서 스프라이트와 이동/페이드 설정.
    /// </summary>
    public void Init(
        Sprite sprite,
        Vector2 startPos,
        Vector2 stopPos,
        float moveDuration,
        float holdDuration,
        float fadeDuration
    )
    {
        if (image != null)
        {
            image.sprite = sprite;
            var c = image.color;
            c.a = 1f;     // 완전 불투명으로 시작
            image.color = c;
        }

        this.startPos = startPos;
        this.stopPos = stopPos;
        this.moveDuration = moveDuration;
        this.holdDuration = holdDuration;
        this.fadeDuration = fadeDuration;

        rectTransform.anchoredPosition = startPos;

        StartCoroutine(RunRoutine());
    }

    IEnumerator RunRoutine()
    {
        // 1) 이동
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float lerpT = Mathf.Clamp01(t / moveDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, stopPos, lerpT);
            yield return null;
        }

        // 2) 멈춰 있는 구간
        rectTransform.anchoredPosition = stopPos;
        yield return new WaitForSeconds(holdDuration);

        // 3) 잔상처럼 서서히 사라지는 구간
        if (image != null && fadeDuration > 0f)
        {
            float ft = 0f;
            Color startColor = image.color;

            while (ft < fadeDuration)
            {
                ft += Time.deltaTime;
                float lerpT = Mathf.Clamp01(ft / fadeDuration);
                float alpha = Mathf.Lerp(1f, 0f, lerpT);
                Color c = startColor;
                c.a = alpha;
                image.color = c;
                yield return null;
            }
        }

        // 4) 완전히 사라지면 삭제
        Destroy(gameObject);
    }
}
