using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ComboSlideUI : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform rect;   // ComboEffectPanel
    public Image comboImage;     // ComboEffectImage (또는 Panel에 있는 Image)

    [Header("Slide Positions")]
    public Vector2 hiddenPos;    // 화면 오른쪽 밖
    public Vector2 shownPos;     // 화면 안쪽

    [Header("Timings")]
    public float slideDuration = 0.25f;
    public float stayDuration = 1.0f;

    Coroutine routine;

    void Awake()
    {
        // 시작은 숨겨진 위치
        rect.anchoredPosition = hiddenPos;
    }

    public void Play(Sprite sprite)
    {
        comboImage.sprite = sprite;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(SlideRoutine());
    }

    IEnumerator SlideRoutine()
    {
        // 1) 오른쪽 밖 → 안쪽
        yield return StartCoroutine(Slide(rect, hiddenPos, shownPos, slideDuration));

        // 2) 잠깐 유지
        yield return new WaitForSeconds(stayDuration);

        // 3) 다시 안쪽 → 오른쪽 밖
        yield return StartCoroutine(Slide(rect, shownPos, hiddenPos, slideDuration));
    }

    IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            target.anchoredPosition = Vector2.Lerp(from, to, lerp);
            yield return null;
        }
        target.anchoredPosition = to;
    }
}
