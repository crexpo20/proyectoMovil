using UnityEngine;

public class powerups : MonoBehaviour
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
