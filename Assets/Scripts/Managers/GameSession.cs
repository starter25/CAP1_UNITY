using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    // 0: 치어리더, 1: 남자 이런 식으로 구분
    public int selectedCaptainId = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("[GameSession] duplicate destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[GameSession] Awake, selectedCaptainId = " + selectedCaptainId);
    }
}