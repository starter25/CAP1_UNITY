using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PoseJudgeEffect : MonoBehaviour
{
    [Header("UI")]
    public Image judgeImage;
    public float showDuration = 0.6f;

    [Header("Sprites")]
    public Sprite perfectSprite;
    public Sprite greatSprite;
    public Sprite goodSprite;
    public Sprite missSprite;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip perfectClip;
    public AudioClip greatClip;
    public AudioClip goodClip;
    public AudioClip missClip;

    Coroutine currentRoutine;

    public void Show(string grade)
    {
        if (judgeImage == null) return;

        // 어떤 스프라이트/사운드를 쓸지 결정
        Sprite sprite = null;
        AudioClip clip = null;

        switch (grade)
        {
            case "PERFECT":
                sprite = perfectSprite;
                clip = perfectClip;
                break;
            case "GREAT":
                sprite = greatSprite;
                clip = greatClip;
                break;
            case "GOOD":
                sprite = goodSprite;
                clip = goodClip;
                break;
            case "MISS":
                sprite = missSprite;
                clip = missClip;
                break;
        }

        if (sprite == null)
            return;

        // 이미지 설정 + 표시
        judgeImage.sprite = sprite;
        judgeImage.enabled = true;
        judgeImage.gameObject.SetActive(true);

        // 소리 재생
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);

        // 일정 시간 뒤 자동으로 끄기
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);

        if (judgeImage != null)
            judgeImage.enabled = false;
    }
}
