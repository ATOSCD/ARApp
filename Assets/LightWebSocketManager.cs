using UnityEngine;
using NativeWebSocket;

public class LightWebSocketManager : MonoBehaviour
{
    public static LightWebSocketManager Instance { get; private set; }

    private WebSocket websocket;
    private string userId = SERVER.user_id;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 필요 시 유지
    }

    private async void OnEnable()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[LightWS] User ID is not set. Cannot connect.");
            return;
        }

        if (websocket != null)
        {
            if (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.Connecting)
            {
                Debug.Log("[LightWS] Already connected or connecting.");
                return;
            }

            await websocket.Close();
            websocket = null;
        }

        string url = $"ws://{SERVER.server}/ws/iot-light";
        websocket = new WebSocket(url);

        websocket.OnOpen += () => Debug.Log("[LightWS] Connected to server.");
        websocket.OnClose += (e) => Debug.Log("[LightWS] Connection closed.");

        await websocket.Connect();
    }

    private async void OnDisable()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.Close();
            Debug.Log("[LightWS] Closed on disable.");
            websocket = null;
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    public async void SendIotMessage(string iotId, string message)
    {
        if (websocket == null || websocket.State != WebSocketState.Open)
        {
            Debug.LogWarning("[LightWS] Cannot send, WebSocket is not open.");
            return;
        }

        var msg = new LightIotMessage { iot_id = iotId, message = message };
        string json = JsonUtility.ToJson(msg);
        Debug.Log($"[LightWS] Sending: {json}");

        await websocket.SendText(json);
    }

    // Light 제어
    public void SendLightOn() => SendIotMessage("light", "on");
    public void SendLightOff() => SendIotMessage("light", "off");

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }
}

[System.Serializable]
public class LightIotMessage
{
    public string iot_id;
    public string message;
}
