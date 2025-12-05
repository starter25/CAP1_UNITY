using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Score System Reference")]
    public PoseScoreSystem scoreSystem;   // 인스펙터에서 드래그

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
    /// - 결과 씬으로 이동
    /// </summary>
public void EndGame()
{
    if (isGameEnded) return;
    isGameEnded = true;

    Debug.Log("[GameManager] EndGame() 호출됨 - 결과 저장 및 씬 이동 시작");

    if (scoreSystem != null)
    {
        GameResultData.Score      = scoreSystem.TotalScore;
        GameResultData.Perfect    = scoreSystem.PerfectCount;
        GameResultData.Great      = scoreSystem.GreatCount;
        GameResultData.Good       = scoreSystem.GoodCount;
        GameResultData.Miss       = scoreSystem.MissCount;
        GameResultData.MaxCombo   = scoreSystem.MaxCombo;
        GameResultData.FeverGauge = scoreSystem.FeverGauge;   // 0~1 값
        GameResultData.FeverCount = scoreSystem.FeverFillCount;

    }
    else
    {
        Debug.LogWarning("[GameManager] scoreSystem이 비어있음 - 결과값을 저장하지 못함");
    }

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
