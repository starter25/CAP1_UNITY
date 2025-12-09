using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class FeverTimeManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("피버타임 이미지를 표시할 UI Image")]
    public Image feverImage;

    [Tooltip("게이지 옆에 표시할 피버 횟수 텍스트 (x1, x2, x3...)")]
    public TMP_Text multiplierText;

    [Header("Fever Settings")]
    [Tooltip("피버 이미지가 유지되는 시간(초)")]
    public float showDuration = 1.5f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip feverClip;

    [Header("Fever Video")]
    [Tooltip("피버 영상이 들어있는 패널 (RawImage + RectMask2D 포함)")]
    public GameObject feverVideoPanel;     // 캔버스 안 Panel

    [Tooltip("피버 영상을 재생할 VideoPlayer 오브젝트")]
    public VideoPlayer feverVideoPlayer;   // 씬에 있는 VideoPlayer

    [Tooltip("피버 시 영상 재생 여부")]
    public bool playVideoOnFever = true;

    [Tooltip("피버 종료 시 영상 정지할지 여부")]
    public bool stopVideoWhenHidden = true;

    // 🔥 피버 횟수 (배율이 아니라 '몇 번째 피버인지' 표시용)
    int feverCount = 0;
    
    public int FeverCount => feverCount;   // 외부에서 읽기용
    
    float timer = 0f;
    bool isShowing = false;

    // 필요하면 쓸 수 있도록 남겨둔 프로퍼티 (현재는 점수 배율에는 사용 안 함)
    public int CurrentMultiplier => Mathf.Max(1, feverCount);

    void Start()
    {
        // 시작 상태 정리
        if (feverImage != null)
            feverImage.gameObject.SetActive(false);

            // ✅ 시작할 때 영상 패널도 꺼두기
            if (feverVideoPanel != null)
            feverVideoPanel.SetActive(false);

        UpdateMultiplierUI();
    }

    void Update()
    {
        if (!isShowing) return;

        timer -= Time.unscaledDeltaTime;
        if (timer <= 0f)
        {
            isShowing = false;
            if (feverImage != null)
                feverImage.gameObject.SetActive(false);

            // ✅ 시간 끝나면 영상 패널도 같이 끄기
            if (feverVideoPanel != null)
                feverVideoPanel.SetActive(false);

            if (stopVideoWhenHidden && feverVideoPlayer != null)
                feverVideoPlayer.Stop();
        }
    }

    /// <summary>
    /// 게이지가 꽉 찼을 때 PoseScoreSystem에서 호출
    /// 피버 횟수 1 증가 + 이미지/사운드 재생
    /// </summary>
    public void TriggerFever()
    {
        // 피버 횟수 증가 (x1, x2, x3... 용)
        feverCount++;
        UpdateMultiplierUI();

        // 피버 이미지 켜기
        if (feverImage != null)
            feverImage.gameObject.SetActive(true);

        // 🔥 피버 영상 패널 켜기 (여기가 포인트!)
        if (feverVideoPanel != null)
            feverVideoPanel.SetActive(true);

        // 🔥 비디오 재생
        if (feverVideoPlayer != null)
        {
            feverVideoPlayer.Stop();  // 이전 재생 중이었으면 초기화
            feverVideoPlayer.Play();
        }

        // 사운드 재생
        if (audioSource != null && feverClip != null)
            audioSource.PlayOneShot(feverClip);


        // 타이머 시작
        isShowing = true;
        timer = showDuration;

        Debug.Log($"[FeverTimeManager] FEVER! count = {feverCount}");
    }

    void HideFever()
    {
        isShowing = false;

        if (feverImage != null)
            feverImage.gameObject.SetActive(false);

        if (feverVideoPlayer != null)
            feverVideoPlayer.Stop();

        if (feverVideoPanel != null)
            feverVideoPanel.SetActive(false);
    }
    void UpdateMultiplierUI()
    {
        if (multiplierText == null) return;

        // 처음에는 x0 또는 공백으로 둘 수 있음 (취향대로 바꿔도 됨)
        if (feverCount <= 0)
            multiplierText.text = "x0";
        else
            multiplierText.text = $"x{feverCount}";
    }

    public void ResetFever()
    {
        feverCount = 0;
        isShowing = false;

        if (feverImage != null)
            feverImage.gameObject.SetActive(false);

        if (stopVideoWhenHidden && feverVideoPlayer != null)
            feverVideoPlayer.Stop();

        UpdateMultiplierUI();
    }
}
