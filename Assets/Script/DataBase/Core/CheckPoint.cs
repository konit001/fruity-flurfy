using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    public void save()
    {
        // if (!GameSession.IsLoggedIn)
        // {
        //     Debug.LogWarning("CheckPoint: ยังไม่ได้ login");
        //     return;
        // }

        // var playerObj = GameObject.FindGameObjectWithTag("Player");
        // if (playerObj == null)
        // {
        //     Debug.LogWarning("CheckPoint: หา Player ในฉากไม่เจอ");
        //     return;
        // }

        // var db = DbProvider.Connection;

        // var playerRepo = new PlayerRepository(db);
        // var saveRepo = new SaveRepository(db);
        // var materialsRepo = new MaterialsRepository(db);
        // var inventoryRepo = new InventoryRepository(db);

        // int playerId = GameSession.PlayerId;
        // var player = playerRepo.GetById(playerId);
        // if (player == null)
        // {
        //     Debug.LogWarning("CheckPoint: ไม่พบ Player ใน DB");
        //     return;
        // }

        // // ขึ้นวันใหม่ตอน save (แบบ Stardew Valley sleep)
        // if (CalendarManager.instance != null)
        //     CalendarManager.instance.NextDay();
        // int day = CalendarManager.instance != null ? CalendarManager.instance.dayCount : player.Day + 1;

        // // Player: sync Day/Gold ปัจจุบัน
        // playerRepo.Update(playerId, player.Gold, day);

        // // SaveData: scene + ตำแหน่งผู้เล่นปัจจุบัน
        // var pos = playerObj.transform.position;
        // string sceneName = SceneManager.GetActiveScene().name;
        // saveRepo.Save(playerId, sceneName, day, player.Gold, pos.x, pos.y, pos.z);

        // // Inventory: เขียนทับทั้งชุดจาก InventoryManager ปัจจุบัน
        // var entries = new List<(int MaterialId, int Quantity)>();
        // foreach (var slot in InventoryManager.Instance.currentItem)
        // {
        //     if (slot.IsEmpty) continue;

        //     var material = materialsRepo.GetOrCreate(slot.data._name, slot.data.price);
        //     entries.Add((material.Id, slot.amount));
        // }
        // inventoryRepo.ReplaceAll(playerId, entries);

        // Debug.Log($"CheckPoint: saved. Day {day}, Gold {player.Gold}, Scene {sceneName}, Pos {pos}");
    }
}
