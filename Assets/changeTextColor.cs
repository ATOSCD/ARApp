using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTextColor : MonoBehaviour
{
    public Text tmpText;  // TextMeshPro ��ü ����

    public void ChangeColorRed()
    {
        tmpText.color = Color.red;  // ���������� ����
    }

    public void ChangeColorWhite()
    {
        tmpText.color = Color.white;  // ���� ������ ����
    }
}