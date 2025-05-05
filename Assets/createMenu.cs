using UnityEngine;

public class SpawnUniqueButtons : MonoBehaviour
{
    public GameObject buttonBarPrefab; // 다른 디자인을 가진 버튼 프리팹 4개
    public Transform spawnParent; // 버튼을 배치할 부모 오브젝트
    public Vector3 spawnOffset = new Vector3(0.2f, 0, 0); // 생성 위치 간격
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
                Debug.Log("첫 번째 버튼: 색 변경");
                GetComponent<Renderer>().material.color = Color.blue;
                break;
            case 1:
                Debug.Log("두 번째 버튼: 크기 변경");
                transform.localScale *= 1.5f;
                break;
            case 2:
                Debug.Log("세 번째 버튼: 회전");
                transform.Rotate(0, 45, 0);
                break;
            case 3:
                Debug.Log("네 번째 버튼: 메시지 출력");
                Debug.Log("네 번째 버튼이 눌렸습니다!");
                break;
        }
    }
}