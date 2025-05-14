using TMPro;
using UnityEngine;

public class ChangeTMPColor1 : MonoBehaviour
{
    public TextMeshPro tmpText;  // TextMeshPro 객체 참조

    public void ChangeColorRed()
    {
        tmpText.faceColor = Color.red;  // 빨간색으로 변경
    }

    public void ChangeColorWhite()
    {
        tmpText.faceColor = Color.white;  // 원래 색으로 변경
    }
}