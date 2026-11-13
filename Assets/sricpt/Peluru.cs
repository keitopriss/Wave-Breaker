using UnityEngine;

public class Peluru : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime); // Peluru otomatis hancur jika tidak kena musuh
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jika mengenai musuh
        if (collision.CompareTag("Musuh"))
        {
            darahmusuh musuh = collision.GetComponent<darahmusuh>();
            if (musuh != null)
            {
                musuh.TakeDamage(damage);
            }
            Destroy(gameObject); // Hancurkan peluru setelah kena
        }
    }
}