using UnityEngine;
using UnityEngine.SceneManagement;

// วางพรีแฟบ Portal ในซีนไหนก็ได้ แล้วลาก SceneList ที่ต้องการสุ่มใส่ช่อง sceneList
[RequireComponent(typeof(Collider))]
public class portal : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private SceneList sceneList;
    [Tooltip("ไม่สุ่มซ้ำกับซีนที่ Portal เพิ่งพาไปล่าสุด (เมื่อมีตัวเลือกมากกว่า 1)")]
    [SerializeField] private bool avoidRepeat = true;

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";
    [Tooltip("true = ต้องกดโต้ตอบขณะอยู่ในพอร์ทัล, false = เข้าแล้วเปลี่ยนซีนทันที")]
    [SerializeField] private bool requireInteract;

    private static string _lastScene;

    private bool _playerInside;
    private bool _loading;

    private void Update()
    {
        if (requireInteract && _playerInside && userInput.instance != null && userInput.instance.interact)
            Enter();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInside = true;
        if (!requireInteract) Enter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag)) _playerInside = false;
    }

    public void Enter()
    {
        if (_loading) return;
        if (sceneList == null)
        {
            Debug.LogWarning($"[portal] {name}: ยังไม่ได้ตั้ง sceneList", this);
            return;
        }

        string scene = sceneList.Pick(avoidRepeat ? _lastScene : null);
        if (scene == null)
        {
            Debug.LogWarning($"[portal] {name}: ไม่มีซีนปลายทางที่โหลดได้ใน {sceneList.name}", this);
            return;
        }

        _loading = true;
        _lastScene = scene;
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }
}
