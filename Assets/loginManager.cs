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
            Debug.LogWarning("ID 또는 비밀번호가 비어 있습니다.");
            return;
        }

        LoginRequest loginRequest = new LoginRequest
        {
            user_id = id,
            password = pw
        };
        string jsonRequest = JsonUtility.ToJson(loginRequest);
        Debug.Log("로그인 요청: " + jsonRequest);

        // 서버에 로그인 요청을 보내는 부분
        StartCoroutine(SendLoginRequest(jsonRequest));



    }

    IEnumerator SendLoginRequest(string jsonRequest)
    {
        string request_url = "/login/";
        string url = $"http://" + SERVER.server + request_url;

        Debug.Log($"로그인 요청 URL: {url}");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text.Trim();
            Debug.Log("로그인 응답: " + responseText);

            if (int.TryParse(responseText, out int resultCode))
            {
                if (resultCode == 2)
                {
                    Debug.Log("로그인 성공");
                    PlayerPrefs.SetString("userId", idInputField.text);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("basis");
                    SERVER.user_id = idInputField.text; // 서버에 로그인한 사용자 ID 저장
                }
                else
                {
                    Debug.LogWarning($"로그인 실패 - 서버 응답 코드: {resultCode}");
                }
            }
            else
            {
                Debug.LogError("서버 응답을 정수로 변환할 수 없습니다: " + responseText);
            }
        }
        else
        {
            Debug.LogError($"서버 오류: {request.responseCode} - {request.error}");
        }
    }




}

[Serializable]
public class LoginRequest {
    public string user_id;
    public string password;
}