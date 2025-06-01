using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public InputField idInputField;
    public InputField pwInputField;

    

    public void OnLoginButtonClicked()
    {
        string id = idInputField.text;
        string pw = pwInputField.text;

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(pw))
        {
            Debug.LogWarning("ID �Ǵ� ��й�ȣ�� ��� �ֽ��ϴ�.");
            return;
        }

        LoginRequest loginRequest = new LoginRequest
        {
            user_id = id,
            password = pw
        };
        string jsonRequest = JsonUtility.ToJson(loginRequest);
        Debug.Log("�α��� ��û: " + jsonRequest);

        // ������ �α��� ��û�� ������ �κ�
        StartCoroutine(SendLoginRequest(jsonRequest));



    }

    IEnumerator SendLoginRequest(string jsonRequest)
    {
        string request_url = "/login/";
        string url = $"http://" + SERVER.server + request_url;

        Debug.Log($"�α��� ��û URL: {url}");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text.Trim();
            Debug.Log("�α��� ����: " + responseText);

            if (int.TryParse(responseText, out int resultCode))
            {
                if (resultCode == 2)
                {
                    Debug.Log("�α��� ����");
                    PlayerPrefs.SetString("userId", idInputField.text);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("basis");
                    SERVER.user_id = idInputField.text; // ������ �α����� ����� ID ����
                }
                else
                {
                    Debug.LogWarning($"�α��� ���� - ���� ���� �ڵ�: {resultCode}");
                }
            }
            else
            {
                Debug.LogError("���� ������ ������ ��ȯ�� �� �����ϴ�: " + responseText);
            }
        }
        else
        {
            Debug.LogError($"���� ����: {request.responseCode} - {request.error}");
        }
    }




}

[Serializable]
public class LoginRequest {
    public string user_id;
    public string password;
}