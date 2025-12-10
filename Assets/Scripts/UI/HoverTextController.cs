using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTextController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup hoverGroup;
    public float fadeSpeed = 6f;
    bool isHover = false;

    void Start()
    {
        hoverGroup.alpha = 0f; // 시작은 투명
    }

    void Update()
    {
        float target = isHover ? 1f : 0f;
        hoverGroup.alpha = Mathf.Lerp(hoverGroup.alpha, target, Time.deltaTime * fadeSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
    }
}
