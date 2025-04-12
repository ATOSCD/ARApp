using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance { get; private set; } // 싱글톤 패턴

    private NativeWebSocket.WebSocket websocket; // NativeWebSocket.WebSocket 명시적 사용

    public GameObject chatContentObject; // 기존 GameObject를 참조
    private Text chatContentText;        // GameObject의 Text 컴포넌트 참조
    public string userId;               // 현재 사용자의 user_id

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ConnectToWebSocket();
        // GameObject에서 Text 컴포넌트 가져오기
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
    //    this.userId = userId; // 외부에서 user_id를 설정
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
            // 서버로부터 메시지 수신
            string message = Encoding.UTF8.GetString(bytes);
            Debug.Log($"Received: {message}");

            // 채팅창 업데이트
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
        // 기존 GameObject의 Text UI에 메시지 추가
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