using UnityEngine;

public class Movimiento_Caveman : MonoBehaviour
{
    // --- Configuración de Velocidad y Detección ---
    [Header("Configuración de Movimiento")]
    [SerializeField] private float patrolSpeed = 1f; // Velocidad normal de patrullaje
    [SerializeField] private float chaseSpeed = 3f;  // Velocidad cuando persigue al jugador
    
    [Header("Configuración de Raycast")]
    public Transform groundCheckOrigin; 
    public Transform wallCheckOrigin; 
    public float groundCheckDistance = 0.1f; 
    public float wallCheckDistance = 0.1f; 
    public LayerMask groundLayer; 

    [Header("Configuración de Detección del Jugador")]
    [SerializeField] private float detectionRange = 10f; // Rango de detección visual
    [SerializeField] private float chaseRange = 8f;     // Rango máximo de persecución
    [SerializeField] private LayerMask playerLayer;     // Capa del jugador
    [SerializeField] private LayerMask obstacleLayer;   // Capa de obstáculos (para línea de visión)

    // --- Variables Privadas ---
    private Rigidbody2D rb;
    private int facingDirection = 1; 
    private Transform player;          // Referencia al jugador
    private bool isChasing = false;    // ¿Está persiguiendo al jugador?
    private Vector2 patrolStartPos;    // Posición inicial para retornar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Movimiento_Caveman requiere un Rigidbody2D.");
            enabled = false;
            return;
        }

        // Buscar al jugador automáticamente
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("No se encontró un objeto con tag 'Player'.");
        }

        facingDirection = (transform.localScale.x > 0) ? 1 : -1;
        patrolStartPos = transform.position;
    }

    void FixedUpdate()
    {
        // 1. Verificar si puede detectar al jugador
        bool canSeePlayer = CheckForPlayer();
        
        // 2. Lógica de estados: Patrullaje vs Persecución
        if (canSeePlayer && !isChasing)
        {
            StartChasing();
        }
        else if (isChasing && !canSeePlayer)
        {
            // Si pierde de vista al jugador, verificar si debe seguir persiguiendo
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > chaseRange)
            {
                StopChasing();
            }
        }

        // 3. Movimiento según el estado
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    /// <summary>
    /// Verifica si el jugador está en rango y visible
    /// </summary>
    private bool CheckForPlayer()
    {
        if (player == null) return false;

        // Calcular distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Si está fuera del rango de detección, no hacer nada
        if (distanceToPlayer > detectionRange) return false;

        // Calcular dirección al jugador
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        
        // Verificar si el jugador está en el campo de visión (frente al enemigo)
        bool playerInFront = (facingDirection > 0 && player.position.x > transform.position.x) ||
                            (facingDirection < 0 && player.position.x < transform.position.x);

        if (!playerInFront) return false;

        // Verificar línea de visión (que no haya obstáculos entre medio)
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            directionToPlayer, 
            detectionRange, 
            obstacleLayer
        );

        // Debug visual
        Debug.DrawRay(transform.position, directionToPlayer * detectionRange, 
                     hit.collider != null && hit.collider.CompareTag("Player") ? Color.green : Color.red);

        // Si el raycast golpea al jugador, puede verlo
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    /// <summary>
    /// Comportamiento de patrullaje normal
    /// </summary>
    private void Patrol()
    {
        // 1. Detección de Vacío y Paredes (igual que antes)
        bool isGroundedAhead = CheckGroundAhead();
        bool isWallAhead = CheckWallAhead();

        // 2. Lógica de Giro
        if (!isGroundedAhead || isWallAhead)
        {
            Flip();
        }

        // 3. Movimiento a velocidad de patrullaje
        rb.linearVelocity = new Vector2(patrolSpeed * facingDirection, rb.linearVelocity.y);
    }

    /// <summary>
    /// Comportamiento de persecución al jugador
    /// </summary>
    private void ChasePlayer()
    {
        if (player == null) return;

        // Determinar dirección hacia el jugador
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
        
        // Girar si es necesario para mirar al jugador
        if (Mathf.Abs(directionToPlayer - facingDirection) > 0.1f)
        {
            facingDirection = (int)directionToPlayer;
            Flip();
        }

        // Verificar si puede avanzar hacia el jugador (no hay obstáculos)
        bool canMoveForward = CheckGroundAhead() && !CheckWallAhead();
        
        if (canMoveForward)
        {
            // Moverse hacia el jugador a mayor velocidad
            rb.linearVelocity = new Vector2(chaseSpeed * facingDirection, rb.linearVelocity.y);
        }
        else
        {
            // Si hay obstáculo, detenerse momentáneamente o buscar alternativa
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    /// <summary>
    /// Iniciar persecución
    /// </summary>
    private void StartChasing()
    {
        isChasing = true;
        Debug.Log("¡Caveman está persiguiendo al jugador!");
        
        // Opcional: Aquí puedes agregar efectos/sonidos
        // AudioManager.Instance.PlaySound("CavemanAlert");
    }

    /// <summary>
    /// Detener persecución
    /// </summary>
    private void StopChasing()
    {
        isChasing = false;
        Debug.Log("Caveman dejó de perseguir al jugador");
    }

    // --- MÉTODOS EXISTENTES (sin cambios) ---
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

    // Debug visual en el Editor
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        // Dibujar rango de detección
        Gizmos.color = isChasing ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Dibujar rango de persecución
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}