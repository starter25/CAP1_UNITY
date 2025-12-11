using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CaptainHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("내 이미지")]
    public Image myImage;
    public Color myNormalColor = Color.white;                  // 정상 컬러
    public Color myHighlightColor = Color.white;               // 강조 컬러 (그대로 white 가능)

    [Header("반대쪽 이미지")]
    public Image otherImage;
    public Color otherNormalColor = Color.white;               // 정상 컬러
    public Color otherGrayColor = new Color(0.5f, 0.5f, 0.5f); // 흐린 흑백 느낌

    void Reset()
    {
        myImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 내가 Hover된 경우 → 나 = 컬러 / 상대 = 흑백
        if (myImage != null)
            myImage.color = myHighlightColor;

        if (otherImage != null)
            otherImage.color = otherGrayColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 기본 상태로 복귀
        if (myImage != null)
            myImage.color = myNormalColor;

        if (otherImage != null)
            otherImage.color = otherNormalColor;
    }
}
