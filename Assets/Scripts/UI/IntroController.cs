using System.Collections;          // ★ 코루틴(IEnumerator) 쓰려면 필요
using UnityEngine;
using UnityEngine.UI;              // ★ Image 쓰려면 필요
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public string nextSceneName = "PreGameScene";

    [Header("Fade")]
    public Image fadePanel;         // Canvas 안의 검은 패널
    public float fadeDuration = 0.5f;

    private bool isSkipping = false;

    void Start()
    {
        // 영상 끝나면 자동으로 다음 씬으로
        videoPlayer.loopPointReached += OnVideoEnd;

        // 페이드 패널 초기화 (완전 검은색으로 시작)
        if (fadePanel != null)
        {
            var c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(true);
        }

        // 영상 준비 + 페이드 아웃 시작
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 1) 영상 준비
        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;

            videoPlayer.Play();
        }

        // 2) 페이드 아웃
        if (fadePanel != null)
            yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        fadePanel.gameObject.SetActive(false);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (isSkipping) return;
        LoadNextScene();
    }

    public void SkipVideo()
    {
        if (isSkipping) return;
        isSkipping = true;
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
