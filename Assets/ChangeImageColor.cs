using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour
{
    public Image tmpImage;  // TextMeshPro 객체 참조
    private Color originalColor;

    public void ChangeColorRed()
    {
        originalColor = tmpImage.color;
        tmpImage.color = Color.red;  // 빨간색으로 변경
    }

    public void ChangeColorOriginal()
    {
        tmpImage.color = originalColor;  // 원래 색으로 변경
    }

    public void switchColor()
    {
        if (originalColor == Color.white)
        {
            originalColor = Color.green;
        }
        else {
            originalColor = Color.white;
        }
    }

}