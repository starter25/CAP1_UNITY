using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PoseGameManager의 poseTimeline을 읽어서,
/// 다음 포즈 시간에 맞춰 반투명 픽토그램(잔상)을 보여주는 컨트롤러.
/// PoseGameManager 코드는 수정하지 않는다.
/// </summary>
public class PictogramGhostController : MonoBehaviour
{
    [Header("References")]
    public PoseGameManager gameManager;  // PoseGameManager 오브젝트 드래그
    public Image ghostImage;             // 잔상용 UI Image

    [Header("Timing")]
    [Tooltip("포즈 목표 시간 몇 초 전에 잔상을 보여줄지")]
    public float showLeadTime = 1.0f;

    [Tooltip("판정 구간(windowAfter) 이후에도 잔상을 유지할 추가 시간")]
    public float extraVisibleAfterWindow = 0.1f;

    [Header("Visual")]
    [Range(0f, 1f)]
    public float ghostAlpha = 0.35f;     // 잔상 투명도

    [Header("PoseName → Sprite 매핑")]
    public List<PosePictogramMapping> mappings = new List<PosePictogramMapping>();

    // 내부 상태
    PoseGameManager.PoseTimelineEntry currentEntry;
    bool hasSpriteForCurrentEntry = false;

    [Serializable]
    public class PosePictogramMapping
    {
        public string poseName;  // JSON의 pose_name 과 동일하게
        public Sprite sprite;    // 해당 포즈 픽토그램
    }

    void Start()
    {
        if (ghostImage != null)
        {
            ghostImage.enabled = false;
        }
    }

    void Update()
    {
        if (gameManager == null || ghostImage == null)
            return;

        float songTime = gameManager.CurrentSongTime;

        // 1) 현재 타겟이 없거나 이미 끝났으면 새로운 타겟 포즈 찾기
        if (currentEntry == null ||
            currentEntry.judged ||
            songTime > currentEntry.time + currentEntry.windowAfter + extraVisibleAfterWindow)
        {
            currentEntry = FindNextTargetEntry(songTime);
            SetupSpriteForCurrentEntry();
        }

        // 타겟이 없거나 이 포즈에 쓸 스프라이트 자체가 없으면 그냥 숨김
        if (currentEntry == null || !hasSpriteForCurrentEntry)
        {
            ghostImage.enabled = false;
            return;
        }

        // 2) 이 포즈에 대해 잔상을 보여줄 시간 구간 계산
        float showTime = currentEntry.time - showLeadTime;
        float endTime  = currentEntry.time + currentEntry.windowAfter;

        bool shouldShow =
            !currentEntry.judged &&
            songTime >= showTime &&
            songTime <= endTime + extraVisibleAfterWindow;

        ghostImage.enabled = shouldShow;
    }

    /// <summary>
    /// 아직 judged 되지 않은 포즈 중,
    /// 현재 시간 기준으로 "가장 가까운" 다음 포즈를 찾는다.
    /// </summary>
    PoseGameManager.PoseTimelineEntry FindNextTargetEntry(float songTime)
    {
        PoseGameManager.PoseTimelineEntry closest = null;
        float minDt = float.MaxValue;

        foreach (var e in gameManager.poseTimeline)
        {
            if (e == null) continue;
            if (e.judged) continue;
            if (e.refPose == null) continue;

            float dt = e.time - songTime;

            // 너무 과거(충분히 지나간 포즈)는 무시
            if (songTime > e.time + e.windowAfter + extraVisibleAfterWindow)
                continue;

            // showLeadTime 이전 구간까지 포함해서
            if (dt >= -showLeadTime && dt < minDt)
            {
                minDt = dt;
                closest = e;
            }
        }

        return closest;
    }

    /// <summary>
    /// currentEntry에 맞는 스프라이트를 찾아서 ghostImage에 세팅.
    /// 포즈 이름은 refPose.pose_name 을 사용.
    /// </summary>
    void SetupSpriteForCurrentEntry()
    {
        hasSpriteForCurrentEntry = false;

        if (ghostImage == null)
            return;

        if (currentEntry == null || currentEntry.refPose == null)
        {
            ghostImage.enabled = false;
            return;
        }

        string poseName = currentEntry.refPose.pose_name;
        // 어떤 이름이 들어오는지 확인용 로그
        Debug.Log($"[PictogramGhost] target pose_name = {poseName}");

        Sprite sprite = FindSpriteForPoseName(poseName);

        if (sprite != null)
        {
            ghostImage.sprite = sprite;

            // 알파 조정해서 반투명 처리
            Color c = ghostImage.color;
            c.a = ghostAlpha;
            ghostImage.color = c;

            hasSpriteForCurrentEntry = true;
            ghostImage.enabled = false; // 시간 맞을 때 Update에서 켬
        }
        else
        {
            // 매핑이 없으면 아예 안 보이게
            Debug.LogWarning($"[PictogramGhost] 매핑 없음: pose_name = {poseName}");
            ghostImage.enabled = false;
        }
    }

    Sprite FindSpriteForPoseName(string poseName)
    {
        foreach (var m in mappings)
        {
            if (!string.IsNullOrEmpty(m.poseName) &&
                m.poseName == poseName &&
                m.sprite != null)
            {
                return m.sprite;
            }
        }
        return null;
    }
}
