using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text playerInfoText;
    [SerializeField] private TMP_Text statusText;

    private PlayerRepository playerRepo;
    private SaveRepository saveRepo;

    void Start()
    {
        var db = DbProvider.Connection;
        playerRepo = new PlayerRepository(db);
        saveRepo = new SaveRepository(db);

        ShowPlayerInfo();
    }

    void ShowPlayerInfo()
    {
        if (playerInfoText == null) return;

        Players player = GameSession.IsLoggedIn ? playerRepo.GetById(GameSession.PlayerId) : null;
        SaveData save = player != null ? saveRepo.Load(player.Id) : null;

        if (player == null)
        {
            playerInfoText.text = "Guest (not saved)";
            return;
        }

        int wave = save != null ? save.Wave : player.Wave;
        int gold = save != null ? save.Gold : player.Gold;
        playerInfoText.text = "Welcome: " + player.Name + "\nWave: " + wave + "\nGold: " + gold;
    }

    // เซฟเวฟปัจจุบัน + ทอง + ของที่ซื้อ + buff
    public void OnSave()
    {
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (!GameSession.IsLoggedIn || waveManager == null)
        {
            SetStatus("Can't save here");
            return;
        }

        GameSaver.SaveAtWave(waveManager.CurrentWave);
        SetStatus("Saved wave " + waveManager.CurrentWave);
        ShowPlayerInfo();
    }

    public void OnLoad()
    {
        SaveData save = GameSession.IsLoggedIn ? saveRepo.Load(GameSession.PlayerId) : null;
        if (save == null)
        {
            SetStatus("No saved data");
            return;
        }

        SetStatus("Saved: wave " + save.Wave + ", gold " + save.Gold);
    }

    public void OnLogOut()
    {
        GameSession.LogOut();
        SceneManager.LoadScene(GameSession.StartScene);
    }

    private void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }
}
