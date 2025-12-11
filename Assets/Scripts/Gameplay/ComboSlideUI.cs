using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ComboSlideUI : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform rect;   // ComboEffectImage가 붙어있는 RectTransform
    public Image comboImage;     // 실제로 보이는 이미지

    [Header("Slide Positions")]
    public Vector2 hiddenPos;    // 숨김 위치 (화면 밖)
    public Vector2 shownPos;     // 보여줄 위치 (화면 안)

    [Header("Timings")]
    public float slideDuration = 0.25f;
    public float stayDuration = 1.0f;

    private Coroutine routine;

    void Awake()
    {
        // rect, comboImage가 인스펙터에서 안 넣어져 있어도 버티도록
        if (rect == null)
            rect = GetComponent<RectTransform>();

        if (rect != null)
            rect.anchoredPosition = hiddenPos;

        if (comboImage == null)
            comboImage = GetComponent<Image>();
    }

    /// <summary>
    /// 콤보 이미지 보여줄 때 호출
    /// </summary>
    public void Play(Sprite sprite)
    {
        if (comboImage != null && sprite != null)
            comboImage.sprite = sprite;

        // 이전 애니메이션이 돌고 있으면 중지
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        // 1) 숨김 → 보여주기
        yield return StartCoroutine(Slide(rect, hiddenPos, shownPos, slideDuration));

        // 2) 잠깐 유지
        yield return new WaitForSeconds(stayDuration);

        // 3) 다시 보여준 위치 → 숨김 위치
        yield return StartCoroutine(Slide(rect, shownPos, hiddenPos, slideDuration));

        routine = null;
    }

    private IEnumerator Slide(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        if (target == null)
            yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // Time.timeScale 영향을 안 받게
            float k = Mathf.Clamp01(t / duration);
            target.anchoredPosition = Vector2.Lerp(from, to, k);
            yield return null;
        }
        target.anchoredPosition = to;
    }
}
