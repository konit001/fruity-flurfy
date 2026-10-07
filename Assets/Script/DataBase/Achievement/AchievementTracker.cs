using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AchievementTracker
{
    public static event Action<AchievementData> OnUnlocked;
    public static readonly List<AchievementData> JustUnlocked = new List<AchievementData>();
    private static List<AchievementData> catalog = new List<AchievementData>();
    public static IReadOnlyList<AchievementData> Catalog => catalog;
    
    public static void SetCatalog(IEnumerable<AchievementData> list)
    {
        catalog = list.Where(a => a != null).ToList();
    }

    public static void EvaluateKills()
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        int playerId = GameSession.PlayerId;
        var repo = new AchievementRepository(db);

        Players player = new PlayerRepository(db).GetById(playerId);
        int kills = player?.Kill ?? 0;

        foreach (AchievementData a in Catalog)
        {
            if (a.metric != AchievementMetric.Kills) continue;

            if (repo.SaveProgress(playerId, a.id, kills, a.target))
            {
                JustUnlocked.Add(a);
                OnUnlocked?.Invoke(a);
            }
        }
    }

    public static void Evaluate(int waveReached)
    {
        if (!GameSession.IsLoggedIn) return;

        var db = DbProvider.Connection;
        int playerId = GameSession.PlayerId;
        var repo = new AchievementRepository(db);

        Players player = new PlayerRepository(db).GetById(playerId);
        int kills = player?.Kill ?? 0;
        int items = new InventoryRepository(db).GetAll(playerId).Sum(r => r.AllRun);
        int buffs = new PlayerBuffRepository(db).GetAllCount(playerId);

        foreach (AchievementData a in Catalog)
        {
            int value = 0;
            switch (a.metric)
            {
                case AchievementMetric.Kills:       value = kills; break;
                case AchievementMetric.BestWave:    value = waveReached; break;
                case AchievementMetric.ItemsBought: value = items; break;
                case AchievementMetric.BuffsPicked: value = buffs; break;
            }

            if (repo.SaveProgress(playerId, a.id, value, a.target))
            {
                JustUnlocked.Add(a);
                OnUnlocked?.Invoke(a);
            }
        }
    }
}
