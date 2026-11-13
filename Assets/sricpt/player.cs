using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Pisau / Pedang (Jarak Dekat)")]
    [SerializeField] private float damagePisau = 20f;
    [SerializeField] private float jangkauanPisau = 1.5f;
    [SerializeField] private LayerMask musuhLayer;

    [Header("Visual Tebasan Pedang")]
    [SerializeField] private Transform porosPedang;
    [SerializeField] private TrailRenderer trailPedang;
    [SerializeField] private float sudutTebasan = 150f;
    [SerializeField] private float durasiTebas = 0.15f;
    [SerializeField] private float cooldownTebas = 0.35f;
    private bool sedangMenebas = false;

    [Header("Tampilan UI (TMP)")]
    [SerializeField] private TextMeshProUGUI teksHP;
    [SerializeField] private TextMeshProUGUI teksAmunisi;

    [Header("Game Over")]
    [SerializeField] private GameObject panelGameOver;
    private bool isGameOver = false;

    private void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        currentHP = maxHP;
        sisaAmunisi = maxAmunisi;

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        if (porosPedang != null)
        {
            porosPedang.gameObject.SetActive(false);
        }

        UpdateUI();
    }

    private void Update()
    {
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

        // Serang Pedang / Pisau (Klik Kanan)
        if (Input.GetMouseButtonDown(1) && !sedangMenebas)
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
        StartCoroutine(AnimasiTebas());

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

    private IEnumerator AnimasiTebas()
    {
        sedangMenebas = true;

        if (porosPedang != null)
        {
            porosPedang.gameObject.SetActive(true);
            if (trailPedang != null) trailPedang.Clear();

            float sudutAwal = sudutTebasan / 2f;
            float sudutAkhir = -sudutTebasan / 2f;
            float waktu = 0f;

            while (waktu < durasiTebas)
            {
                waktu += Time.deltaTime;
                float t = waktu / durasiTebas;
                float sudutSekarang = Mathf.Lerp(sudutAwal, sudutAkhir, t);
                porosPedang.localRotation = Quaternion.Euler(0f, 0f, sudutSekarang);
                yield return null;
            }

            porosPedang.gameObject.SetActive(false);
        }

        float sisaCooldown = Mathf.Max(0f, cooldownTebas - durasiTebas);
        yield return new WaitForSeconds(sisaCooldown);
        sedangMenebas = false;
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
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
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