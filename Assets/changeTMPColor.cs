using TMPro;
using UnityEngine;

public class ChangeTMPColor : MonoBehaviour
{
    public TextMeshProUGUI tmpText;  // TextMeshPro 객체 참조

    public void ChangeColorRed()
    {
        tmpText.faceColor = Color.red;  // 빨간색으로 변경
    }

    public void ChangeColorWhite()
    {
        tmpText.faceColor = Color.white;  // 원래 색으로 변경
    }
}