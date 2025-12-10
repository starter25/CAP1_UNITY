using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneWithSound : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("버튼 클릭 후 이동할 씬 이름")]
    public string nextSceneName = "PlayScene";

    [Header("Sound")]
    public AudioSource audioSource;    // 버튼에 붙어 있는 AudioSource
    public AudioClip clickClip;       // 클릭 효과음
    public float delay = 0.15f;       // 씬 전환까지 기다릴 시간(초)

    bool isClicked = false;

    // 버튼의 OnClick() 이벤트에 이 함수를 연결하면 됨
    public void OnClick()
    {
        if (isClicked) return; // 중복 클릭 방지
        isClicked = true;

        // 1) 효과음 재생
        if (audioSource != null && clickClip != null)
        {
            audioSource.PlayOneShot(clickClip);
        }

        // 2) delay 초 뒤에 씬 로드
        Invoke(nameof(LoadScene), delay);
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
