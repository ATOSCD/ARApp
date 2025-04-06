using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHoverColorChange : MonoBehaviour
{
    public RawImage backplateImage;
    public Color hoverColor = new Color(1f, 0f, 0f, 0.2f); // ���� ������
    public Color originalColor;

    void Awake()
    {
        if (backplateImage != null)
        {
            // ���İ� �����ؼ� ����
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
            visibleColor.a = 0.2f;  // �Ǵ� 0.2f �� ���ϴ� ��
            backplateImage.color = visibleColor;

            Debug.Log($"Reverting to color: {visibleColor}");
        }
    }

}