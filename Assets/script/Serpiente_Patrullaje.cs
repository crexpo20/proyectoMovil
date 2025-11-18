using UnityEngine;

public class Patrullaje : MonoBehaviour
{
    // --- Configuración de Velocidad y Detección ---
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 1f;
    
    [Header("Configuración de Ataque")]
    [SerializeField] private float rangoAtaque = 2f;
    [SerializeField] private float cooldownAtaque = 1.5f; // Usa tu cold original
    
    [Header("Configuración de Raycast")]
    public Transform groundCheckOrigin;
    public Transform wallCheckOrigin;
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.1f;
    public LayerMask groundLayer;

    // --- Referencias ---
    private Rigidbody2D rb;
    private Animator animator;
    private Transform jugador;

    // --- Variables Privadas ---
    private int facingDirection = 1;
    private bool puedeAtacar = true;
    private float tiempoUltimoAtaque = -10f; // Usa tu lógica original
    private bool estaAtacando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (rb == null)
        {
            Debug.LogError("Patrullaje requiere un Rigidbody2D.");
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Patrullaje requiere un Animator.");
        }

        facingDirection = (transform.localScale.x > 0) ? 1 : -1;
    }

    void Update()
    {
        // Verificar si el jugador está en rango de ataque
        if (jugador != null && !estaAtacando)
        {
            VerificarAtaque();
        }

        ActualizarAnimaciones();
    }

    void FixedUpdate()
    {
        // Si está atacando, no moverse
        if (estaAtacando)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // 1. Detección de Vacío
        bool isGroundedAhead = CheckGroundAhead();

        // 2. Detección de Pared
        bool isWallAhead = CheckWallAhead();

        // 3. Lógica de Giro
        if (!isGroundedAhead || isWallAhead)
        {
            Flip();
        }

        // 4. Movimiento
        rb.linearVelocity = new Vector2(speed * facingDirection, rb.linearVelocity.y);
    }

    /// <summary>
    /// Verifica si el jugador está en rango y puede atacar
    /// </summary>
    private void VerificarAtaque()
    {
        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        
        // Verificar si el jugador está en rango y en la dirección correcta
        bool jugadorEnRango = distanciaAlJugador <= rangoAtaque;
        bool jugadorEnDireccionCorrecta = EstaJugadorEnDireccionCorrecta();

        // Usa tu lógica original de cooldown
        if (jugadorEnRango && jugadorEnDireccionCorrecta && Time.time >= tiempoUltimoAtaque + cooldownAtaque)
        {
            IniciarAtaque();
        }
    }

    /// <summary>
    /// Verifica si el jugador está en la dirección que mira la serpiente
    /// </summary>
    private bool EstaJugadorEnDireccionCorrecta()
    {
        if (jugador == null) return false;
        
        Vector2 direccionAlJugador = (jugador.position - transform.position).normalized;
        float dotProduct = Vector2.Dot(transform.right * facingDirection, direccionAlJugador);
        
        return dotProduct > 0.5f; // Jugador está más adelante que a los lados
    }

    /// <summary>
    /// Inicia la animación y lógica de ataque - MANTIENE TU LÓGICA ORIGINAL
    /// </summary>
    private void IniciarAtaque()
    {
        estaAtacando = true;
        tiempoUltimoAtaque = Time.time;

        // Tu lógica original de animación
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        Debug.Log("Serpiente atacando!");

        // Aplicar daño al jugador usando tu sistema original
        AplicarDanio();

        // Programar fin del ataque (ajusta el tiempo según tu animación)
        Invoke("FinalizarAtaque", 0.5f);
    }

    /// <summary>
    /// Aplica daño al jugador - MANTIENE TU LÓGICA ORIGINAL
    /// </summary>
    private void AplicarDanio()
    {
        if (jugador != null)
        {
            vidaPersonaje monje = jugador.GetComponent<vidaPersonaje>();
            if (monje != null)
            {
                monje.hit(); // Tu método original
                Debug.Log("Serpiente hizo daño al jugador!");
            }
        }
    }

    /// <summary>
    /// Finaliza el estado de ataque
    /// </summary>
    private void FinalizarAtaque()
    {
        estaAtacando = false;
        // El cooldown se maneja automáticamente con tu lógica de tiempoUltimoAtaque
    }

    /// <summary>
    /// Actualiza los parámetros del Animator
    /// </summary>
    private void ActualizarAnimaciones()
    {
        if (animator == null) return;

        // Parámetros básicos
        animator.SetBool("isMoving", !estaAtacando && Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("isAttacking", estaAtacando);
    }

    // Los métodos de detección se mantienen igual
    private bool CheckGroundAhead()
    {
        Vector2 rayOrigin = groundCheckOrigin.position;
        rayOrigin.x += wallCheckDistance * facingDirection;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        Color rayColor = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, rayColor);

        return hit.collider != null;
    }

    private bool CheckWallAhead()
    {
        Vector2 direction = Vector2.right * facingDirection;
        RaycastHit2D hit = Physics2D.Raycast(wallCheckOrigin.position, direction, wallCheckDistance, groundLayer);

        Color rayColor = hit.collider != null ? Color.blue : Color.yellow;
        Debug.DrawRay(wallCheckOrigin.position, direction * wallCheckDistance, rayColor);

        return hit.collider != null;
    }

    private void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Método para ser llamado desde la animación (Animation Event)
    public void OnAttackAnimationEnd()
    {
        Debug.Log("Animación de ataque terminada");
        estaAtacando = false;
    }

    // Visualización del rango de ataque en el Editor
    private void OnDrawGizmosSelected()
    {
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);

        // Dirección de ataque
        Gizmos.color = Color.magenta;
        Vector3 direccionAtaque = transform.right * facingDirection * rangoAtaque;
        Gizmos.DrawRay(transform.position, direccionAtaque);
    }
}