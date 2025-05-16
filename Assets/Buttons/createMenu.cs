using UnityEngine;

public class SpawnUniqueButtons : MonoBehaviour
{
    public GameObject buttonBarPrefab; // �ٸ� �������� ���� ��ư ������ 4��
    public Transform spawnParent; // ��ư�� ��ġ�� �θ� ������Ʈ
    public Vector3 spawnOffset = new Vector3(0.2f, 0, 0); // ���� ��ġ ����
    private GameObject newButton;

    public void SpawnNewButtons()
    {
        if (buttonBarPrefab == null) return;
        if ( newButton == null)
        {
            Vector3 spawnPosition = transform.position + spawnOffset;
            newButton = Instantiate(buttonBarPrefab, spawnPosition, Quaternion.identity, spawnParent);
            newButton.SetActive(true);
        }
        else
        {
            if (newButton != null)
            {
                newButton.SetActive(true);
            }
        }
    }

    public void deleteButton()
    {
        newButton.SetActive(false);
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