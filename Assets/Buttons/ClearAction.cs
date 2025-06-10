using System.Collections.Generic;
using UnityEngine;

public class ClearAction : MonoBehaviour
{
    public void ClearObjectByName()
    {
        List<string > objectNames = new List<string>
        {
            "AirRequestButton(Clone)", "BedRequestButton(Clone)", "BookRequestButton(Clone)", "ChairRequestButton(Clone)", "ClockRequestButton(Clone)", "DoorRequestButton(Clone)", "FanRequestButton(Clone)",
            "LampRequestButton(Clone)", "LaptopRequestButton(Clone)", "MugRequestButton(Clone)", "ThermometerRequestButton(Clone)", "TissueRequestButton(Clone)", "WindowRequestButton(Clone)", "TVRequestButton(Clone)",
        };

        List<string> labelNames = new List<string>
        {
            "air conditioner", "bed", "book", "chair", "clock", "door", "fan", "lamp", "laptop", "mug", "thermometer", "tissue", "window", "tv"
        };
        List<GameObject> objectsToDelete = new List<GameObject>();
        foreach (string objectName in objectNames)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj != null)
            {
                objectsToDelete.Add(obj); // ������ ������Ʈ�� ����Ʈ�� �߰�
            }
            else
            {
                Debug.Log($"Object '{objectName}' not found.");
            }
        }
        foreach (GameObject obj in objectsToDelete)
        {
            string labelName = labelNames[objectNames.IndexOf(obj.name)];
            Destroy(obj); // ����Ʈ�� �ִ� ������Ʈ���� ����
            Detection.activeButtonInstances.Remove(labelName); // Detection Ŭ�������� Ȱ��ȭ�� ��ư �ν��Ͻ� ����
            Debug.Log($"Deleted object: {obj.name}");
        }
    }
}