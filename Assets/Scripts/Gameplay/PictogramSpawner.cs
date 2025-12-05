using UnityEngine;

[System.Serializable]
public class TimedPictogram
{
    public float time;      // 몇 초에 나올지 (게임 전체 시간 기준)
    public int poseIndex;   // poseSprites 배열에서 몇 번째 스프라이트 쓸지 (0부터)
}

public class PictogramSpawner : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform container;         // PictogramContainer
    public PictogramItem pictogramPrefab;   // PictogramItem 프리팹
    public Sprite[] poseSprites;            // 0: one, 1: two ...

    [Header("Movement Settings")]
    public Vector2 spawnPos = new Vector2(50f, 800f);   // 우하단 시작 위치
    public Vector2 stopPos  = new Vector2(-200f, 80f);  // 멈출 위치
    public float moveDuration = 1.0f;   // 이동 시간
    public float holdDuration = 0.3f;   // 멈춰 있는 시간
    public float fadeDuration = 0.5f;   // 잔상 사라지는 시간

    [Header("Schedule")]
    public TimedPictogram[] schedule;

    [Header("Time Source")]
    [Tooltip("게임 전체 시간을 제공하는 PoseGameManager (CurrentSongTime 사용)")]
    public PoseGameManager poseGameManager;

    // 로컬 타이머(백업용) + 스케줄 인덱스
    float localSongTime = 0f;
    int nextIndex = 0;

    void Start()
    {
        localSongTime = 0f;
        nextIndex = 0;
    }

    void Update()
    {
        // 1) 기준 시간 가져오기
        float songTime;

        if (poseGameManager != null)
        {
            // 메인 기준: PoseGameManager의 CurrentSongTime
            songTime = poseGameManager.CurrentSongTime;
        }
        else
        {
            // 혹시 참조 안 되어 있을 때는 로컬 타이머로라도 진행
            localSongTime += Time.deltaTime;
            songTime = localSongTime;
        }

        // 2) 스케줄 시간에 도달하면 픽토그램 생성
        while (schedule != null &&
               nextIndex < schedule.Length &&
               songTime >= schedule[nextIndex].time)
        {
            SpawnPictogram(schedule[nextIndex]);
            nextIndex++;
        }
    }

    void SpawnPictogram(TimedPictogram tp)
    {
        if (tp.poseIndex < 0 || tp.poseIndex >= poseSprites.Length)
        {
            Debug.LogWarning($"[PictogramSpawner] 잘못된 poseIndex: {tp.poseIndex}");
            return;
        }

        var item = Instantiate(pictogramPrefab, container);
        item.Init(
            poseSprites[tp.poseIndex],
            spawnPos,
            stopPos,
            moveDuration,
            holdDuration,
            fadeDuration
        );
    }
}
