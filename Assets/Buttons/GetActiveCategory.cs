using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class ActivateByName : MonoBehaviour
{
    public List<GameObject> allObjects;
    public string user_id = SERVER.user_id;
    private string base_url = SERVER.server;

    // API로 받은 활성화할 오브젝트 이름 리스트
    private List<string> namesFromAPI;

    private Coroutine autoRefreshCoroutine;

    void OnEnable()
    {
        if (autoRefreshCoroutine == null)
        {
            StartCoroutine(GetActiveRequests(user_id)); // 최초 요청
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
            yield return StartCoroutine(GetActiveRequests(user_id));
        }
    }

    // 호출 시 이름이 일치하는 오브젝트만 활성화
    IEnumerator GetActiveRequests(string user_id)
    {
        string request_url = $"/get-selected-category-ar/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        // JSON 데이터 생성
        RequestData requestBody = new RequestData
        {
            user_id = user_id
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
            Debug.Log($"Response: {request.downloadHandler.text}");

            // JSON 배열을 강제로 객체 형태로 감싸기
            string rawJson = request.downloadHandler.text;
            string wrappedJson = "{\"items\":" + rawJson + "}";

            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(wrappedJson);
            namesFromAPI = wrapper.items;

            // 오브젝트 활성화/비활성화 처리
            foreach (GameObject obj in allObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(namesFromAPI.Contains(obj.name));
                }
            }
        }


        foreach (GameObject obj in allObjects)
        {
            if (obj != null)
            {
                // 이름이 리스트에 있으면 활성화, 없으면 비활성화
                if (namesFromAPI.Contains(obj.name))
                {
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class StringListWrapper
{
    public List<string> items;
}