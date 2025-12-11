using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Score System Reference")]
    public PoseScoreSystem scoreSystem;   // 인스펙터에서 드래그

    [Header("Gameplay Objects To Disable On End")]
    [Tooltip("플레이 씬에서 노래 시간, 포즈 타겟 등을 관리하는 스크립트")]
    public PoseGameManager poseGameManager;     // 플레이씬의 PoseGameManager

    [Tooltip("픽토그램 전체를 담고 있는 부모 오브젝트 (없으면 비워둬도 됨)")]
    public GameObject pictogramRoot;            // 픽토그램 Canvas 또는 부모 오브젝트

    [Tooltip("플레이 화면 전체(노래, UI 등)를 담은 루트 오브젝트")]
    public GameObject gameplayRoot;             // 필요 없으면 비워둬도 OK

    [Header("Result Scene")]
    [Tooltip("결과 화면으로 사용할 씬 이름")]
    public string resultSceneName = "ResultScene";  // 실제 씬 이름으로 변경

    private bool isGameEnded = false;

    void Start()
    {
        // 새 판 시작할 때 이전 결과 초기화
        GameResultData.Reset();
    }

    /// <summary>
    /// 영상이 끝났을 때 호출되는 최종 엔딩 함수
    /// - 점수/판정/콤보/피버를 GameResultData에 저장
    /// - 플레이 관련 스크립트/오브젝트 비활성화
    /// - 결과 씬으로 이동
    /// </summary>
    public void EndGame()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("[GameManager] EndGame() 호출됨 - 결과 저장 및 씬 이동 시작");

        // 1) 플레이 로직 정지 ------------------------------------
        if (poseGameManager != null)
        {
            poseGameManager.enabled = false;   // songTime, 타겟포즈 업데이트 중단
        }

        if (pictogramRoot != null)
        {
            pictogramRoot.SetActive(false);    // 화면에서 픽토그램 숨기기
        }

        if (gameplayRoot != null)
        {
            gameplayRoot.SetActive(false);     // 필요하면 플레이 UI 전체 숨기기
        }

        // 2) 점수/결과 저장 --------------------------------------
        if (scoreSystem != null)
        {
            GameResultData.Score      = scoreSystem.TotalScore;
            GameResultData.Perfect    = scoreSystem.PerfectCount;
            GameResultData.Great      = scoreSystem.GreatCount;
            //GameResultData.Good     = scoreSystem.GoodCount;
            GameResultData.Miss       = scoreSystem.MissCount;
            GameResultData.MaxCombo   = scoreSystem.MaxCombo;
            GameResultData.FeverGauge = scoreSystem.FeverGauge;   // 0~1 값
            GameResultData.FeverCount = scoreSystem.FeverFillCount;
        }
        else
        {
            Debug.LogWarning("[GameManager] scoreSystem이 비어있음 - 결과값을 저장하지 못함");
        }

        // 3) 결과 씬으로 이동 ------------------------------------
        if (!string.IsNullOrEmpty(resultSceneName))
        {
            SceneManager.LoadScene(resultSceneName);
        }
        else
        {
            Debug.LogError("[GameManager] resultSceneName이 비어있음 - 씬을 로드할 수 없음");
        }
    }
}
