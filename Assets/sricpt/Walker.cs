using UnityEngine;

public class Walker : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player playerScript = collision.GetComponent<player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                Debug.Log("Player terkena serangan musuh!");
            }
        }
    }
}
