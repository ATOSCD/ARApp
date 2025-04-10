using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTextColor : MonoBehaviour
{
    public Text tmpText;  // TextMeshPro 객체 참조

    public void ChangeColorRed()
    {
        tmpText.color = Color.red;  // 빨간색으로 변경
    }

    public void ChangeColorWhite()
    {
        tmpText.color = Color.white;  // 원래 색으로 변경
    }
}