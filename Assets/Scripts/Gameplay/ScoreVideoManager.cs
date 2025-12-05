using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ScoreVideoManager : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;      // 영상 재생기
    public PoseScoreSystem scoreSystem;  // 점수 시스템
    public PoseGameManager poseGameManager; // 시간(songTime) 기준 제공자

    [Header("Default Video")]
    [Tooltip("게임 시작 시 재생할 기본 배경 영상")]
    public VideoClip defaultClip;

    [Serializable]
    public class TimedScoreVideo
    {
        [Tooltip("이 시간이 지나면 조건을 체크합니다 (초)")]
        public float triggerTime = 30f;

        [Tooltip("이 점수 이상이면 successClip, 아니면 failClip 재생")]
        public int requiredScore = 10000;

        [Tooltip("조건을 만족했을 때 재생할 영상")]
        public VideoClip successClip;

        [Tooltip("조건을 만족 못했을 때 재생할 영상")]
        public VideoClip failClip;

        [NonSerialized] public bool triggered = false; // 이미 처리했는지
    }

    [Header("Checkpoints")]
    public List<TimedScoreVideo> checkpoints = new List<TimedScoreVideo>();


    // ========== 게임 시작 시 PoseGameManager가 직접 호출할 함수 ==========

    /// <summary>
    /// PoseGameManager.Start() 시점에서 영상과 songTime을 동시에 시작함.
    /// </summary>
    public void PlayDefaultVideo()
    {
        if (videoPlayer == null || defaultClip == null)
        {
            Debug.LogWarning("[ScoreVideoManager] 기본 영상 재생 불가 (videoPlayer 또는 defaultClip 누락)");
            return;
        }

        videoPlayer.clip = defaultClip;
        videoPlayer.time = 0;  // 혹시 이전 재생 위치가 남아있으면 초기화
        videoPlayer.Play();

        Debug.Log("[ScoreVideoManager] 기본 영상 재생 시작");
    }


    // ======================= 매 프레임 체크 =======================
    void Update()
    {
        if (videoPlayer == null || scoreSystem == null || poseGameManager == null)
            return;

        float t = poseGameManager.CurrentSongTime;  // songTime 기반 (영상과 동기화)
        int score = scoreSystem.TotalScore;         // 현재 누적 점수

        foreach (var cp in checkpoints)
        {
            if (cp.triggered) continue;

            if (t >= cp.triggerTime)
            {
                VideoClip nextClip = null;

                // 점수 조건 검사
                if (score >= cp.requiredScore && cp.successClip != null)
                    nextClip = cp.successClip;
                else if (score < cp.requiredScore && cp.failClip != null)
                    nextClip = cp.failClip;

                // 재생
                if (nextClip != null)
                {
                    videoPlayer.clip = nextClip;
                    videoPlayer.time = 0;  // 새 영상 시작은 항상 0초
                    videoPlayer.Play();

                    Debug.Log($"[ScoreVideoManager] {t:F1}초 체크포인트 발동 → '{nextClip.name}' 재생");
                }

                cp.triggered = true;
            }
        }
    }
}
