using System.Text;
using TMPro;
using UnityEngine;

// วางในซีน DeadSence แล้วลาก Text มาใส่ — โชว์สถิติ 5 รอบล่าสุดของผู้เล่นที่ login อยู่
public class RunHistoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text historyText;
    [SerializeField] private int showCount = 5;

    void Start()
    {
        if (historyText == null) return;

        if (!GameSession.IsLoggedIn)
        {
            historyText.text = "";
            return;
        }

        var repo = new RunHistoryRepository(DbProvider.Connection);
        var sb = new StringBuilder();

        foreach (RunRecord run in repo.GetRecent(GameSession.PlayerId, showCount))
            sb.AppendLine($"Wave {run.WaveReached}  Kills {run.Kills}  Gold {run.GoldEarned}");

        historyText.text = sb.ToString();
    }
}
