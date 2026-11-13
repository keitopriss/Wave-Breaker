using UnityEngine;

public class Musuh : MonoBehaviour
{
    public enum TipeMusuh { Walker, Ranged, Runner, Ghost }

    [Header("Tipe & Statistik Musuh")]
    public TipeMusuh tipeMusuh = TipeMusuh.Walker;
    [SerializeField] private float hp = 100f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int attackDamage = 10;

    [Header("Pengaturan Khusus Ranged")]
    [SerializeField] private float stopDistance = 5f; // Jarak berhenti untuk menembak
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 2f;
    private float fireTimer;

    [Header("Pengaturan Khusus Ghost")]
    [SerializeField] private float stealthDistance = 4f; // Jarak saat Ghost mulai terlihat
    private SpriteRenderer spriteRenderer;

    private Transform playerTransform;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Mencari objek dengan Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Tentukan statistik berdasarkan GDD
        InisialisasiStatistik();
    }

    private void InisialisasiStatistik()
    {
        switch (tipeMusuh)
        {
            case TipeMusuh.Walker:
                hp = 100f; moveSpeed = 2f; attackDamage = 10;
                break;
            case TipeMusuh.Ranged:
                hp = 150f; moveSpeed = 2.5f; attackDamage = 15;
                break;
            case TipeMusuh.Runner:
                hp = 200f; moveSpeed = 5.5f; attackDamage = 5;
                break;
            case TipeMusuh.Ghost:
                hp = 110f; moveSpeed = 3.5f; attackDamage = 20;
                break;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Perilaku Pergerakan Berdasarkan Tipe
        switch (tipeMusuh)
        {
            case TipeMusuh.Walker:
            case TipeMusuh.Runner:
                KejarPlayer();
                break;

            case TipeMusuh.Ranged:
                if (distanceToPlayer > stopDistance)
                {
                    KejarPlayer();
                }
                else
                {
                    TembakRanged();
                }
                break;

            case TipeMusuh.Ghost:
                KejarPlayer();
                PerbaruiTransparansiGhost(distanceToPlayer);
                break;
        }

        // Flip Sprite Arah Karakter
        if (playerTransform.position.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    private void KejarPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            playerTransform.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void TembakRanged()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            if (bulletPrefab != null)
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                // Komponen peluru musuh (opsional)
            }
        }
    }

    private void PerbaruiTransparansiGhost(float distance)
    {
        if (spriteRenderer == null) return;
        Color color = spriteRenderer.color;

        // Menghilang (transparan) jika jauh, memadat saat mendekat
        if (distance > stealthDistance)
            color.a = 0.2f; // Hampir kasat mata
        else
            color.a = 1.0f; // Muncul penuh

        spriteRenderer.color = color;
    }

    public void TerimaDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Mati();
        }
    }

    private void Mati()
    {
        if (spawnmusuh.Instance != null)
        {
            spawnmusuh.Instance.AddKill();
        }
        Destroy(gameObject);
    }
}