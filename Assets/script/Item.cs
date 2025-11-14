using UnityEngine;

public class Item : MonoBehaviour
{
    public int bombAmount = 2;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager.BombCollected(bombAmount);
            Destroy(gameObject);

        }
    }
}
