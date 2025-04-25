using UnityEngine;
using UnityEngine.UI;

public class ChatScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollStep = 0.3f; // �� �� Ŭ���� �� �̵��� ����

    // ���� ��ũ��
    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition += scrollStep;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
    }

    // �Ʒ��� ��ũ��
    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition -= scrollStep;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
    }
}