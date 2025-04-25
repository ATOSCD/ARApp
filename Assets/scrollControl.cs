using UnityEngine;
using UnityEngine.UI;

public class ChatScrollController : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollStep = 0.3f; // 한 번 클릭할 때 이동할 정도

    // 위로 스크롤
    public void ScrollUp()
    {
        scrollRect.verticalNormalizedPosition += scrollStep;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
    }

    // 아래로 스크롤
    public void ScrollDown()
    {
        scrollRect.verticalNormalizedPosition -= scrollStep;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
    }
}