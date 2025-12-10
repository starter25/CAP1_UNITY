using UnityEngine;
using UnityEngine.InputSystem;

public class MouseEffect : MonoBehaviour
{
    public GameObject starPrefab;
    private float spawnTime;
    public float defaultTime = 0.05f;

    void Update()
    {
        // 마우스 좌클릭 유지
        if (Mouse.current.leftButton.isPressed && spawnTime >= defaultTime)
        {
            CreateStar();
            spawnTime = 0f;
        }

        spawnTime += Time.deltaTime;
    }

    void CreateStar()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        worldPos.z = 0f;

        Instantiate(starPrefab, worldPos, Quaternion.identity);
    }
}
