using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// UI achievement 2 อย่าง แบบเดียวกับ BuffManager / ShopManager (prefab แล้ว spawn ลงใน layout)
// 1) Popup : เด้งบนจอตอนปลดล็อก (popupPrefab -> popupParent)
// 2) Page  : หน้ารายละเอียดทุก achievement พร้อมความคืบหน้า (panelPrefab -> listParent)
// ช่องที่ไม่ใช้เว้นว่างได้ — วางได้ทุกซีน เช่น ป๊อปอัพในซีนเกม, หน้า Page ในเมนู/DeadSence
public class AchievementUI : MonoBehaviour
{
    [Header("Data")]
    public List<AchievementData> achievements = new List<AchievementData>();

    [Header("Popup")]
    public AchievementPopup popupPrefab;
    public Transform popupParent;
    public float popupSeconds = 3f;

    [Header("Page")]
    public GameObject page;
    public AchievementPanel panelPrefab;
    public Transform listParent;
    public bool showPageOnStart;   // true = เปิดหน้า Page ทันทีที่เข้าซีน (เช่น DeadSence)
    public Button openButton;      // ปุ่มเปิดหน้า (ไม่บังคับ — หรือผูก OpenPage ใน OnClick เอง)
    public Button closeButton;     // ปุ่มปิดหน้า (อยู่ในหน้า Page)

    private AchievementRepository repo;

    private static bool pageOpen;
    private static int escapeFrame = -1;

    // true เมื่อ Esc ครั้งนี้ถูก Page ใช้แล้ว (หน้าเปิดอยู่ หรือเพิ่งปิดเฟรมนี้) — UiManager ใช้เช็คไม่ให้เปิดหน้าตั้งค่าซ้อน
    public static bool EscapeHandled => pageOpen || escapeFrame == Time.frameCount;

    void Awake()
    {
        repo = new AchievementRepository(DbProvider.Connection);
        AchievementTracker.SetCatalog(achievements);

        if (openButton != null) openButton.onClick.AddListener(OpenPage);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePage);

        if (page != null) page.SetActive(false);
    }

    void OnEnable()
    {
        AchievementTracker.OnUnlocked += ShowPopup;
    }

    void OnDisable()
    {
        AchievementTracker.OnUnlocked -= ShowPopup;
    }

    void OnDestroy()
    {
        pageOpen = false;
    }

    // Esc ปิดหน้า Page
    void Update()
    {
        if (!pageOpen) return;
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;

        escapeFrame = Time.frameCount;
        ClosePage();
    }

    void Start()
    {
        if (showPageOnStart) OpenPage();
    }

    // ---------- Popup ----------

    void ShowPopup(AchievementData data)
    {
        if (popupPrefab == null || popupParent == null) return;

        AchievementPopup popup = Instantiate(popupPrefab, popupParent);
        popup.Setup(data);
        StartCoroutine(RemoveAfter(popup.gameObject, popupSeconds));
    }

    // นับเวลาจริง — ป๊อปอัพหายได้แม้ Time.timeScale = 0
    IEnumerator RemoveAfter(GameObject popup, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (popup != null) Destroy(popup);
    }

    // ---------- Page ----------

    // ผูกกับปุ่มเปิดหน้า achievement
    public void OpenPage()
    {
        if (page == null) return;

        RefreshList();
        page.SetActive(true);
        pageOpen = true;
    }

    // ผูกกับปุ่มปิด (Esc ก็เรียกตัวนี้)
    public void ClosePage()
    {
        if (page != null) page.SetActive(false);
        pageOpen = false;
    }

    // สร้างรายการใหม่ทั้งหมด — แถวที่เพิ่งปลดใหม่ติดป้าย NEW แล้วเคลียร์รายการ "ใหม่"
    void RefreshList()
    {
        if (panelPrefab == null || listParent == null) return;

        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        Dictionary<string, AchievementRow> rows = GameSession.IsLoggedIn
            ? repo.GetAll(GameSession.PlayerId).ToDictionary(r => r.AchievementId)
            : new Dictionary<string, AchievementRow>();

        foreach (AchievementData a in AchievementTracker.Catalog)
        {
            rows.TryGetValue(a.id, out AchievementRow row);
            bool unlocked = row != null && !string.IsNullOrEmpty(row.UnlockedAt);
            int progress = row != null ? row.Progress : 0;
            bool isNew = AchievementTracker.JustUnlocked.Contains(a);

            AchievementPanel panel = Instantiate(panelPrefab, listParent);
            panel.Setup(a, progress, unlocked, isNew);
        }

        AchievementTracker.JustUnlocked.Clear();
    }
}
