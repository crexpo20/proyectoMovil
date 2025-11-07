using UnityEngine;

public class Explosion_script : MonoBehaviour
{
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float explosionRadius = 3f;
    private Animator animator;
    void Start()
    {

        AplicarFuerzaExplosion();
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }
    }

    private void AplicarFuerzaExplosion()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                rb.AddForce(direction * explosionForce, ForceMode2D.Impulse);
            }
        }
    }
    
    private void EfectosExplosion()
    {
       // Camera.main.GetComponent<CameraShake>().Shake();
    }

}
