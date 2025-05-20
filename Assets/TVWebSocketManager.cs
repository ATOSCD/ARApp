using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using NativeWebSocket;
using TMPro;
using MixedReality.Toolkit;

public class TVWebSocketManager : MonoBehaviour
{
    //public static TVWebSocketManager Instance { get; private set; } // 싱글톤 패턴

    private WebSocket websocket; // NativeWebSocket.WebSocket 명시적 사용
    public string userId = SERVER.user_id;               // 현재 사용자의 user_id
    //public string iotId;

    private async void OnEnable()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is not set. Cannot connect to WebSocket.");
            return;
        }

        if (websocket != null)
        {
            if (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.Connecting)
            {
                Debug.Log("WebSocket already connected or connecting.");
                return;
            }

            await websocket.Close();
            websocket = null;
        }

        string webSocketUrl = $"ws://{SERVER.server}/ws/iot";
        websocket = new WebSocket(webSocketUrl);

        websocket.OnOpen += () => Debug.Log($"{gameObject.name} connected to TVWebSocket server!");
        websocket.OnClose += (e) => Debug.Log($"{gameObject.name} WebSocket closed.");

        await websocket.Connect();
    }

    public async void SendTVRequestMessage(string iotId, string message)
    {
        if (websocket == null || websocket.State != WebSocketState.Open)
        {
            Debug.LogWarning("WebSocket is not connected.");
            return;
        }

        IotMessage msg = new IotMessage
        {
            iot_id = iotId,
            message = message
        };

        string json = JsonUtility.ToJson(msg);
        Debug.Log($"Sending: {json}");

        await websocket.SendText(json);
    }

    public void SendTVPower()
    {
        SendTVRequestMessage("tv", "power");
    }

    public void SendTVVolumeUp()
    {
        SendTVRequestMessage("tv", "volup");
    }

    public void SendTVVolumeDown()
    {
        SendTVRequestMessage("tv", "voldown");
    }

    public void SendTVChannelUp()
    {
        SendTVRequestMessage("tv", "chup");
    }

    public void SendTVChannelDown()
    {
        SendTVRequestMessage("tv", "chdown");
    }

    public void SendLightOn()
    {
        SendTVRequestMessage("light", "on");
    }

    public void SendLightOff()
    {
        SendTVRequestMessage("light", "off");
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    private async void OnDisable()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.Close();
            Debug.Log("WebSocket closed on disable.");
            websocket = null;
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
class IotMessage
{
    public string iot_id;
    public string message;
}