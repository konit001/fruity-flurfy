using System.Collections.Generic;
using UnityEngine;

// รายการซีนที่ Portal สุ่มไป — สร้างได้จาก Create > Game > Scene List
// ซีนทุกตัวต้องอยู่ใน Build Settings
[CreateAssetMenu(menuName = "Game/Scene List", fileName = "SceneList")]
public class SceneList : ScriptableObject
{
    public List<string> scenes = new List<string>();

    // สุ่มซีนที่โหลดได้ ถ้ามีตัวเลือกมากกว่า 1 จะไม่เลือก avoid; คืน null ถ้าไม่มีซีนใช้ได้
    public string Pick(string avoid = null)
    {
        var candidates = new List<string>();
        foreach (string s in scenes)
        {
            if (string.IsNullOrEmpty(s)) continue;
            if (!Application.CanStreamedLevelBeLoaded(s))
            {
                Debug.LogError($"[SceneList] {name}: ไม่พบซีน '{s}' ใน Build Settings", this);
                continue;
            }
            candidates.Add(s);
        }

        if (!string.IsNullOrEmpty(avoid) && candidates.Count > 1)
            candidates.RemoveAll(s => s == avoid);

        return candidates.Count == 0 ? null : candidates[Random.Range(0, candidates.Count)];
    }
}
