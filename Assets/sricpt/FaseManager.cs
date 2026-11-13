using UnityEngine;
using TMPro; // Diperlukan untuk komponen TextMeshPro

public class FaseManager : MonoBehaviour
{
    public static FaseManager Instance;

    [Header("Pengaturan Fase 1")]
    public int targetKillFase1 = 15;
    private int currentKillCount = 0;
    private bool faseSelesai = false;

    [Header("Tampilan UI")]
    [SerializeField] private TextMeshProUGUI teksKill; // Slot UI Text

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI(); // Tampilkan teks awal (0 / 15) saat game dimulai
    }

    public void TambahKill()
    {
        if (faseSelesai) return;

        currentKillCount++;
        UpdateUI(); // Perbarui angka kill di layar

        if (currentKillCount >= targetKillFase1)
        {
            Fase1Selesai();    
        }
    }

    private void UpdateUI()
    {
        if (teksKill != null)
        {
            teksKill.text = $"Target Kill: {currentKillCount} / {targetKillFase1}";
        }
    }

    private void Fase1Selesai()
    {
        faseSelesai = true;
        if (teksKill != null)
        {
            teksKill.text = "FASE 1 SELESAI!";
        }
        Debug.Log("FASE 1 SELESAI! Target 15 Kill Tercapai.");
    }
}