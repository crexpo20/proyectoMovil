using UnityEngine;

public class PerseguirJugador : MonoBehaviour
{
    [Header("Persecución")]
    public Transform jugador; 
    public float rangoDeteccion = 5f;
    public float velocidadPersecucion = 3f;

    [Header("Detección de Obstáculos")]
    public Transform groundCheck;
    public float groundCheckDist = 0.3f;
    public Transform wallCheck;
    public float wallCheckDist = 0.3f;
    public LayerMask groundLayer;

    private Patrullaje patrulla;
    private Rigidbody2D rb;
    private Animator anim;

    private bool persecucionActiva = false;

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
            persecucionActiva = true;
            patrulla.enabled = false;
            if(anim != null) anim.SetBool("Perseguir", true);

            Perseguir();
        }
        else
        {
            persecucionActiva = false;
            if (patrulla != null)
            {
                patrulla.enabled = true;
            }
            if(anim != null) anim.SetBool("Perseguir", false);
        }
    }

    void Perseguir()
    {
        bool haySuelo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDist, groundLayer);
        bool hayPared = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDist, groundLayer);

        // Si no hay piso, frena y no cae bugueado flotando
        if (!haySuelo)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Si choca con pared, que dé la vuelta
        if (hayPared)
        {
            Flip();
            return;
        }

        // Ver si el jugador está a los lados, ignorar si está arriba
        float diferenciaVertical = jugador.position.y - transform.position.y;

        if (Mathf.Abs(diferenciaVertical) < 1.0f)
        {
            // Solo volteamos si el jugador no está muy arriba
            if ((jugador.position.x > transform.position.x && transform.localScale.x < 0) ||
                (jugador.position.x < transform.position.x && transform.localScale.x > 0))
            {
                Flip();
            }
        }

        // Dirección hacia el jugador
        float direccion = jugador.position.x > transform.position.x ? 1 : -1;

        rb.linearVelocity = new Vector2(direccion * velocidadPersecucion, rb.linearVelocity.y);
    }

    private void Flip()
    {
        Vector3 ls = transform.localScale;
        ls.x *= -1;
        transform.localScale = ls;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if(groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDist);

        Gizmos.color = Color.red;
        if(wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * wallCheckDist);
    }
}
