using TMPro;
using UnityEngine;

public class ChangeTMPColor1 : MonoBehaviour
{
    public TextMeshPro tmpText;  // TextMeshPro ��ü ����

    public void ChangeColorRed()
    {
        tmpText.faceColor = Color.red;  // ���������� ����
    }

    public void ChangeColorWhite()
    {
        tmpText.faceColor = Color.white;  // ���� ������ ����
    }
}