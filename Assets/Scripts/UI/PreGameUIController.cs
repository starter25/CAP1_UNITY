using UnityEngine;
using UnityEngine.SceneManagement;



public class PreGameUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject missionPanel;        // 미션 이미지 패널
    public GameObject captainSelectPanel;  // 응원단장 선택 패널

    void Start()
    {
        // 처음 상태: 미션 패널 ON, 응원단장 패널 OFF
        if (missionPanel != null) missionPanel.SetActive(true);
        if (captainSelectPanel != null) captainSelectPanel.SetActive(false);
    }

    // 미션 닫기 버튼에서 호출할 함수
    public void OnClickCloseMission()
    {
        if (missionPanel != null) missionPanel.SetActive(false);
        if (captainSelectPanel != null) captainSelectPanel.SetActive(true);
    }

    public void OnClickStartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }
        public void OnClickSelectCaptain(int captainId)
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.selectedCaptainId = captainId;
        }

        // 선택 끝났으니 PlayScene으로 이동
        SceneManager.LoadScene("PlayScene");
    }
}
