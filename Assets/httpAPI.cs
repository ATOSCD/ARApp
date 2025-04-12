using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Unity.VisualScripting;

public class http : MonoBehaviour
{
    string base_url = "https://192.168.1.45:8000";

    public void GetAllMessages(string user_id)
    {
        StartCoroutine(GetMessages(user_id));
    }

    IEnumerator GetMessages(string user_id)
    {
        string request_url = $"/get-messages/{user_id}";
        string url = base_url + request_url;

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"ERROR: {request.error}");
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"Response: {jsonResponse}");

            Message[] messages = JsonHelper.FromJson<Message>(jsonResponse);

            foreach (Message message in messages)
            {
                Debug.Log($"ID: {message.id}, UserID: {message.user_id}, Content: {message.content}, Created At: {message.created_at}, Is Read: {message.is_read}\n");
            }
        }
    }
}


[Serializable]
public class Message
{
    public int id;
    public string user_id;
    public string content;
    public string created_at;
    public bool is_read;

}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = $"{{\"items\":{json}}}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson) ;
        return wrapper.items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}