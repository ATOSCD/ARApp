using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GazeMove : MonoBehaviour
{
    public float moveSpeed = 2f; // ���� �ö󰡴� �ӵ�
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

    // Gaze ���� �� ȣ��
    public void OnGazeEnter()
    {
        isGazedAt = true;
    }

    // Gaze ���� �� ȣ��
    public void OnGazeExit()
    {
        isGazedAt = false;
        transform.position = initialPosition; // ���� ��ġ�� ����
    }
}
