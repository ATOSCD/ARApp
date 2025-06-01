using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class RequestSend : MonoBehaviour
{
    private string base_url = SERVER.server;
    private string user_id = SERVER.user_id;
    public string category;

    public void SendRequest(TextMeshPro input)
    {
        StartCoroutine(SendRequests(user_id, input));
    }

    IEnumerator SendRequests(string user_id, TextMeshPro input)
    {
        string request_url = $"/send-notification/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        string notificationTitle = category + " 관련 환자의 호출이 있습니다.";

        // 버튼 ID를 TextMeshPro 객체의 텍스트로 설정
        string notificationBody = input.text;
        // JSON 데이터 생성
        RequestNotificationData requestBody = new RequestNotificationData
        {
            user_id = user_id,
            title = notificationTitle,
            body = notificationBody,
            category = category

        };
        string jsonData = JsonUtility.ToJson(requestBody);
        // UnityWebRequest 생성
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"ERROR: {request.error}");
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Response: {jsonResponse}");
        }
    }
}

public class RequestNotificationData
{
    public string user_id;
    public string title;
    public string body;
    public string category;
}