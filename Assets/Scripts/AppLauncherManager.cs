using UnityEngine;
using UnityEngine.SceneManagement;

public class AppLauncherManager : MonoBehaviour
{
    void Start()
    {
        // ��: �ڵ� �α��� ������ �ִ� ���
        if (PlayerPrefs.HasKey("userId"))
        {
            SceneManager.LoadScene("basis");
        }
        else
        {
            SceneManager.LoadScene("login");
        }
    }
}
