using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;


/// <summary>
/// 1) UdpPoseReceiver에서 실시간 관절 각도(Dictionary<string,float>) 받기
/// 2) PoseTimeline(시간 + 포즈 JSON + 타임 윈도우)에 따라
///    각 포즈 구간에서 "최고 점수"를 기록
/// 3) 구간이 끝날 때, 최종 점수를 PoseScoreSystem에 넘겨서
///    PERFECT / GREAT / GOOD / MISS + 누적 점수/게이지 처리
/// </summary>
public class PoseGameManager : MonoBehaviour
{
    public float CurrentSongTime => songTime;

    [Header("Video Manager")]
    public ScoreVideoManager videoManager;

    [Header("References")]
    [Tooltip("UDP로 포즈 각도를 받아오는 리시버 (UdpPoseReceiver 붙어있는 오브젝트 드래그)")]
    public UdpPoseReceiver receiver;

    [Tooltip("현재/다음 목표 동작 이름 표시용 텍스트")]
    public TMP_Text poseText;

    [Tooltip("실시간 점수 표시용 텍스트 (포즈 정확도 수치)")]
    public TMP_Text scoreText;

    [Header("Score System")]
    [Tooltip("점수/등급/게이지 관리 스크립트 (PoseScoreSystem 붙은 오브젝트 드래그)")]
    public PoseScoreSystem scoreSystem;

    [Header("Video Sync")]
    [Tooltip("메인 영상 재생용 VideoPlayer (MainVideoPlayer 오브젝트 드래그)")]
    public VideoPlayer mainVideoPlayer;

    // ================== 타임라인 설정 ==================
    [Serializable]
    public class PoseTimelineEntry
    {
        [Tooltip("이 포즈를 맞춰야 하는 목표 시간 (초) - 노래/영상 타이밍 기준")]
        public float time = 0f;

        [Tooltip("이 시간에 맞출 포즈 JSON (Resources/ 또는 아무 폴더의 TextAsset)")]
        public TextAsset poseJson;

        [Tooltip("목표 시간보다 '얼마나 일찍'부터 판정을 시작할지 (초)")]
        public float windowBefore = 0.3f;

        [Tooltip("목표 시간보다 '얼마나 늦게'까지 허용할지 (초)")]
        public float windowAfter = 0.3f;

        // --------- 런타임용 캐시/상태 ---------
        [NonSerialized] public PoseComparer.RefPoseData refPose;  // JSON 파싱 결과
        [NonSerialized] public bool judged = false;               // 이 포즈는 판정 끝났는지
        [NonSerialized] public float bestScore = 0f;              // 이 포즈 구간에서 나온 최고 점수
    }

    [Header("Pose Timeline")]
    public List<PoseTimelineEntry> poseTimeline = new List<PoseTimelineEntry>();

    // ================== 판정 세팅 ==================
    [Header("Judge Settings")]
    [Tooltip("각도 차이가 이 값일 때 관절 점수는 0점 (크게 줄수록 관대해짐)")]
    public float angleTolerance = 60f;

    [Tooltip("몇 프레임마다 점수 계산할지 (1이면 매 프레임, 2면 격프레임 등)")]
    public int judgeInterval = 2;

    // 내부 시간 (지금은 그냥 Time.time 기반. 나중에 영상/음악 시간과 동기화해도 됨)
    private float songTime = 0f;
    private int frameCounter = 0;

    [Header("Game End Settings")]
    [Tooltip("이 시간이 지나면 포즈 판정을 종료합니다 (초)")]
    public float gameplayEndTime = 30f;  // 인스펙터에서 조절

    [Tooltip("true가 되면 더 이상 판정/점수 갱신을 하지 않습니다.")]
    public bool isGameplayEnded = false;


    void Start()
    {
        // 타임라인마다 JSON을 미리 파싱해서 RefPoseData 캐시
        foreach (var entry in poseTimeline)
        {
            if (entry.poseJson == null) continue;

            try
            {
                entry.refPose = PoseComparer.LoadRefPose(entry.poseJson);
                if (entry.refPose == null || entry.refPose.joints == null)
                {
                    Debug.LogError($"[PoseGameManager] JSON 파싱 결과 joints 없음: {entry.poseJson.name}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PoseGameManager] JSON 파싱 실패 ({entry.poseJson.name}): {e.Message}");
            }
        }

        // 시간 0부터 시작
        songTime = 0f;

        // 점수/게이지 초기화
        if (scoreSystem != null)
        {
            scoreSystem.ResetScore();
        }
        
        // ★ 이 시점에 영상도 같이 시작 ★
        if (videoManager != null)
            videoManager.PlayDefaultVideo();

        // 처음 화면에 보여줄 목표 포즈 이름 (있으면)
        UpdatePoseTextForNextTarget();
    }

    /// <summary>
    /// 설정한 gameplayEndTime을 지나면 호출되는 게임플레이 종료 처리
    /// - 포즈 판정/점수 갱신을 멈추고, 이후에는 영상만 재생되게 만들고 싶을 때 사용
    /// </summary>
    void EndGameplay()
    {
        if (isGameplayEnded) return;

        isGameplayEnded = true;
        Debug.Log($"[PoseGameManager] 게임플레이 종료! time={songTime:F2}");

        // 필요하면 여기서 UI를 정리하거나 문구 바꾸기
        // 예: poseText.text = "";   // 다음 포즈 안내 지우기
        //     scoreText.text += "\n(플레이 종료)";
    }

    void Update()
    {
        // 시간 진행 (현재는 씬 시작 후 경과 시간)
                if (mainVideoPlayer != null)
        {
            songTime = (float)mainVideoPlayer.time;
        }
        else
        {
            // 혹시 비디오플레이어 없을 때 대비용
            songTime += Time.deltaTime;
        }

        // ★ 디버그용 ★
        if (Time.frameCount % 30 == 0)  // 너무 많이 안 찍히게 30프레임마다
        {
            Debug.Log($"[PoseGameManager] songTime={songTime:F2}, endTime={gameplayEndTime:F2}, ended={isGameplayEnded}");
        }

        // ===== 2) 게임플레이 종료 시간 체크 =====
        if (!isGameplayEnded && songTime >= gameplayEndTime)
        {
            EndGameplay();
        }

        // 게임플레이가 끝난 뒤에는 판정/점수 관련 로직은 돌리지 않고 리턴
        // (영상은 VideoPlayer에서 계속 재생됨, songTime 갱신도 위에서 이미 됨)
        if (isGameplayEnded)
        {
            return;
        }

        // 포즈 데이터 아직 안 들어왔으면 스킵
        if (receiver == null || !receiver.HasValidPose)
            return;

        frameCounter++;
        if (judgeInterval <= 0) judgeInterval = 1;  // 안전장치
        if (frameCounter % judgeInterval != 0)
            return;

        // 현재 실시간 포즈 (Dictionary<string,float>)
        Dictionary<string, float> currentPose = receiver.LatestAngles;
        if (currentPose == null || currentPose.Count == 0)
            return;

        bool anyUpdated = false;

        // 아직 판정이 안 끝난 포즈들을 순회
        foreach (var entry in poseTimeline)
        {
            if (entry.judged) continue;              // 이미 끝난 포즈
            if (entry.refPose == null) continue;     // JSON 파싱 실패한 포즈

            float startT = entry.time - entry.windowBefore;
            float endT   = entry.time + entry.windowAfter;

            // 아직 판정 구간이 오지 않음 → 다음 루프에서 다시 체크
            if (songTime < startT)
                continue;

            // 판정 구간이 지났으면, 최고점 기준으로 최종 판정
            if (songTime > endT)
            {
                entry.judged = true;
                FinalizeJudgement(entry);
                anyUpdated = true;
                continue;
            }

            // ===== 여기서가 실제 판정 구간 (startT ~ endT) =====
            float score = PoseComparer.Compare(
                currentPose,
                entry.refPose,
                angleTolerance
            );

            if (score > entry.bestScore)
                entry.bestScore = score;

            // 실시간 점수 UI (원하면)
            if (scoreText != null)
                scoreText.text = $"{score:F1}";
        }

        // 어떤 포즈가 막 끝났다면, "다음 목표 포즈" 텍스트 갱신
        if (anyUpdated)
        {
            UpdatePoseTextForNextTarget();
        }
    }

    /// <summary>
    /// 한 포즈의 판정 구간이 끝났을 때, bestScore를 ScoreSystem에 넘겨줌
    /// </summary>
    void FinalizeJudgement(PoseTimelineEntry entry)
    {
        float finalScore = entry.bestScore;
        string poseName = (entry.refPose != null) ? entry.refPose.pose_name : "Unknown";

        if (scoreSystem != null)
        {
            // 등급/누적점수/게이지 업데이트는 PoseScoreSystem이 전담
            scoreSystem.HandleJudgement(finalScore, poseName);
        }
        else
        {
            Debug.LogWarning(
                $"[PoseGameManager] scoreSystem이 할당되지 않음. " +
                $"Pose '{poseName}' finalScore={finalScore:F1} 만 로그로 출력합니다."
            );
        }
    }

    /// <summary>
    /// 아직 판정이 안 끝난 포즈 중에서, "현재 시간 이후에 가장 가까운" 포즈 이름을 표시
    /// </summary>
    void UpdatePoseTextForNextTarget()
    {
        if (poseText == null) return;

        PoseTimelineEntry closest = null;
        float minDt = float.MaxValue;

        foreach (var e in poseTimeline)
        {
            if (e.judged) continue;
            if (e.refPose == null) continue;

            float dt = e.time - songTime;
            if (dt >= 0f && dt < minDt)
            {
                minDt = dt;
                closest = e;
            }
        }

        if (closest != null)
        {
            poseText.text = closest.refPose.pose_name;
        }
        else
        {
            // 남은 포즈가 없으면 비워두거나 "끝!" 같은 문구로 변경 가능
            // poseText.text = "";
        }
    }
}

