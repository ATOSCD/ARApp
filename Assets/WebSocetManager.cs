using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance { get; private set; } // �̱��� ����

    private NativeWebSocket.WebSocket websocket; // NativeWebSocket.WebSocket ����� ���

    public GameObject chatContentObject; // ���� GameObject�� ����
    private Text chatContentText;        // GameObject�� Text ������Ʈ ����
    public string userId;               // ���� ������� user_id

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
        // GameObject���� Text ������Ʈ ��������
        if (chatContentObject != null)
        {
            chatContentText = chatContentObject.GetComponent<Text>();
            if (chatContentText == null)
            {
                Debug.LogError("The assigned GameObject does not have a Text component.");
            }
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

        string webSocketUrl = $"ws://192.168.1.45:8000/ws/chat?user_id={userId}";
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

    public async void SendMessage(string message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(message);
        }
    }

    private void UpdateChatWindow(string message)
    {
        // ���� GameObject�� Text UI�� �޽��� �߰�
        if (chatContentText != null)
        {
            chatContentText.text += $"{message}\n";
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
}