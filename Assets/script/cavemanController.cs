using UnityEngine;

public class CavemanController : MonoBehaviour
{
    [Header("=== CONFIGURACIÓN VIDA ===")]
    [SerializeField] private int saludMaxima = 3;
    [SerializeField] private int danioAtaque = 1;
    private int saludActual;
    private bool estaMuerto = false;

    [Header("=== CONFIGURACIÓN MOVIMIENTO ===")]
    [SerializeField] private float velocidadPatrullaje = 2f;
    [SerializeField] private float velocidadPersecucion = 3.5f;
    
    [Header("=== CONFIGURACIÓN DETECCIÓN ===")]
    [SerializeField] private float rangoDeteccion = 5f;
    [SerializeField] private float rangoPersecucion = 8f;
    [SerializeField] private LayerMask capaJugador;
    [SerializeField] private LayerMask capaObstaculos;

    [Header("=== DETECCIÓN DE OBSTÁCULOS ===")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private float wallCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("=== CONFIGURACIÓN RETROCESO ===")]
    [SerializeField] private float fuerzaRetroceso = 3f;
    [SerializeField] private float duracionRetroceso = 0.3f;
    private bool enRetroceso = false;
    private float tiempoRetroceso;

    [Header("=== COMPONENTES ===")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform jugador;

    // Estados
    private enum EstadoCaveman { Patrullando, Persiguiendo, Retroceso, Muerto }
    private EstadoCaveman estadoActual = EstadoCaveman.Patrullando;

    // Dirección y control
    private int direccion = 1;
    private float cooldownAtaque = 0.5f;
    private float tiempoUltimoAtaque;
    private bool enSuelo = false;

    // Constantes para parámetros de animación
    private const string PARAM_MOVIMIENTO = "Movimiento";
    private const string PARAM_PERSIGUIENDO = "Persiguiendo";
    private const string PARAM_MUERTO = "Muerto";
    private const string PARAM_DANIO = "Danio";
    private const string PARAM_EN_SUELO = "EnSuelo";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador == null)

        saludActual = saludMaxima;
        direccion = transform.localScale.x > 0 ? 1 : -1;
    }

    void Update()
    {
        if (estaMuerto) return;
        
        // Verificar si está en suelo
        enSuelo = VerificarSuelo();
        
        if (enRetroceso)
        {
            ManejarRetroceso();
            return;
        }

        ActualizarEstado();
        ActualizarAnimaciones();
    }

    void FixedUpdate()
    {
        if (estaMuerto || enRetroceso) return;

        EjecutarMovimiento();
    }

    #region MÁQUINA DE ESTADOS
    private void ActualizarEstado()
    {
        if (jugador == null) 
        {
            estadoActual = EstadoCaveman.Patrullando;
            return;
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        bool puedeVerJugador = JugadorEnLineaDeVision();

        if (distanciaAlJugador <= rangoDeteccion && puedeVerJugador)
        {
            estadoActual = EstadoCaveman.Persiguiendo;
        }
        else if (distanciaAlJugador > rangoPersecucion || !puedeVerJugador)
        {
            estadoActual = EstadoCaveman.Patrullando;
        }
    }

    private void EjecutarMovimiento()
    {
        switch (estadoActual)
        {
            case EstadoCaveman.Patrullando:
                Patrullar();
                break;

            case EstadoCaveman.Persiguiendo:
                PerseguirYAtacar();
                break;
        }
    }
    #endregion

    #region COMPORTAMIENTOS DE MOVIMIENTO
    private void Patrullar()
    {
        // En patrullaje normal, SÍ respeta los bordes
        bool puedeAvanzar = HaySueloAdelante() && !HayMuroAdelante();
        
        if (!puedeAvanzar)
        {
            Girar();
        }

        // Moverse en la dirección actual
        rb.linearVelocity = new Vector2(velocidadPatrullaje * direccion, rb.linearVelocity.y);
        
    }

    private void PerseguirYAtacar()
    {
        if (jugador == null) 
        {
            estadoActual = EstadoCaveman.Patrullando;
            return;
        }

        // Determinar dirección hacia el jugador
        float direccionJugador = Mathf.Sign(jugador.position.x - transform.position.x);
        
        // Girar para mirar al jugador si es necesario
        if (Mathf.Abs(direccionJugador - direccion) > 0.1f)
        {
            direccion = (int)direccionJugador;
            Girar();
        }

        // En persecución, SOLO verifica muros, IGNORA si hay suelo adelante
        // Esto permite que caiga de plataformas al perseguir
        bool hayMuro = HayMuroAdelante();
        
        if (!hayMuro)
        {
            // Moverse hacia el jugador - puede caer si no hay suelo
            rb.linearVelocity = new Vector2(velocidadPersecucion * direccion, rb.linearVelocity.y);
            
            // Aplicar daño si está suficientemente cerca
            AplicarDanioPorContacto();
            
        }
        else
        {
            // Detenerse solo si hay muro (no si no hay suelo)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            
            // Intentar girar para buscar otro camino
            Girar();
        }
    }

    private void AplicarDanioPorContacto()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);
        
        // Si está muy cerca del jugador y puede atacar
        if (distancia < 1.2f && Time.time >= tiempoUltimoAtaque + cooldownAtaque)
        {
            vidaPersonaje vidaJugador = jugador.GetComponent<vidaPersonaje>();
            if (vidaJugador != null)
            {
                vidaJugador.hit();
                tiempoUltimoAtaque = Time.time;
            }
        }
    }
    #endregion

    #region DETECCIÓN DE OBSTÁCULOS
    private bool VerificarSuelo()
    {
        if (groundCheck == null) return false;
        
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private bool HaySueloAdelante()
    {
        if (groundCheck == null) return false;
        
        // Posición adelantada para verificar suelo delante
        Vector2 origenAdelantado = groundCheck.position;
        origenAdelantado.x += 0.5f * direccion;
        
        RaycastHit2D hit = Physics2D.Raycast(origenAdelantado, Vector2.down, groundCheckDistance, groundLayer);
        
        // Debug visual
        Debug.DrawRay(origenAdelantado, Vector2.down * groundCheckDistance, hit.collider != null ? Color.green : Color.red);
        
        return hit.collider != null;
    }

    private bool HayMuroAdelante()
    {
        // Usar la posición del transform principal para detectar muros
        Vector2 origen = transform.position;
        Vector2 direccionRayo = Vector2.right * direccion;
        
        RaycastHit2D hit = Physics2D.Raycast(origen, direccionRayo, wallCheckDistance, groundLayer);
        
        // Debug visual
        Debug.DrawRay(origen, direccionRayo * wallCheckDistance, hit.collider != null ? Color.blue : Color.yellow);
        
        return hit.collider != null;
    }

    private bool JugadorEnLineaDeVision()
    {
        if (jugador == null) return false;

        Vector2 direccion = (jugador.position - transform.position).normalized;
        float distancia = Vector2.Distance(transform.position, jugador.position);
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, distancia, capaObstaculos);

        Debug.DrawRay(transform.position, direccion * distancia, 
                     hit.collider == null ? Color.green : Color.red);

        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    private void Girar()
    {
        direccion *= -1;
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
        transform.localScale = escala;
        
    }
    #endregion

    #region DAÑO Y MUERTE
    public void RecibirDanio(int danio, Vector2 direccionDanio = default)
    {
        if (estaMuerto) return;
        
        saludActual -= danio;
        
        if (saludActual <= 0)
        {
            Morir();
        }
        else
        {
            // Animación de daño y retroceso
            if (animator != null)
                animator.SetTrigger(PARAM_DANIO);
            
            IniciarRetroceso(direccionDanio);
        }
    }

    private void Morir()
    {
        estaMuerto = true;
        estadoActual = EstadoCaveman.Muerto;
        
        // Detener movimiento
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        
        // Desactivar colisiones
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        
        // Animación de muerte
        if (animator != null)
            animator.SetTrigger(PARAM_MUERTO);
        
    }
    #endregion

    #region RETROCESO
    private void IniciarRetroceso(Vector2 direccionDanio)
    {
        enRetroceso = true;
        tiempoRetroceso = 0f;
        estadoActual = EstadoCaveman.Retroceso;

        // Aplicar fuerza de retroceso
        if (rb != null)
        {
            Vector2 direccionRetroceso = direccionDanio != default ? 
                -direccionDanio.normalized : 
                new Vector2(-direccion, 0.3f);
            
            rb.linearVelocity = direccionRetroceso * fuerzaRetroceso;
        }
    }

    private void ManejarRetroceso()
    {
        tiempoRetroceso += Time.deltaTime;
        
        if (tiempoRetroceso >= duracionRetroceso)
        {
            enRetroceso = false;
            estadoActual = EstadoCaveman.Patrullando;
            
            if (rb != null)
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    #endregion

    #region ANIMACIONES
    private void ActualizarAnimaciones()
    {
        if (animator == null) return;

        animator.SetFloat(PARAM_MOVIMIENTO, Mathf.Abs(rb.linearVelocity.x));
        //animator.SetBool(PARAM_PERSIGUIENDO, estadoActual == EstadoCaveman.Persiguiendo);
        animator.SetBool(PARAM_MUERTO, estaMuerto);
    }
    #endregion

    #region COLISIONES Y TRIGGERS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (estaMuerto) return;

        // Detectar arma del jugador
        if (collision.CompareTag("weapon"))
        {
            Vector2 direccionDanio = (transform.position - collision.transform.position).normalized;
            RecibirDanio(1, direccionDanio);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estaMuerto) return;

        // Retroceso por colisión con jugador
        if (collision.gameObject.CompareTag("Player") && !enRetroceso)
        {
            ContactPoint2D contacto = collision.contacts[0];
            if (contacto.normal.y < 0.7f) // No es una pisada desde arriba
            {
                IniciarRetroceso(contacto.normal);
            }
        }
    }
    #endregion

    #region DEBUG
    private void OnDrawGizmosSelected()
    {
        // Rangos de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoPersecucion);

        // Dirección actual
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector2.right * direccion * 1f);

        // Estado actual
        GUIStyle style = new GUIStyle();
        style.normal.textColor = estadoActual == EstadoCaveman.Persiguiendo ? Color.red : Color.white;
        style.fontSize = 12;
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, estadoActual.ToString(), style);
        #endif
    }
    #endregion
}