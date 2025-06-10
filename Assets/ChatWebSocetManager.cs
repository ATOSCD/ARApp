using System;
using System.Text;
using MixedReality.Toolkit;
using NativeWebSocket;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;


public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance { get; private set; } // �̱��� ����

    private NativeWebSocket.WebSocket websocket; // NativeWebSocket.WebSocket ����� ���

    public GameObject chatContentObject; // ���� GameObject�� ����
    public InputField textToSend;
    private TextMeshProUGUI chatContentText;        // GameObject�� Text ������Ʈ ����
    private string userId = SERVER.user_id;               // ���� ������� user_id

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
        Debug.Log($"userId = {userId}");
        // GameObject���� Text ������Ʈ ��������
        if (chatContentObject != null)
        {
            GameObject textObject = new GameObject("ChatContentText");
            textObject.transform.SetParent(chatContentObject.transform,false);
            Debug.Log($"textObject = {textObject}");

            chatContentText = textObject.AddComponent<TextMeshProUGUI>();

            chatContentText.fontSize = 10;
            chatContentText.alignment = TextAlignmentOptions.TopLeft;
            chatContentText.text = "";
            chatContentText.color = Color.white;
            chatContentText.font = Resources.Load<TMP_FontAsset>("malgun TMP");

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            Debug.Log($"rectTransform = {rectTransform}");
            
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.localPosition = new Vector3(10, -110, -0.01f);
            //rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // ������ �ʱ�ȭ
            rectTransform.sizeDelta = new Vector2(280, 0); // ũ�� ����
            //rectTransform.anchoredPosition = new Vector2(0, 0.4f); // ��ġ ����
            rectTransform.ForceUpdateRectTransforms();


            ConnectToWebSocket();
        }
        else
        {
            Debug.LogError("ChatContentObject is not assigned.");
        }

    }

    //public void Initialize(string userId)
    //{
    //    this.userId = userId; // �ܺο��� user_id�� ����
    //    ConnectToWebSocket();
    //}

    private async void ConnectToWebSocket()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is not set. Cannot connect to WebSocket.");
            return;
        }

        string webSocketUrl = $"ws://{SERVER.server}/ws/chat?user_id={userId}";
        websocket = new NativeWebSocket.WebSocket(webSocketUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connected to WebSocket server!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // �����κ��� �޽��� ����
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log($"Received: {message}");

            // ä��â ������Ʈ
            UpdateChatWindow(message);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket connection closed.");
        };

        await websocket.Connect();
    }

    public async void SendMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(textToSend.text);
        }

        textToSend.text = "";
    }

    private void UpdateChatWindow(string message)
    {
        ChatMessage chat = JsonUtility.FromJson<ChatMessage>(message);

        string formattedMessage = $"<b>{chat.user_name}</b> : {chat.message}";
        // ���� GameObject�� Text UI�� �޽��� �߰�
        if (chatContentText != null)
        {
            chatContentText.text += $"{formattedMessage}\n";
        }
        else
        {
            Debug.LogError("ChatContentText is not assigned or missing.");
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


    private string base_url = SERVER.server;

    public void SendChatNotification()
    {
        StartCoroutine(SendRequests(userId, textToSend.text));
    }

    IEnumerator SendRequests(string user_id, string input)
    {
        string request_url = $"/send-chat-notification/";
        string url = $"http://" + base_url + request_url;
        Debug.Log($"URL: {url}");

        string notificationTitle = "ȯ���� ä��";

        // ��ư ID�� TextMeshPro ��ü�� �ؽ�Ʈ�� ����
        string notificationBody = input;
        // JSON ������ ����
        RequestNotificationData requestBody = new RequestNotificationData
        {
            user_id = user_id,
            title = notificationTitle,
            body = notificationBody,

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

[System.Serializable]
class ChatMessage
{
    public string user_id;
    public string user_name;
    public string message;
}

public class ChatNotificationData
{
    public string user_id;
    public string title;
    public string body;
}