using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class RequestAPI : MonoBehaviour
{
    private string base_url = SERVER.server;
    private string user_id = SERVER.user_id;
    public string category;

    private Coroutine autoRefreshCoroutine;

    void OnEnable()
    {
        if (autoRefreshCoroutine == null)
        {
            StartCoroutine(GetRequests(user_id, category)); // 최초 요청
            autoRefreshCoroutine = StartCoroutine(AutoRefreshRoutine()); // 중복 실행 방지
        }
    }

    void OnDisable()
    {
        if (autoRefreshCoroutine != null)
        {
            StopCoroutine(autoRefreshCoroutine);
            autoRefreshCoroutine = null;
        }
    }

    IEnumerator AutoRefreshRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            Debug.Log("Fetching data for user_id = " + user_id);
            yield return StartCoroutine(GetRequests(user_id, category));
        }
    }

    public void GetRequestNames()
    {
        StartCoroutine(GetRequests(user_id, category));
    }
    public TextMeshPro[] textMeshProObjects;

    IEnumerator GetRequests(string user_id, string category)
    {
        string request_url = $"/get-button-by-category/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        // JSON 데이터 생성
        RequestData requestBody = new RequestData
        {
            user_id = user_id,
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
        Debug.Log($"Request sent: {jsonData}");

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
}

public class RequestData
{
    public string user_id;
    public string category;
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