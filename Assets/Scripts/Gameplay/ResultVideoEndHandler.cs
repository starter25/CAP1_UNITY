using UnityEngine;
using UnityEngine.Video;

public class ResultVideoEndHandler : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;   // 결과 영상 재생하는 VideoPlayer
    public GameManager gameManager;   // 엔딩 실행 담당

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            // 영상이 끝나는 시점에 자동으로 호출되는 이벤트 등록
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("[ResultVideoEndHandler] VideoPlayer가 할당되지 않았습니다.");
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("[ResultVideoEndHandler] 영상 종료 감지 → GameManager.EndGame() 호출");

        if (gameManager != null)
        {
            gameManager.EndGame();
        }
        else
        {
            Debug.LogError("[ResultVideoEndHandler] GameManager가 할당되지 않아서 EndGame()을 호출할 수 없습니다.");
        }
    }
}
