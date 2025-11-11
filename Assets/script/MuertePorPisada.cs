using UnityEngine;

public class MuertePorPisada : MonoBehaviour
{
    [Header("Configuración de Muerte por Pisada")]
    [SerializeField] private float fuerzaRebote = 8f;
    [SerializeField] private LayerMask capaJugador;
    [SerializeField] private float umbralPisada = 0.9f; // Más sensible para detectar desde arriba
    
    [Header("Referencias")]
    [SerializeField] private Transform puntoSuperior; // Punto de referencia para la parte superior
    [SerializeField] private float radioDeteccion = 0.3f; // Radio para detección circular
    
    private Patrullaje patrullaje;
    private Collider2D colisionadorSerpiente;
    private Rigidbody2D rb;
    private provocaDanio danioScript;
    private bool estaMuerta = false;

    void Start()
    {
        patrullaje = GetComponent<Patrullaje>();
        colisionadorSerpiente = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        danioScript = GetComponent<provocaDanio>();
        
        // Si no se asignó puntoSuperior, usar el transform
        if (puntoSuperior == null)
            puntoSuperior = transform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarColision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // También detectar si el jugador se mantiene encima
        ProcesarColision(collision);
    }

    private void ProcesarColision(Collision2D collision)
    {
        // Verificar si es el jugador y si la serpiente no está muerta
        if (((1 << collision.gameObject.layer) & capaJugador) != 0 && !estaMuerta)
        {
            // Verificar si el jugador viene desde arriba usando múltiples métodos
            bool esPisada = EsPisadaDesdeArriba(collision) || DetectarJugadorEncima();
            
            if (esPisada)
            {
                // DESACTIVAR INMEDIATAMENTE el script de daño antes de cualquier otra cosa
                if (danioScript != null)
                {
                    danioScript.enabled = false;
                    Debug.Log("Script de daño desactivado inmediatamente");
                }
                
                Morir();
                
                // Hacer rebotar al jugador
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rbJugador != null)
                {
                    rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, fuerzaRebote);
                }
            }
            else
            {
                // Si no es pisada, es colisión lateral - permitir que provocaDanio haga su trabajo
                Debug.Log("Colisión lateral - el jugador recibe daño");
                // El script provocaDanio se encargará del daño a través del Trigger
            }
        }
    }

    private bool EsPisadaDesdeArriba(Collision2D collision)
    {
        // Método 1: Analizar todos los puntos de contacto
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            Vector2 direccion = (contacto.point - (Vector2)puntoSuperior.position).normalized;
            
            // Dibujar rayo de debug
            Debug.DrawRay(contacto.point, direccion * 0.5f, Color.cyan, 1f);
            
            // Si el contacto viene desde arriba (dirección Y negativa) Y el jugador está moviéndose hacia abajo
            if (direccion.y < -umbralPisada && JugadorSeMueveHaciaAbajo(collision.gameObject))
            {
                Debug.Log("Pisada detectada - Dirección: " + direccion + ", Umbral: " + -umbralPisada);
                return true;
            }
        }
        return false;
    }

    private bool DetectarJugadorEncima()
    {
        // Método 2: Usar CircleCast para detectar jugador encima
        Vector2 origen = new Vector2(puntoSuperior.position.x, puntoSuperior.position.y);
        RaycastHit2D hit = Physics2D.CircleCast(origen, radioDeteccion, Vector2.up, 0.1f, capaJugador);
        
        // Dibujar círculo de debug
        Debug.DrawRay(origen, Vector2.up * 0.1f, hit.collider != null ? Color.green : Color.red, 0.1f);
        
        if (hit.collider != null && JugadorSeMueveHaciaAbajo(hit.collider.gameObject))
        {
            Debug.Log("Jugador detectado encima y moviéndose hacia abajo");
            return true;
        }
        
        return false;
    }

    private bool JugadorSeMueveHaciaAbajo(GameObject jugador)
    {
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        if (rbJugador != null)
        {
            // Si el jugador se mueve hacia abajo (velocidad Y negativa)
            bool seMueveHaciaAbajo = rbJugador.linearVelocity.y < -0.1f;
            Debug.Log("Velocidad jugador Y: " + rbJugador.linearVelocity.y + ", Moviéndose hacia abajo: " + seMueveHaciaAbajo);
            return seMueveHaciaAbajo;
        }
        return false;
    }

    private void Morir()
    {
        // Prevenir múltiples llamadas
        if (estaMuerta) return;
        
        estaMuerta = true;
        Debug.Log("Muerto por pisada!");
        
        // Desactivar componentes de la serpiente
        if (patrullaje != null)
            patrullaje.enabled = false;
        
        // El script de daño ya fue desactivado al inicio de la detección
        
        // IMPORTANTE: Desactivar el collider para evitar más colisiones
        if (colisionadorSerpiente != null)
            colisionadorSerpiente.enabled = false;
            
        // Detener movimiento
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        
        // Activar animación de muerte si existe
        Animator anim = GetComponent<Animator>();
        if (anim != null && TieneParametro(anim, "Morir"))
        {
            anim.SetTrigger("Morir");
            Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            // Si no hay animación, destruir inmediatamente
            Destroy(gameObject);
        }
    }

    private bool TieneParametro(Animator animator, string parametro)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parametro) return true;
        }
        return false;
    }

    // Dibujo de Gizmos para debug en el Editor
    private void OnDrawGizmosSelected()
    {
        if (puntoSuperior != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(puntoSuperior.position, radioDeteccion);
            
            // Dibujar área de detección superior
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
            Gizmos.DrawSphere(puntoSuperior.position, radioDeteccion);
        }
    }
}