using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Emergency : MonoBehaviour
{
    private string base_url = SERVER.server;
    public string user_id = SERVER.user_id;

    public void SendEmergencyRequest()
    {
        StartCoroutine(SendEmergencyNotification(user_id));
    }

    public IEnumerator SendEmergencyNotification(string user_id)
    {
        string request_url = $"/send-emergency-notification/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        string notificationTitle = "⚠️⚠️⚠️";
        string notificationBody = "환자의 긴급 호출입니다.";
        // JSON 데이터 생성  
        RequestNotificationData requestBody = new RequestNotificationData
        {
            user_id = user_id,
            title = notificationTitle,
            body = notificationBody
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
