using UnityEngine;
using UnityEngine.Video;

public class PlaySceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // 0 = 여자, 1 = 남자 라고 가정
    public VideoClip femaleVideo;
    public VideoClip maleVideo;

    void Start()
    {
        int id = 0;

        if (GameSession.Instance != null)
            id = GameSession.Instance.selectedCaptainId;

        // 선택값에 따라 영상 변경
        if (id == 0)
        {
            videoPlayer.clip = femaleVideo;
        }
        else if (id == 1)
        {
            videoPlayer.clip = maleVideo;
        }

        videoPlayer.Play();
    }
}
