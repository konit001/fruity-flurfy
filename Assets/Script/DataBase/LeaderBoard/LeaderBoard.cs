using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text waveText;
    public GameObject page;

    DatabaseReference dbRef;
    void Awake()
    {
        if (page != null) page.SetActive(false);
    }
    void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        LoadScores();
    }

    public void OnOpen()
    {
        page.SetActive(true);
        LoadScores();
    }

    public void OnClose()
    {
        page.SetActive(false);
    }

    // ส่ง wave ของผู้เล่นขึ้น leaderboard
    public static void SubmitWave(string playerName, int wave)
    {
        // ชื่อเดิมจะเขียนทับ node เดิมเสมอ 
        var playerRef = FirebaseDatabase.DefaultInstance.RootReference
            .Child("leaderboard").Child(playerName);

        // RunTransaction = อ่านค่าเดิมแล้วเขียนค่าใหม่แบบอะตอมมิก
        playerRef.Child("wave").RunTransaction(data =>
        {
            // ยังไม่เคยมีข้อมูล (null) ให้ถือว่าเป็น 0
            long old = data.Value == null ? 0 : (long)data.Value;

            // wave ใหม่ไม่เกินสถิติเดิม ไม่ต้องบันทึก (Abort = ยกเลิก ค่าเดิมไม่ถูกแตะ)
            if (wave <= old) return TransactionResult.Abort();

            // wave ใหม่สูงกว่า เขียนทับสถิติเดิม
            data.Value = (long)wave;
            return TransactionResult.Success(data);

        }).ContinueWithOnMainThread(task => playerRef.Child("name").SetValueAsync(playerName));
    }

    void LoadScores()
    {
        dbRef.Child("leaderboard")
            .OrderByChild("wave")
            .LimitToLast(100)
            .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception);
                return;
            }

            var list = new List<(string n, long s)>();

            foreach (var child in task.Result.Children)
            {
                if (!child.HasChild("wave") || !child.HasChild("name")) continue;   // ข้ามข้อมูลเก่าที่ไม่ครบ
                list.Add((
                    child.Child("name").Value.ToString(),
                    (long)child.Child("wave").Value));
            }

            list.Sort((a, b) => b.s.CompareTo(a.s));

            string names = "";
            string waves = "";

            for (int i = 0; i < list.Count; i++)
            {
                names += $"{i + 1}. {list[i].n}\n";
                waves += $"{list[i].s} WAVE\n";
            }

            nameText.text = names;
            waveText.text = waves;
        });
    }
}
