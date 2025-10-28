using UnityEngine;

public class Movimiento_Caveman : MonoBehaviour
{
   // --- Configuración de Velocidad y Detección ---
    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 1f; // Velocidad de movimiento
    
    [Header("Configuración de Raycast")]
    // Punto de origen del rayo, generalmente en el centro del objeto o un poco más bajo
    public Transform groundCheckOrigin; 
    // Punto de origen del rayo que chequea paredes
    public Transform wallCheckOrigin; 
    
    [Tooltip("Distancia a la que el enemigo busca el suelo antes de caer")]
    public float groundCheckDistance = 0.1f; 
    
    [Tooltip("Distancia a la que el enemigo detecta una pared")]
    public float wallCheckDistance = 0.1f; 

    // Capas con las que el enemigo debe interactuar (Suelo y Paredes)
    public LayerMask groundLayer; 

    // --- Variables Privadas ---
    private Rigidbody2D rb;
    // La dirección es +1 (Derecha) o -1 (Izquierda)
    private int facingDirection = 1; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Patrullaje requiere un Rigidbody2D.");
            enabled = false; // Desactiva el script si falta el Rigidbody
            return;
        }

        // Aseguramos que la serpiente se mueva a la derecha inicialmente
        facingDirection = (transform.localScale.x > 0) ? 1 : -1;
    }

    void FixedUpdate()
    {
        // 1. Detección de Vacío (Edge Detection)
        // Lanza un rayo hacia abajo (Vector2.down) desde una posición frontal
        bool isGroundedAhead = CheckGroundAhead();

        // 2. Detección de Pared (Wall Detection)
        // Lanza un rayo horizontalmente en la dirección del movimiento
        bool isWallAhead = CheckWallAhead();

        // 3. Lógica de Giro
        // Si no hay suelo por delante O si hay una pared, girar.
        if (!isGroundedAhead || isWallAhead)
        {
            Flip();
        }

        // 4. Movimiento
        // Aplica la velocidad en la dirección actual
        rb.linearVelocity = new Vector2(speed * facingDirection, rb.linearVelocity.y);
    }

    /// <summary>
    /// Lanza un Raycast para comprobar si hay suelo delante.
    /// </summary>
    private bool CheckGroundAhead()
    {
        // Calcular la posición frontal y ligeramente baja para el chequeo de suelo
        Vector2 rayOrigin = groundCheckOrigin.position;
        // Mueve el origen del rayo un poco hacia adelante en la dirección actual
        rayOrigin.x += wallCheckDistance * facingDirection; 

        // Raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // Opcional: Dibujar el rayo en la escena para depuración (solo visible en el Editor)
        Color rayColor = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(rayOrigin, Vector2.down * groundCheckDistance, rayColor);

        return hit.collider != null;
    }

/// <summary>
    /// Lanza un Raycast para comprobar si hay una pared inmediatamente delante.
    /// </summary>
    private bool CheckWallAhead()
    {
        // La dirección del rayo es la dirección de movimiento (facingDirection)
        Vector2 direction = Vector2.right * facingDirection;

        // Raycast
        RaycastHit2D hit = Physics2D.Raycast(wallCheckOrigin.position, direction, wallCheckDistance, groundLayer);

        // Opcional: Dibujar el rayo en la escena para depuración
        Color rayColor = hit.collider != null ? Color.blue : Color.yellow;
        Debug.DrawRay(wallCheckOrigin.position, direction * wallCheckDistance, rayColor);

        return hit.collider != null;
    }

    /// <summary>
    /// Invierte la dirección de movimiento y el escalado del sprite.
    /// </summary>
    private void Flip()
    {
        // 1. Invierte la dirección interna de movimiento
        facingDirection *= -1;

        // 2. Invierte el sprite (escalado local en el eje X)
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

}
