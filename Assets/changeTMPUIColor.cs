using TMPro;
using UnityEngine;

public class ChangeTMPColor : MonoBehaviour
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