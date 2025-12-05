using UnityEngine;
using UnityEngine.Video;

using UnityEngine.SceneManagement;



public class PauseManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;     // MainVideoPlayer
    public PoseGameManager gameManager; // songTime 멈추기 용
    public GameObject pauseMenuUI;      // 일시정지 패널

    private bool isPaused = false;

    void Update()
    {
        // ESC로 일시정지/해제 가능 (추가 기능)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) Pause();
            else Resume();
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;

        // 시간 정지 (Update는 돌지만 Time.deltaTime이 0이 됨)
        Time.timeScale = 0f;

        // 영상 정지
        if (videoPlayer != null)
            videoPlayer.Pause();

        // UI 패널 활성화
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Debug.Log("=== Game Paused ===");
    }

    public void Resume()
    {
        if (!isPaused) return;

        isPaused = false;

        // 시간 다시 재생
        Time.timeScale = 1f;

        // 영상 재생
        if (videoPlayer != null)
            videoPlayer.Play();

        // UI 패널 비활성화
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Debug.Log("=== Game Resumed ===");
    }

        public void RestartGame()
    {
        // 혹시 일시정지 상태일 수 있으니 먼저 원래 속도로 되돌리기
        Time.timeScale = 1f;

        // 필요하면 영상도 정지
        if (videoPlayer != null)
            videoPlayer.Stop();

        // 현재 활성화된 씬 이름 다시 로드 → 처음부터
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // 혹시 Pause 상태일 수 있으니 원래대로
        SceneManager.LoadScene("MainScene"); // 메인 씬 이름
    }
}

