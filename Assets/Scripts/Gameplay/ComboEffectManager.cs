using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ComboEffectEntry
{
    [Tooltip("이 콤보에 도달했을 때 발동 (예: 5, 10, 20)")]
    public int comboThreshold = 5;

    [Tooltip("해당 콤보에서 보여줄 이미지")]
    public Sprite sprite;
}

public class ComboEffectManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("콤보 이펙트가 나올 패널 (마스크 영역)")]
    public GameObject comboPanel;      // ComboEffectPanel (지금은 항상 켜둘 것)

    [Header("Slide UI")]
    public ComboSlideUI slideUI;       // 슬라이드 연출 담당

    [Tooltip("패널 안에서 이미지를 표시할 UI Image (슬라이드에서도 같은 이미지 사용)")]
    public Image comboImage;           // ComboEffectImage

    [Header("Effect Settings (현재는 사용X, 나중에 확장용)")]
    [Tooltip("이미지가 유지되는 시간(초)")]
    public float showDuration = 1.0f;

    [Tooltip("사라질 때 페이드 아웃 시간(초)")]
    public float fadeOutDuration = 0.4f;

    [Tooltip("같은 콤보 Threshold에서는 한 번만 발동할지 여부")]
    public bool playOncePerThreshold = true;

    [Header("Combo Threshold List")]
    public ComboEffectEntry[] comboEffects;

    Coroutine currentRoutine;
    int lastShownThreshold = -1;

    void Start()
    {
        // 패널은 항상 켜둔다 (비활성화하면 코루틴이 안 돌아감)
        if (comboPanel != null)
            comboPanel.SetActive(true);

        // 슬라이드 UI가 있으면 시작 위치를 숨겨진 위치로 세팅
        if (slideUI != null && slideUI.rect != null)
        {
            slideUI.rect.anchoredPosition = slideUI.hiddenPos;
        }
    }

    /// <summary>
    /// PoseScoreSystem에서 콤보가 변경될 때마다 호출
    /// </summary>
    public void OnComboChanged(int comboCount)
    {
        if (comboCount <= 0)
        {
            // 콤보 끊겼을 때는 그냥 숨기기만
            HideImmediate();
            return;
        }

        // 현재 콤보에 해당하는 설정 찾기 (정확히 같은 값일 때)
        ComboEffectEntry entry = null;
        for (int i = 0; i < comboEffects.Length; i++)
        {
            if (comboCount == comboEffects[i].comboThreshold)
            {
                entry = comboEffects[i];
                break;
            }
        }

        if (entry == null || entry.sprite == null)
            return;

        if (playOncePerThreshold && entry.comboThreshold == lastShownThreshold)
            return; // 이미 이 콤보에서 한 번 보여줬다면 패스

        lastShownThreshold = entry.comboThreshold;
        Show(entry.sprite);
    }

    void Show(Sprite sprite)
    {
        if (slideUI == null)
        {
            Debug.LogWarning("Slide UI not assigned!");
            return;
        }

        comboPanel.SetActive(true);

        slideUI.Play(sprite);
    }

    // 기존 페이드용 코루틴은 이제 안 쓰이지만,
    // 혹시 나중에 쓸까봐 남겨두고 싶으면 그대로 두고, 호출만 안 하면 됨.
    // 지금은 어디에서도 ShowRoutine을 호출하지 않으니까 무시해도 괜찮다.
    /*
    IEnumerator ShowRoutine()
    {
        Color c = comboImage.color;
        c.a = 1f;
        comboImage.color = c;

        yield return new WaitForSecondsRealtime(showDuration);

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            c.a = a;
            comboImage.color = c;
            yield return null;
        }

        HideImmediate();
        currentRoutine = null;
    }
    */

    public void HideImmediate()
    {
        // 슬라이드 UI가 있다면 바로 숨김 위치로 보내기
        if (slideUI != null && slideUI.rect != null)
        {
            slideUI.rect.anchoredPosition = slideUI.hiddenPos;
        }

        // 알파는 굳이 조절 안 해도 되지만,
        // 혹시 모를 경우를 위해 1로 유지하거나 0으로 초기화하고 싶으면 여기서 설정.
        /*
        if (comboImage != null)
        {
            Color c = comboImage.color;
            c.a = 0f;
            comboImage.color = c;
        }
        */

        // 패널은 절대 SetActive(false) 하지 않는다!
        // if (comboPanel != null)
        //     comboPanel.SetActive(false);
    }

    public void ResetEffect()
    {
        lastShownThreshold = -1;
        HideImmediate();
    }
}
