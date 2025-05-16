using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class ActivateByName : MonoBehaviour
{
    public List<GameObject> allObjects;
    public string user_id = SERVER.user_id;
    private string base_url = SERVER.server;

    // API�� ���� Ȱ��ȭ�� ������Ʈ �̸� ����Ʈ
    private List<string> namesFromAPI;

    private Coroutine autoRefreshCoroutine;

    void OnEnable()
    {
        if (autoRefreshCoroutine == null)
        {
            StartCoroutine(GetActiveRequests(user_id)); // ���� ��û
            autoRefreshCoroutine = StartCoroutine(AutoRefreshRoutine()); // �ߺ� ���� ����
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

    // ȣ�� �� �̸��� ��ġ�ϴ� ������Ʈ�� Ȱ��ȭ
    IEnumerator GetActiveRequests(string user_id)
    {
        string request_url = $"/get-selected-category-ar/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        // JSON ������ ����
        RequestData requestBody = new RequestData
        {
            user_id = user_id
        };
        string jsonData = JsonUtility.ToJson(requestBody);

        // UnityWebRequest ����
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

            // JSON �迭�� ������ ��ü ���·� ���α�
            string rawJson = request.downloadHandler.text;
            string wrappedJson = "{\"items\":" + rawJson + "}";

            StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(wrappedJson);
            namesFromAPI = wrapper.items;

            // ������Ʈ Ȱ��ȭ/��Ȱ��ȭ ó��
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
                // �̸��� ����Ʈ�� ������ Ȱ��ȭ, ������ ��Ȱ��ȭ
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