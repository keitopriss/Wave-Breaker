using System.Collections;
using UnityEngine;

public class spawnmusuh : MonoBehaviour
{
    public static spawnmusuh Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameObject enemyPrefab;  // Prefab musuh
    [SerializeField] private float spawnInterval = 2f; // Jeda waktu spawn (detik)

    [Header("Ukuran Arena (Batas Sisi Spawn)")]
    // Sesuaikan nilai ini dengan lebar (X) dan tinggi (Y) arena kamu
    [SerializeField] private Vector2 arenaSize = new Vector2(22f, 12f);

    private int killCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    public void AddKill()
    {
        killCount++;
    }

    public int GetKillCount()
    {
        return killCount;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector3 spawnPosition = GetRandomEdgePosition();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    // Fungsi menghitung posisi di 4 tepi luar arena
    private Vector3 GetRandomEdgePosition()
    {
        float halfWidth = arenaSize.x / 2f;
        float halfHeight = arenaSize.y / 2f;

        // Pilih salah satu sisi secara acak: 0 = Atas, 1 = Bawah, 2 = Kiri, 3 = Kanan
        int side = Random.Range(0, 4);
        Vector3 spawnPos = transform.position;

        switch (side)
        {
            case 0: // Sisi Atas
                spawnPos += new Vector3(Random.Range(-halfWidth, halfWidth), halfHeight, 0f);
                break;
            case 1: // Sisi Bawah
                spawnPos += new Vector3(Random.Range(-halfWidth, halfWidth), -halfHeight, 0f);
                break;
            case 2: // Sisi Kiri
                spawnPos += new Vector3(-halfWidth, Random.Range(-halfHeight, halfHeight), 0f);
                break;
            case 3: // Sisi Kanan
                spawnPos += new Vector3(halfWidth, Random.Range(-halfHeight, halfHeight), 0f);
                break;
        }

        return spawnPos;
    }

    // Garis merah batas spawn pada tab Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(arenaSize.x, arenaSize.y, 0f));
    }
}