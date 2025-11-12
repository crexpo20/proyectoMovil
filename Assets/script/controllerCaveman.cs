using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerCaveman : MonoBehaviour
{
    // --- Parámetros de Detección y Movimiento ---
    public Transform personaje; // Referencia al Transform del jugador
    public float detectionRadius = 3f;
    public float speed = 5.0f;

    // --- Parámetros de Ataque y Cooldown ---
    // NO se usa vidaPersonaje Vidajuador; porque el ataque se maneja por colisión (OnTriggerEnter2D)
    public float attackCooldown = 1f; // Tiempo de espera entre ataques
    private float lastAttackTime = -10f;

    // --- Componentes Privados ---
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    // --- Nombres de Parámetros de Animación ---
    // Mejor usar strings constantes para evitar errores de tipeo
    private const string ATTACK_ANIM_BOOL = "IsAttacking"; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        HandleMovementAndDetection();
    }

    private void HandleMovementAndDetection()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, personaje.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (personaje.position - transform.position).normalized;
            // Solo movimiento horizontal (eje X)
            movement = new Vector2(direction.x, 0);

            // Activar Animator si estaba desactivado
           // if (animator != null && !animator.enabled)
                animator.enabled = true;

            // Lógica para girar el sprite en la dirección del movimiento
            FlipSprite(movement.x);
        }
        else
        {
            movement = Vector2.zero;

            // Desactivar Animator cuando no detecta al jugador 
            if (animator != null)
                animator.enabled = false;
        }

        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

        // **Opcional:** Si deseas una animación de caminar, usarías aquí un float. 
        // Por ahora, solo se usa el bool de ataque en el Trigger.
    }

    private void FlipSprite(float horizontalMovement)
    {
        if (horizontalMovement > 0)
        {
            // Moviéndose a la derecha (asume escala por defecto 1.4f)
            transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
        else if (horizontalMovement < 0)
        {
            // Moviéndose a la izquierda (escala invertida en X)
            transform.localScale = new Vector3(-1.4f, 1.4f, 1.4f);
        }
    }

    // --- Lógica de Ataque ---
    // Usa un Trigger Collider 2D para detectar al jugador y atacarlo
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Busca el componente vidaPersonaje en el objeto que colisiona
        vidaPersonaje playerHealth = collision.GetComponent<vidaPersonaje>();

        if (playerHealth != null)
        {
            // Verifica el cooldown antes de atacar
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                
                // 1. Activa la animación de ataque usando un Bool
                if (animator != null)
                {
                    // Se establece a true y el animador debe encargarse de resetearlo a false
                    // al finalizar la animación (usando un Event en la animación o un script de StateMachineBehaviour).
                    animator.SetBool(ATTACK_ANIM_BOOL, true);
                }

                // 2. Aplica daño al jugador
                playerHealth.hit();
            }
        }
    }
    
    // --- Función para ser llamada al final de la animación de ataque ---
    // (Opcional, pero esencial si no usas un StateMachineBehaviour para resetear el bool)
    public void OnAttackAnimationEnd()
    {
        if (animator != null)
        {
            animator.SetBool(ATTACK_ANIM_BOOL, false);
        }
    }


    // --- Utilidad de Depuración (Gizmos) ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}