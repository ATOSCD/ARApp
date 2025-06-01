using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using TMPro;
using MixedReality.Toolkit;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance { get; private set; } // 싱글톤 패턴

    private NativeWebSocket.WebSocket websocket; // NativeWebSocket.WebSocket 명시적 사용

    public GameObject chatContentObject; // 기존 GameObject를 참조
    public InputField textToSend;
    private TextMeshProUGUI chatContentText;        // GameObject의 Text 컴포넌트 참조
    private string userId = SERVER.user_id;               // 현재 사용자의 user_id

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
        Debug.Log($"userId = {userId}");
        // GameObject에서 Text 컴포넌트 가져오기
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
            //rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // 스케일 초기화
            rectTransform.sizeDelta = new Vector2(280, 0); // 크기 조정
            //rectTransform.anchoredPosition = new Vector2(0, 0.4f); // 위치 조정
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

        string webSocketUrl = $"ws://{SERVER.server}/ws/chat?user_id={userId}";
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
        // 기존 GameObject의 Text UI에 메시지 추가
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
}

[System.Serializable]
class ChatMessage
{
    public string user_id;
    public string user_name;
    public string message;
}