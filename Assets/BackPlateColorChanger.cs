using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHoverColorChange : MonoBehaviour
{
    public RawImage backplateImage;
    public Color hoverColor = new Color(1f, 0f, 0f, 0.2f); // 투명 빨간색
    public Color originalColor;

    void Awake()
    {
        if (backplateImage != null)
        {
            // 알파값 포함해서 저장
            originalColor = backplateImage.color;
            Debug.Log($"Original color is {originalColor}");
        }
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (backplateImage != null)
        {
            backplateImage.color = hoverColor;
            Debug.Log($"Reverting to color: {hoverColor}");
        }
    }

    public void OnHoverExited(HoverExitEventArgs args)
    {
        if (backplateImage != null)
        {
            Color visibleColor = originalColor;
            visibleColor.a = 0.2f;  // 또는 0.2f 등 원하는 값
            backplateImage.color = visibleColor;

            Debug.Log($"Reverting to color: {visibleColor}");
        }
    }

}