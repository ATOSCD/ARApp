using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using TMPro;
using MixedReality.Toolkit;
using System.Collections.Generic;

public class Recommend : MonoBehaviour
{
    public static Recommend Instance { get; private set; } // �̱��� ����

    private NativeWebSocket.WebSocket websocket; // NativeWebSocket.WebSocket ����� ���

    public InputField textToSend;
    public TextMeshProUGUI Button1;
    public TextMeshProUGUI Button2;
    public TextMeshProUGUI Button3;
    public TextMeshProUGUI Button4;

    public string userId = SERVER.user_id;               // ���� ������� user_id

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ConnectToWebSocket();
    }

    private async void ConnectToWebSocket()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is not set. Cannot connect to WebSocket.");
            return;
        }

        string webSocketUrl = $"ws://{SERVER.server}/ws/chat-recommend";
        websocket = new WebSocket(webSocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connected to RecommendWebSocket server!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log($"Received: {message}");

            try
            {
                ChatRecommendMessage chatMessage = JsonUtility.FromJson<ChatRecommendMessage>(message);

                if (chatMessage != null)
                {
                    Debug.Log($"Parsed user_id: {chatMessage.user_id}");

                    if (chatMessage.text != null)
                    {
                        for (int i = 0; i < chatMessage.text.Count; i++)
                        {
                            Debug.Log($"Text[{i}]: {chatMessage.text[i]}");
                        }

                        Button1.text = chatMessage.text.Count > 0 ? chatMessage.text[0] : "";
                        Button2.text = chatMessage.text.Count > 1 ? chatMessage.text[1] : "";
                        Button3.text = chatMessage.text.Count > 2 ? chatMessage.text[2] : "";
                        Button4.text = chatMessage.text.Count > 3 ? chatMessage.text[3] : "";
                    }
                    else
                    {
                        Debug.Log("Parsed text is null");
                    }
                }
                else
                {
                    Debug.Log("Failed to parse ChatRecommendMessage");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON parsing error: {e.Message}");
            }
        };


        websocket.OnClose += (e) =>
        {
            Debug.Log("RecommendWebSocket connection closed.");
        };

        await websocket.Connect();
    }

    public async void SendMessageToServer()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            // 1. ��ü ����
            ChatRecommendRequest request = new ChatRecommendRequest
            {
                user_id = userId,
                text = textToSend.text
            };

            // 2. JSON���� ����ȭ
            string jsonString = JsonUtility.ToJson(request);

            // 3. �α� ��� (����)
            Debug.Log("Sending JSON: " + jsonString);

            // 4. WebSocket���� ����
            await websocket.SendText(jsonString);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open.");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }
}

[System.Serializable]
class ChatRecommendMessage
{
    public string user_id;
    public List<string> text;
}

[System.Serializable]
class ChatRecommendRequest
{
    public string user_id;
    public string text;
}