using TMPro;
using UnityEngine;

public class ChangeTextColor : MonoBehaviour
{
    public TextMeshProUGUI tmpText;  // TextMeshPro ��ü ����

    public void ChangeColorRed()
    {
        tmpText.faceColor = Color.red;  // ���������� ����
    }

    public void ChangeColorWhite()
    {
        tmpText.faceColor = Color.white;  // ���� ������ ����
    }
}