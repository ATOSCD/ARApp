using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

public class GazeButtonColorChange : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI buttonIcon;
    public PressableButton button;
    public Color defaultColor = Color.white;
    public Color gazeColor = Color.red;

    void Update()
    {
        if (IsUserLookingAtButton())
        {
            buttonText.color = gazeColor;
            buttonIcon.color = gazeColor;
        }
        else
        {
            buttonText.color = defaultColor;
            buttonIcon.color = defaultColor;
        }
    }

    bool IsUserLookingAtButton()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == button.gameObject; // 버튼을 바라볼 때만 true 반환

        }

        return false;
    }
}