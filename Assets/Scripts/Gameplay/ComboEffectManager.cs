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
    public GameObject comboPanel;      // ComboEffectPanel

    [Tooltip("패널 안에서 이미지를 표시할 UI Image")]
    public Image comboImage;           // 자식 Image

    [Header("Effect Settings")]
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
        HideImmediate();
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
        if (comboPanel == null || comboImage == null)
            return;

        comboImage.sprite = sprite;
        comboImage.SetNativeSize();  // 필요하면 이미지 원본 크기 기준

        comboPanel.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        // 알파 1로 시작
        Color c = comboImage.color;
        c.a = 1f;
        comboImage.color = c;

        yield return new WaitForSecondsRealtime(showDuration);

        // 페이드 아웃
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

    public void HideImmediate()
    {
        if (comboImage != null)
        {
            Color c = comboImage.color;
            c.a = 0f;
            comboImage.color = c;
        }

        if (comboPanel != null)
            comboPanel.SetActive(false);
    }

    public void ResetEffect()
    {
        lastShownThreshold = -1;
        HideImmediate();
    }
}
