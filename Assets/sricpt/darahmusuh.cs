using UnityEngine;

public class darahmusuh : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;
    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} terkena damage! Sisa HP: {currentHP}");

        if (currentHP <= 0)
        {
            Mati();
        }
    }

    private void Mati()
    {
        // Laporkan kill ke FaseManager
        if (FaseManager.Instance != null)
        {
            FaseManager.Instance.TambahKill();
        }

        GetComponent<SpriteRenderer>().color = Color.red;

        Destroy(gameObject,0.1f);
    }
}