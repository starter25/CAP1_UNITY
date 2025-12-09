using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 포즈 판정 결과(최종 점수)를 받아서
/// - PERFECT / GREAT / GOOD / MISS 등급 결정
/// - 콤보 계산 + 콤보 보너스
/// - 누적 점수 / 게이지
/// - Judge 이미지/사운드, 피버(게이지/횟수) 연동
/// 를 담당하는 스크립트
/// </summary>
public class PoseScoreSystem : MonoBehaviour
{

    public int FeverFillCount
    {
        get
        {
            if (feverManager != null)
                return feverManager.FeverCount;
            return 0;
        }
    }
    public int TotalScore => totalScore;

    [Header("Combo Effect (optional)")]
    public ComboEffectManager comboEffectManager;


    // ===================== 판정 기준 ======================
    [Header("Judge Thresholds (0~100 점수 기준)")]
    public float perfectThreshold = 70f;
    public float greatThreshold   = 55f;
    public float goodThreshold    = 40f;

    // ===================== 기본 점수 ======================
    [Header("Base Score Settings")]
    [Tooltip("PERFECT일 때 기본 점수")]
    public int perfectScore = 1000;

    [Tooltip("GREAT일 때 기본 점수")]
    public int greatScore = 700;

    [Tooltip("GOOD일 때 기본 점수")]
    public int goodScore = 400;

    [Tooltip("MISS일 때 기본 점수")]
    public int missScore = 0;

    // ===================== 콤보 ==========================
    [Header("Combo Settings")]
    [Tooltip("콤보 1당 추가 점수 (예: 20이면 콤보 5일 때 +100점)")]
    public int comboBonusPerCombo = 20;

    [Tooltip("콤보 표시용 텍스트 (선택)")]
    public TMP_Text comboText;

    private int comboCount = 0;

    // ===================== 결과 저장용 카운트 ======================
    private int perfectCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int missCount = 0;

    private int maxCombo = 0;


    // ===================== Judge 이미지/사운드 =============
    [Header("Judge Effect")]
    [Tooltip("퍼펙/그레잇/굿/미스 이미지/사운드 연출용 스크립트")]
    public PoseJudgeEffect judgeEffect;

    // ===================== 피버 / 게이지 ==================
    [Header("Fever Settings")]
    [Tooltip("피버/횟수 관리 스크립트")]
    public FeverTimeManager feverManager;

    [Tooltip("게이지가 꽉 차는 기준 점수 (게이지 전용 점수)")]
    public int maxGaugeScore = 50000;

    [Header("UI References")]
    [Tooltip("최근 판정 등급 텍스트")]
    public TMP_Text gradeText;

    [Tooltip("누적 점수 텍스트")]
    public TMP_Text totalScoreText;

    [Tooltip("점수 게이지 슬라이더 (min=0, max=1 추천)")]
    public Slider scoreGauge;

    // ===================== 내부 상태 ======================
    private int totalScore = 0;   // 실제 점수
    private int gaugeScore = 0;   // 게이지 전용 점수(피버용)

    // =====================================================
    //  메인 판정 처리 함수
    // =====================================================
    public void HandleJudgement(float finalScore, string poseName = "")
    {
        // 1. 등급 결정
        string grade;
        if (finalScore >= perfectThreshold)      grade = "PERFECT";
        else if (finalScore >= greatThreshold)   grade = "GREAT";
        else if (finalScore >= goodThreshold)    grade = "GOOD";
        else                                     grade = "MISS";

    // 판정 카운트 증가
    switch (grade)
    {
        case "PERFECT": perfectCount++; break;
        case "GREAT":   greatCount++;   break;
        case "GOOD":    goodCount++;    break;
        case "MISS":    missCount++;    break;
    }


        // 1-1. Judge 이미지 + 사운드
        if (judgeEffect != null)
            judgeEffect.Show(grade);

        // 2. 콤보 계산
        if (grade == "MISS")
        {
            comboCount = 0;
        }
        else
        {
            comboCount++;
        }

        // 최대 콤보 갱신
        if (comboCount > maxCombo)
            maxCombo = comboCount;

        if (comboText != null)
            comboText.text = (comboCount <= 1) ? "" : $"{comboCount} Combo!";

        // 🔥 콤보 이펙트에 알리기 (여기 한 줄 추가)
        if (comboEffectManager != null)
            comboEffectManager.OnComboChanged(comboCount);
        // 3. 기본 점수
        int baseScore = grade switch
        {
            "PERFECT" => perfectScore,
            "GREAT"   => greatScore,
            "GOOD"    => goodScore,
            _         => missScore
        };

        // 4. 콤보 보너스
        int comboBonus = (comboCount > 1) ? comboCount * comboBonusPerCombo : 0;
        int addScoreNoFever = baseScore + comboBonus;

        // 5. 점수 누적 (🔥 피버 횟수와 무관, 배율 X)
        int addScore = addScoreNoFever;
        totalScore += addScore;

        // 게이지는 addScoreNoFever 기준으로 채움
        gaugeScore += addScoreNoFever;

        // 6. UI 반영
        if (gradeText != null)
            gradeText.text = grade;

        if (totalScoreText != null)
            totalScoreText.text = totalScore.ToString("N0");

        float gauge01 = 0f;
        if (maxGaugeScore > 0)
            gauge01 = Mathf.Clamp01((float)gaugeScore / maxGaugeScore);

        if (scoreGauge != null)
            scoreGauge.value = gauge01;

        // 7. 게이지가 꽉 찼을 때 → 피버 발동 + 게이지 리셋
        if (gauge01 >= 1f && feverManager != null)
        {
            feverManager.TriggerFever();   // 피버 이미지/사운드 + 피버 횟수(x1, x2, x3...)
            gaugeScore = 0;                // 게이지 점수 리셋

            if (scoreGauge != null)
                scoreGauge.value = 0f;
        }

        Debug.Log($"[PoseScoreSystem] {poseName} : {grade}, combo={comboCount}, " +
                  $"+{addScore} (base={baseScore}, combo={comboBonus}), " +
                  $"total={totalScore}, gaugeScore={gaugeScore}");
    }

    // ===================== 외부에서 결과를 읽기 위한 Getter ======================
    public int PerfectCount => perfectCount;
    public int GreatCount   => greatCount;
    public int GoodCount    => goodCount;
    public int MissCount    => missCount;
    public int MaxCombo     => maxCombo;
    public float FeverGauge
    {
        get
        {
            if (maxGaugeScore > 0)
                return Mathf.Clamp01((float)gaugeScore / maxGaugeScore);
            return 0f;
        }
    }
    // =====================================================
    //  리셋 (게임/노래 재시작용)
    // =====================================================
    public void ResetScore()
    {
        totalScore = 0;
        gaugeScore = 0;
        comboCount = 0;

        if (totalScoreText != null)
            totalScoreText.text = "0";

        if (gradeText != null)
            gradeText.text = "";

        if (comboText != null)
            comboText.text = "";

        if (scoreGauge != null)
            scoreGauge.value = 0f;

        if (feverManager != null)
            feverManager.ResetFever();

        // 🔥 콤보 이펙트도 리셋
        if (comboEffectManager != null)
            comboEffectManager.ResetEffect();
    }

    // =====================================================
    //  🧪 디버그용 테스트 버튼 함수 (살려둔다!)
    // =====================================================
    public void TestPerfect()
    {
        // perfectThreshold보다 충분히 높은 값
        HandleJudgement(perfectThreshold + 10f, "TEST_PERFECT");
    }

    public void TestGreat()
    {
        HandleJudgement((perfectThreshold + greatThreshold) * 0.5f, "TEST_GREAT");
    }

    public void TestGood()
    {
        HandleJudgement((greatThreshold + goodThreshold) * 0.5f, "TEST_GOOD");
    }

    public void TestMiss()
    {
        HandleJudgement(0f, "TEST_MISS");
    }
}
