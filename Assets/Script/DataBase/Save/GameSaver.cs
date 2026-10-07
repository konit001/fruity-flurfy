using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSaver
{
    public static int GetSavedWave()
    {
        if (!GameSession.IsLoggedIn) return 1;

        SaveData save = new SaveRepository(DbProvider.Connection).Load(GameSession.PlayerId);
        return save != null ? Mathf.Max(save.Wave, 1) : 1;
    }

    public static void SaveAtWave(int wave)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        int gold = MoneyManager.Instance != null ? MoneyManager.Instance.GetGold() : 0;

        new PlayerRepository(db).UpdateProgress(GameSession.PlayerId, wave, gold);
        new SaveRepository(db).Save(GameSession.PlayerId, SceneManager.GetActiveScene().name, wave, gold);

        GetOrCreate<InventorySaveUI>().Save();
        AchievementTracker.Evaluate(wave);
    }

    public static void LoadRun()
    {
        if (!GameSession.IsLoggedIn) return;

        SaveData save = new SaveRepository(DbProvider.Connection).Load(GameSession.PlayerId);
        if (save == null) return;

        if (MoneyManager.Instance != null) MoneyManager.Instance.SetGold(save.Gold);

        GetOrCreate<InventorySaveUI>().Load();
        Object.FindFirstObjectByType<BuffManager>()?.Load();
    }

    public static void SaveOnDeath(int wave)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        var playerRepo = new PlayerRepository(db);

        playerRepo.UpdateProgress(GameSession.PlayerId, 1, 0);

        new SaveRepository(db).Save(GameSession.PlayerId, GameSession.GameScene, 1, 0);
        GetOrCreate<InventorySaveUI>().ResetRun();
        new PlayerBuffRepository(db).ResetRun(GameSession.PlayerId);
        AchievementTracker.Evaluate(wave);
        LeaderBoard.SubmitWave(playerRepo.GetById(GameSession.PlayerId).Name, wave);
    }

    private static T GetOrCreate<T>() where T : MonoBehaviour
    {
        T found = Object.FindFirstObjectByType<T>();
        if (found != null) return found;

        return new GameObject(typeof(T).Name).AddComponent<T>();
    }
}
