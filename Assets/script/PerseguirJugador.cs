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
            patrulla.enabled = false;
            if(anim != null) anim.SetBool("Perseguir", true);

            Perseguir();
        }
        else
        {
            patrulla.enabled = true;
            if(anim != null) anim.SetBool("Perseguir", false);
        }
    }

    void Perseguir()
    {
        bool haySuelo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDist, groundLayer);
        bool hayPared = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDist, groundLayer);

        // Evita quedarse flotando en vacío
        if (!haySuelo)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Si toca pared → voltear
        if (hayPared)
        {
            Flip();
            return;
        }

        float distanciaX = jugador.position.x - transform.position.x;
        float distanciaY = jugador.position.y - transform.position.y;

        // Si el jugador está arriba pero cerca en X → voltearlo igual
        if (distanciaY > 1f && Mathf.Abs(distanciaX) < 1.2f)
        {
            // El jugador me pasó por arriba → me doy la vuelta
            if ((distanciaX > 0 && transform.localScale.x < 0) ||
                (distanciaX < 0 && transform.localScale.x > 0))
            {
                Flip();
            }
        }
        else
        {
            // Jugador está a nivel normal → volteo normal
            if ((distanciaX > 0 && transform.localScale.x < 0) ||
                (distanciaX < 0 && transform.localScale.x > 0))
            {
                Flip();
            }
        }

        // Movimiento de persecución
        float direccion = distanciaX > 0 ? 1 : -1;

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
