using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk Restart Scene
using TMPro;

public class player : MonoBehaviour
{
    [Header("Pergerakan")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("HP Player (GDD 3.3)")]
    [SerializeField] private float maxHP = 100f;
    private float currentHP;

    [Header("Pistol (Jarak Jauh)")]
    [SerializeField] private GameObject peluruPrefab;
    [SerializeField] private Transform titikTembak;
    [SerializeField] private int maxAmunisi = 12;
    [SerializeField] private float waktuReload = 1.5f;
    private int sisaAmunisi;
    private bool sedangReload = false;

    [Header("Pisau (Jarak Dekat)")]
    [SerializeField] private float damagePisau = 20f;
    [SerializeField] private float jangkauanPisau = 1.5f;
    [SerializeField] private LayerMask musuhLayer;

    [Header("Tampilan UI (TMP)")]
    [SerializeField] private TextMeshProUGUI teksHP;
    [SerializeField] private TextMeshProUGUI teksAmunisi;

    [Header("Game Over")]
    [SerializeField] private GameObject panelGameOver;
    private bool isGameOver = false;

    private void Start()
    {
        Time.timeScale = 1f; // Pastikan waktu berjalan normal saat mulai/restart
        isGameOver = false;
        currentHP = maxHP;
        sisaAmunisi = maxAmunisi;

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        UpdateUI();
    }

    private void Update()
    {
        // Hentikan kontrol jika sudah Game Over
        if (isGameOver) return;

        GerakPlayer();
        RotasiKeMouse();

        // Tembak Pistol (Klik Kiri)
        if (Input.GetMouseButtonDown(0) && !sedangReload)
        {
            if (sisaAmunisi > 0)
            {
                Tembak();
            }
            else
            {
                StartCoroutine(Reloading());
            }
        }

        // Manual Reload (Tombol R)
        if (Input.GetKeyDown(KeyCode.R) && !sedangReload && sisaAmunisi < maxAmunisi)
        {
            StartCoroutine(Reloading());
        }

        // Serang Pisau (Klik Kanan)
        if (Input.GetMouseButtonDown(1))
        {
            SerangPisau();
        }
    }

    private void GerakPlayer()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, v, 0f).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void RotasiKeMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
        transform.up = direction;
    }

    private void Tembak()
    {
        sisaAmunisi--;
        UpdateUI();
        Transform spawnPoint = titikTembak != null ? titikTembak : transform;
        Instantiate(peluruPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private IEnumerator Reloading()
    {
        sedangReload = true;
        UpdateUI();
        yield return new WaitForSeconds(waktuReload);
        sisaAmunisi = maxAmunisi;
        sedangReload = false;
        UpdateUI();
    }

    private void SerangPisau()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, jangkauanPisau, musuhLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            darahmusuh health = enemy.GetComponent<darahmusuh>();
            if (health != null)
            {
                health.TakeDamage(damagePisau);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isGameOver) return;

        currentHP -= amount;
        if (currentHP < 0) currentHP = 0;
        UpdateUI();

        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
        Time.timeScale = 0f; // Menghentikan seluruh pergerakan musuh dan spawner
    }

    // Fungsi ini akan dipanggil oleh Tombol Restart
    public void RestartGame()
    {
        Time.timeScale = 1f; // Kembalikan kecepatan waktu ke normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateUI()
    {
        if (teksHP != null)
        {
            teksHP.text = $"HP: {currentHP} / {maxHP}";
        }

        if (teksAmunisi != null)
        {
            if (sedangReload)
                teksAmunisi.text = "Ammo: Reloading...";
            else
                teksAmunisi.text = $"Ammo: {sisaAmunisi} / {maxAmunisi}";
        }
    }
}