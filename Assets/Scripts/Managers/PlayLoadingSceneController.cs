using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayLoadingSceneController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip femaleClip;   // 여자 응원단장 영상
    public VideoClip maleClip;     // 남자 응원단장 영상

    [Header("Next Scene")]
    public string playSceneName = "PlayScene";

    [Header("Fade (선택)")]
    public Image fadePanel;        // 검은 패널 (없으면 비워둬도 됨)
    public float fadeDuration = 0.5f;

    void Start()
    {
        // 1) GameSession에서 어떤 단장을 선택했는지 읽기
        int captainId = 0;
        if (GameSession.Instance != null)
            captainId = GameSession.Instance.selectedCaptainId;

        // 0 = 여자, 1 = 남자 라는 가정
        if (captainId == 1 && maleClip != null)
            videoPlayer.clip = maleClip;
        else
            videoPlayer.clip = femaleClip;

        // 영상 끝나면 자동으로 PlayScene 로딩
        videoPlayer.loopPointReached += OnVideoEnd;

        // 페이드 패널 초기화
        if (fadePanel != null)
        {
            var c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(true);
        }

        StartCoroutine(PlayVideoRoutine());
    }

    IEnumerator PlayVideoRoutine()
    {
        // 영상 준비
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        // 재생 시작
        videoPlayer.Play();

        // 페이드 아웃
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
        SceneManager.LoadScene(playSceneName);
    }

    // 스킵 버튼 만들고 싶으면 이 함수 버튼에 연결
    public void Skip()
    {
        SceneManager.LoadScene(playSceneName);
    }
}
