using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public bool showPageOnStart;
    private AchievementRepository repo;
    public static bool pageOpen;
    private static int toggledFrame = -1;
    public static bool EscapeHandled => pageOpen || toggledFrame == Time.frameCount;


    void Awake()
    {
        repo = new AchievementRepository(DbProvider.Connection);
        AchievementTracker.SetCatalog(achievements);

        if (page != null)
        {
            page.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pageOpen)
            {
                closePage();
                toggledFrame = Time.frameCount;
            }
        }
    }

    //  Popup 
    void ShowPopup(AchievementData data)
    {
        if (popupPrefab == null || popupParent == null) return;

        AchievementPopup popup = Instantiate(popupPrefab, popupParent);
        popup.Setup(data);
        StartCoroutine(RemoveAfter(popup.gameObject, popupSeconds));
    }

    IEnumerator RemoveAfter(GameObject popup, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (popup != null) Destroy(popup);
    }


    //  Page 
    public void openPage()
    {
        pageOpen = true;
        if (page != null) page.SetActive(pageOpen);
        RefreshList();
    }

    public void closePage()
    {
        pageOpen = false;
        if (page != null) page.SetActive(pageOpen);
    }

    void RefreshList()
    {
        if (panelPrefab == null || listParent == null) return;

        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, AchievementRow> save = GameSession.IsLoggedIn
            ? repo.GetAll(GameSession.PlayerId).ToDictionary(r => r.AchievementId)
            : new();

        foreach (AchievementData achievement in AchievementTracker.Catalog)
        {
            save.TryGetValue(achievement.id, out AchievementRow row);

            bool unlocked = row != null && !string.IsNullOrEmpty(row.UnlockedAt);
            int progress = row?.Progress ?? 0;

            var panel = Instantiate(panelPrefab, listParent);
            panel.Setup(achievement, progress, unlocked);
        }
        AchievementTracker.JustUnlocked.Clear();
    }
    void OnEnable() => AchievementTracker.OnUnlocked += ShowPopup;
    void OnDisable() => AchievementTracker.OnUnlocked -= ShowPopup;
    void OnDestroy() => pageOpen = false;
}
