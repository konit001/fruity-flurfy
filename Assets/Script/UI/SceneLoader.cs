using UnityEngine;
using UnityEngine.SceneManagement;

// ใช้กับปุ่ม UI (OnClick ส่งชื่อซีนเป็น string)
public class SceneLoader : MonoBehaviour
{
    public void Load(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
