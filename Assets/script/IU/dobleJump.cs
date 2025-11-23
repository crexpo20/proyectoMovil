using UnityEngine;

public class dobleJump : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Personaje_movimiento player = collision.GetComponent<Personaje_movimiento>();

            if (player != null)
            {
                player.UnlockDoubleJump();
            }

            Destroy(gameObject); 
        }
    }
}
