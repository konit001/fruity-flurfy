using UnityEngine;
using UnityEngine.SceneManagement;

// วางไว้ในฉากเกม (Hub/Test) — โหลดตำแหน่งกลับมาจาก save ตอนเข้าฉาก
public class GameLoader : MonoBehaviour
{
    void Start()
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;

        var saveRepo = new SaveRepository(db);

        int playerId = GameSession.PlayerId;
        var save = saveRepo.Load(playerId);

        // ตำแหน่งผู้เล่น: apply เฉพาะตอนกลับมา scene เดียวกับที่เซฟไว้
        if (save != null && save.SceneName == SceneManager.GetActiveScene().name)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerObj.transform.position = new Vector3(save.PosX, save.PosY, save.PosZ);
        }
    }
}
