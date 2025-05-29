using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecommendUsed : MonoBehaviour
{
    public InputField inputField;
    public TextMeshProUGUI text;

    public void setValue()
    {
        inputField.text = text.text;
    }
}
