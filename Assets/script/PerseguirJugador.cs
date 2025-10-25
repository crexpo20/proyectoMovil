using UnityEngine;

public class PerseguirJugador : MonoBehaviour
{
    public Transform jugador; 
    public float rangoDeteccion = 5f;
    public float velocidadPersecucion = 3f;

    private Patrullaje patrulla;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        patrulla = GetComponent<Patrullaje>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if(jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia < rangoDeteccion)
        {
            // Empieza la persecución
            patrulla.enabled = false;

            if(anim != null)
                anim.SetBool("Perseguir", true);

            Perseguir();
        }
        else
        {
            // Vuelve a patrullar
            patrulla.enabled = true;
            
            if(anim != null)
                anim.SetBool("Perseguir", false);
        }
    }

    void Perseguir()
    {
        // Mira hacia el jugador
        if ((jugador.position.x > transform.position.x && transform.localScale.x < 0) ||
            (jugador.position.x < transform.position.x && transform.localScale.x > 0))
        {
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }

        float direccion = jugador.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direccion * velocidadPersecucion, rb.linearVelocity.y);
    }
}