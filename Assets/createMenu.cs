using UnityEngine;

public class SpawnUniqueButtons : MonoBehaviour
{
    public GameObject[] buttonPrefabs; // �ٸ� �������� ���� ��ư ������ 4��
    public Transform spawnParent; // ��ư�� ��ġ�� �θ� ������Ʈ
    public Vector3 spawnOffset = new Vector3(0.2f, 0, 0); // ���� ��ġ ����

    public void SpawnNewButtons()
    {
        for (int i = 0; i < buttonPrefabs.Length; i++)
        {
            if (buttonPrefabs[i] == null) continue;

            Vector3 spawnPosition = transform.position + spawnOffset * (i + 1);
            GameObject newButton = Instantiate(buttonPrefabs[i], spawnPosition, Quaternion.identity, spawnParent);

            // ��ư�� ������ ������ �����ϴ� ��ũ��Ʈ �߰�
            UniqueButtonBehavior buttonBehavior = newButton.AddComponent<UniqueButtonBehavior>();
            buttonBehavior.SetButtonRole(i);
        }
    }
}

public class UniqueButtonBehavior : MonoBehaviour
{
    private int buttonIndex;

    public void SetButtonRole(int index)
    {
        buttonIndex = index;
    }

    public void OnButtonPressed()
    {
        switch (buttonIndex)
        {
            case 0:
                Debug.Log("ù ��° ��ư: �� ����");
                GetComponent<Renderer>().material.color = Color.blue;
                break;
            case 1:
                Debug.Log("�� ��° ��ư: ũ�� ����");
                transform.localScale *= 1.5f;
                break;
            case 2:
                Debug.Log("�� ��° ��ư: ȸ��");
                transform.Rotate(0, 45, 0);
                break;
            case 3:
                Debug.Log("�� ��° ��ư: �޽��� ���");
                Debug.Log("�� ��° ��ư�� ���Ƚ��ϴ�!");
                break;
        }
    }
}