using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageColor : MonoBehaviour
{
    public Image tmpImage;  // TextMeshPro ��ü ����
    private Color originalColor;

    public void ChangeColorRed()
    {
        originalColor = tmpImage.color;
        tmpImage.color = Color.red;  // ���������� ����
    }

    public void ChangeColorOriginal()
    {
        tmpImage.color = originalColor;  // ���� ������ ����
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