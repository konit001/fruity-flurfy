using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    public float interval = 2f;
    public int maxEnemies = 20;
    [SerializeField] private int maxEnemiesPerWave = 3;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 14f;

    private Transform player;
    private float timer;

    void Start()
    {
        var obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) player = obj.transform;
    }

    void Update()
    {
        if (player == null || enemyPrefabs.Length == 0) return;

        timer += Time.deltaTime;
        if (timer < interval) return;
        timer = 0f;

        if (EnemyControl.All.Count >= maxEnemies) return;
        SpawnOne();
    }

    void SpawnOne()
    {
        // สุ่มจุดรอบผู้เล่น ที่ความสูงเดียวกับผู้เล่น
        Vector2 dir = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
        Vector3 pos = player.position + new Vector3(dir.x, 0f, dir.y);

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, pos, Quaternion.identity);
    }

    public void ClearEnemies()
    {
        foreach (var enemy in EnemyControl.All.ToArray())
            Destroy(enemy.gameObject);
    }
}
