using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AirConAPI : MonoBehaviour
{
    private string base_url = SERVER.server;
    public string user_id = SERVER.user_id;

    void Start()
    {
        StartCoroutine(GetRequests(user_id));
    }

    public void GetRequestNames()
    {
        StartCoroutine(GetRequests(user_id));
    }
    public TextMeshPro[] textMeshProObjects;

    IEnumerator GetRequests(string user_id)
    {
        string request_url = $"/get-button-by-category/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        // JSON 데이터 생성
        RequestData requestBody = new RequestData
        {
            user_id = user_id,
            category = "에어컨"
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

            // JSON 응답 파싱
            string wrappedJson = "{\"items\":" + jsonResponse + "}";
            ButtonDataList dataList = JsonUtility.FromJson<ButtonDataList>(wrappedJson);

            for (int i = 0; i < dataList.items.Length && i < textMeshProObjects.Length; i++)
            {
                textMeshProObjects[i].text = dataList.items[i].button_text;
                Debug.Log($"Assigned '{dataList.items[i].button_text}' to TextMeshPro object at index {i}");
            }
        }
    }

    public void SendRequest(TextMeshPro input)
    {
        StartCoroutine(SendRequests(user_id, input));
    }

    IEnumerator SendRequests(string user_id, TextMeshPro input)
    {
        string request_url = $"/send-notification/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        // 버튼 ID를 TextMeshPro 객체의 텍스트로 설정
        string notificationBody = input.text;
        // JSON 데이터 생성
        RequestNotificationData requestBody = new RequestNotificationData
        {
            user_id = user_id,
            title = "환자의 호출이 있습니다.",
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

public class RequestData
{
    public string user_id;
    public string category;
}

public class RequestNotificationData
{
    public string user_id;
    public string title;
    public string body;
}

[System.Serializable]
public class ButtonData
{
    public int button_id;
    public string category;
    public string button_text;
    public string user_id;
}

[System.Serializable]
public class ButtonDataList
{
    public ButtonData[] items;
}