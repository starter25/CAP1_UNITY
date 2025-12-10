using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlaySceneVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public Image fadePanel;          // 검은 패널

    void Start()
    {
        // 처음엔 무조건 까맣게 시작
        fadePanel.transform.SetAsLastSibling(); // hierarchy에서 가장 위(제일 나중에 그림)
        fadePanel.gameObject.SetActive(true);

        StartCoroutine(PlayVideoAndFadeIn());
    }

    System.Collections.IEnumerator PlayVideoAndFadeIn()
    {
        // 먼저 영상 준비
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        // 준비 끝났으면 영상 + 노래 재생 시작
        videoPlayer.Play();
        if (audioSource != null)
            audioSource.Play();

        // 한 프레임은 그리게 살짝 기다렸다가
        yield return new WaitForEndOfFrame();

        // 이제 검은 화면 걷어내기
        fadePanel.gameObject.SetActive(false);
    }
}
