using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "PlayScene";

    private AsyncOperation loadOp;

    void Start()
    {
        // 1) 다음 씬 미리 로드 (비동기)
        loadOp = SceneManager.LoadSceneAsync(nextSceneName);
        loadOp.allowSceneActivation = false;  // 영상 끝날 때까지는 넘어가지 않기

        // 2) 비디오 끝났을 때 콜백 등록
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // 3) 로딩이 끝났고, 영상도 끝났으니 씬 전환 허용
        loadOp.allowSceneActivation = true;
    }
}
