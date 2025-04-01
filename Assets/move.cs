using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeMove : MonoBehaviour
{
    public float moveSpeed = 2f; // 위로 올라가는 속도
    private bool isGazedAt = false;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isGazedAt)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
    }

    // Gaze 시작 시 호출
    public void OnGazeEnter()
    {
        isGazedAt = true;
    }

    // Gaze 끝날 때 호출
    public void OnGazeExit()
    {
        isGazedAt = false;
        transform.position = initialPosition; // 원래 위치로 리셋
    }
}
