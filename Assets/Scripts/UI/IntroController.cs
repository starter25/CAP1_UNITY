using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "PreGameScene";

    void Start()
    {
        // 영상이 끝났을 경우 자동으로 다음 씬 이동
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    public void SkipVideo()
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
