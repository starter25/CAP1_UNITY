using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // 플레이 버튼에서 실행할 함수
    public void OnClickPlay()
    {
        SceneManager.LoadScene("IntroScene"); // 네 플레이 씬 이름과 정확히 맞춰야 함
    }
}
