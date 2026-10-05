using UnityEngine;

// วางไว้ในซีนเกม — โหลดทอง / ของที่ซื้อ / buff จากเซฟตอนเข้าซีน (ไม่ login ก็ไม่ทำอะไร)
public class GameLoader : MonoBehaviour
{
    void Start()
    {
        GameSaver.LoadRun();
    }
}
