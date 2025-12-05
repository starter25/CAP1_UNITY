using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    // 0: 치어리더, 1: 드러머 이런 식으로 구분
    public int selectedCaptainId = 0;

    private void Awake()
    {
        // 싱글톤 패턴 + 씬 넘어가도 안 없어지게
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
