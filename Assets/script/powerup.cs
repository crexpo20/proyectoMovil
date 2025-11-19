using UnityEngine;

public class powerup : MonoBehaviour
{
    public pewerEfecct powerUpEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            powerUpEffect.Apply(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
