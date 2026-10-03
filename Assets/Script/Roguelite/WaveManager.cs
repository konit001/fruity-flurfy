using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static int TotalWave = 1;

    [Header("Wave")]
    [SerializeField] private float waveDuration = 60f;
    [SerializeField] private float durationPerWave = 10f;     // เวฟต่อไปนานขึ้นเท่านี้
    [SerializeField] private float spawnIntervalScale = 0.9f; // เวฟต่อไปศัตรูเกิดถี่ขึ้น
    [SerializeField] private int maxEnemiesPerWave = 3;       // เวฟต่อไปศัตรูอยู่พร้อมกันได้เพิ่มเท่านี้

    [Header("UI")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text waveText;

    [Header("Refs")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private GridMaterialSpawn materialSpawn;
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private ShopManager shopManager;

    private int wave;
    private float remaining;
    private bool counting;
    private bool firstStart;
    private float baseSpawnInterval;
    private int baseMaxEnemies;
    private Health playerHealth;

    void Start()
    {
        Time.timeScale = 1f;
        TotalWave = 1;

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerHealth.OnDeath += OnPlayerDeath;

        baseSpawnInterval = spawner.interval;
        baseMaxEnemies = spawner.maxEnemies;

        wave = Mathf.Max(TotalWave, 1) - 1; // StartWave() ถัดไปจะเริ่มที่เวฟที่เซฟไว้
        firstStart = true;
        StartWave();
        firstStart = false;
    }

    void OnDestroy()
    {
        if (playerHealth != null) playerHealth.OnDeath -= OnPlayerDeath;
    }

    void Update()
    {

        if (!counting) return;

        remaining -= Time.deltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            EndWave();
        }
        RefreshTimeUI();
    }

    void StartWave()
    {
        wave++;
        RefreshWaveUI();

        TotalWave = wave;

        if (!firstStart)
        {
            DroppedItem.ClearAll();
            if (materialSpawn != null) materialSpawn.Respawn();
        }

        spawner.interval = baseSpawnInterval * Mathf.Pow(spawnIntervalScale, wave - 1);
        spawner.maxEnemies = baseMaxEnemies + maxEnemiesPerWave * (wave - 1);
        remaining = waveDuration + durationPerWave * (wave - 1);
        RefreshTimeUI();

    }

    void BeginWave()
    {
        spawner.enabled = true;
        counting = true;
        RefreshWaveUI();
    }

    void RefreshWaveUI()
    {
        if (waveText == null) return;

        waveText.text = "Wave " + wave;
    }

    void EndWave()
    {
        counting = false;
        spawner.enabled = false;
        spawner.ClearEnemies();
        // เลือก buff ฟรี → เข้าร้านอัพเกรด → NEXT WAVE (ไม่มีร้านในฉากก็ไปเวฟถัดไปเลย)
        if (shopManager != null) buffManager.Show(() => shopManager.Open(wave, StartWave));
        else buffManager.Show(StartWave);
    }

    void RefreshTimeUI()
    {
        if (timeText == null) return;

        int total = Mathf.CeilToInt(remaining);
        timeText.text = (total / 60).ToString("00") + " : " + (total % 60).ToString("00");
    }

    void OnPlayerDeath()
    {
        counting = false;
        spawner.enabled = false;
        Invoke(nameof(RestartRun), 2f);
    }

    void RestartRun()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
