using UnityEngine;

public class CavemanReaccionAtaque : MonoBehaviour
{
    [Header("Configuración de Retroceso")]
    [SerializeField] private float fuerzaRetroceso = 3f; // Reducida para evitar que vuele
    [SerializeField] private float duracionRetroceso = 0.3f;
    
    [Header("Referencias")]
    [SerializeField] private string animacionCaida = "Caida"; // Nombre del trigger de animación
    [SerializeField] private LayerMask capaJugador;
    
    private Rigidbody2D rb;
    private Animator anim;
    private Movimiento_Caveman movimientoCaveman;
    private PerseguirJugador perseguirJugador;
    private bool enRetroceso = false;
    private float tiempoRetroceso = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        movimientoCaveman = GetComponent<Movimiento_Caveman>();
        perseguirJugador = GetComponent<PerseguirJugador>();
    }

    void Update()
    {
        // Controlar el tiempo de retroceso
        if (enRetroceso)
        {
            tiempoRetroceso += Time.deltaTime;
            
            // Limitar la velocidad vertical durante el retroceso
            if (rb != null)
            {
                // Mantener la velocidad Y controlada (evitar que vuele)
                float velocidadY = Mathf.Clamp(rb.linearVelocity.y, -5f, 2f);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, velocidadY);
            }
            
            if (tiempoRetroceso >= duracionRetroceso)
            {
                TerminarRetroceso();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si es colisión con el jugador y no estamos en retroceso
        if (((1 << collision.gameObject.layer) & capaJugador) != 0 && !enRetroceso)
        {
            // Verificar que no sea una pisada desde arriba
            if (!EsPisadaDesdeArriba(collision))
            {
                IniciarRetroceso(collision);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // También detectar triggers por si el jugador usa collider de trigger
        vidaPersonaje monje = collision.GetComponent<vidaPersonaje>();
        if (monje != null && !enRetroceso)
        {
            // Pequeño delay para asegurar que el daño se aplique primero
            Invoke(nameof(IniciarRetrocesoDesdeTrigger), 0.1f);
        }
    }

    private void IniciarRetrocesoDesdeTrigger()
    {
        if (!enRetroceso)
        {
            IniciarRetroceso(null);
        }
    }

    private void IniciarRetroceso(Collision2D collision)
    {
        enRetroceso = true;
        tiempoRetroceso = 0f;

        // 1. Aplicar fuerza de retroceso CONTROLADA
        AplicarFuerzaRetroceso(collision);

        // 2. Activar animación de caída
        if (anim != null)
        {
            anim.SetTrigger(animacionCaida);
        }

        // 3. Deshabilitar movimiento temporalmente
        if (movimientoCaveman != null)
            movimientoCaveman.enabled = false;
        
        if (perseguirJugador != null)
            perseguirJugador.enabled = false;

        Debug.Log("Caveman en retroceso después de atacar");
    }

    private void AplicarFuerzaRetroceso(Collision2D collision)
    {
        if (rb == null) return;

        // Determinar dirección del retroceso (opuesta a la dirección del caveman)
        int direccionRetroceso = - (int)Mathf.Sign(transform.localScale.x);
        
        // Aplicar fuerza de retroceso SOLO HORIZONTAL y controlada
        Vector2 retroceso = new Vector2(direccionRetroceso * fuerzaRetroceso, 0f);
        
        rb.linearVelocity = retroceso;
        
        // NO aplicar fuerza vertical - mantenerlo en el suelo
        // rb.linearVelocity = new Vector2(retroceso.x, Mathf.Clamp(rb.linearVelocity.y, -1f, 1f));
    }

    private void TerminarRetroceso()
    {
        enRetroceso = false;
        tiempoRetroceso = 0f;

        // Rehabilitar movimiento
        if (movimientoCaveman != null)
            movimientoCaveman.enabled = true;
        
        if (perseguirJugador != null)
            perseguirJugador.enabled = true;

        // Detener cualquier velocidad residual
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        Debug.Log("Caveman recuperado del retroceso");
    }

    private bool EsPisadaDesdeArriba(Collision2D collision)
    {
        // Verificar si el contacto viene desde arriba
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            Vector2 direccion = (contacto.point - (Vector2)transform.position).normalized;
            if (direccion.y > 0.7f) // Contacto desde arriba
            {
                return true;
            }
        }
        return false;
    }

    // Método público para forzar retroceso desde otros scripts
    public void ForzarRetroceso()
    {
        if (!enRetroceso)
        {
            IniciarRetroceso(null);
        }
    }
}