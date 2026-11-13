using UnityEngine;

public class musuhmengejar : MonoBehaviour
{
    [Header("Pergerakan Musuh")]
    [SerializeField] private float moveSpeed = 3f; // Kecepatan musuh mengejar

    private Transform playerTransform;

    private void Start()
    {
        // Mencari GameObject yang memiliki Tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player tidak ditemukan! Pastikan Player sudah diberi Tag 'Player'.");
        }
    }

    private void Update()
    {
        // Jika player ada di scene, musuh akan bergerak mendekati posisi player
        if (playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                playerTransform.position, 
                moveSpeed * Time.deltaTime
            );

            // Membalik arah sprite musuh (Flip) mengikuti posisi Player
            if (playerTransform.position.x < transform.position.x)
            {
                // Menghadap kiri
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // Menghadap kanan
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}