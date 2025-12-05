using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultSceneUI : MonoBehaviour
{
    [Header("Result Texts")]
    public TMP_Text scoreText;
    public TMP_Text perfectText;
    public TMP_Text greatText;
    public TMP_Text goodText;
    public TMP_Text missText;
    public TMP_Text maxComboText;
    public TMP_Text feverGaugeText;

    void Start()
    {
        // GameResultData에 PlayScene에서 저장한 값들이 들어와 있음
        if (scoreText != null)
            scoreText.text = GameResultData.Score.ToString("N0");

        if (perfectText != null)
            perfectText.text = $"Perfect : {GameResultData.Perfect}";

        if (greatText != null)
            greatText.text = $"Great   : {GameResultData.Great}";

        if (goodText != null)
            goodText.text = $"Good    : {GameResultData.Good}";

        if (missText != null)
            missText.text = $"Miss    : {GameResultData.Miss}";

        if (maxComboText != null)
            maxComboText.text = $"Max Combo : {GameResultData.MaxCombo}";

        if (feverGaugeText != null)
        {
            // 0~1 값이면 퍼센트로 보고 싶을 때
            //feverGaugeText.text = $"Fever : {GameResultData.FeverGauge * 100f:0}%";
            // 또는 그냥 숫자로
            //feverGaugeText.text = GameResultData.FeverGauge.ToString("0.00");
            float percent = GameResultData.FeverGauge * 100f;
            int count = GameResultData.FeverCount;
            // 🔹 "n회 (n%)" 형식으로 표시
            feverGaugeText.text = $"Fever : {count}, ({percent:0}%)";
        }
    }

        // 🔹 메인 화면으로 가는 버튼에서 호출할 함수
    public void OnClickGoToMain()
    {
        SceneManager.LoadScene("MainScene");  // ← 메인 씬 이름이랑 정확히 맞춰야 함
    }

    public void OnClickReplay()
    {
        SceneManager.LoadScene("PlayScene");   // ← 너의 플레이씬 이름과 동일해야 함
    }
}
