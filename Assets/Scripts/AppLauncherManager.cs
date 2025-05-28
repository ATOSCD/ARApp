using UnityEngine;
using UnityEngine.SceneManagement;

public class AppLauncherManager : MonoBehaviour
{
    void Start()
    {
        // 예: 자동 로그인 정보가 있는 경우
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
