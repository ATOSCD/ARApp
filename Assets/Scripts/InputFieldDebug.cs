using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

public class InputFieldDebugger : MonoBehaviour
{
    public MRTKUGUIInputField inputField;

    void Update()
    {
        if (inputField != null)
        {
            if (inputField.isFocused)
            {
                Debug.Log("InputField is focused");

                if (!string.IsNullOrEmpty(inputField.text))
                {
                    Debug.Log($"Current Text: {inputField.text}");
                }
                else
                {
                    Debug.Log("No input yet");
                }
            }
        }
    }
}
