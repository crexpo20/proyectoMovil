using UnityEngine;

public class SapoPatrullaje : MonoBehaviour
{
    [Header("Configuración de Salto")]
    [SerializeField] private float fuerzaSalto = 6f;      
    [SerializeField] private float fuerzaHorizontal = 2f; 
    [SerializeField] private float tiempoEntreSaltos = 2f;

    [Header("Detección de Suelo y Paredes")]
    public Transform groundCheckOrigin;
    public Transform wallCheckOrigin;
    public float groundCheckDistance = 0.2f;
    public float wallCheckDistance = 0.2f;
    public LayerMask groundLayer;

    // --- Componentes ---
    private Rigidbody2D rb;
    private Animator anim;

    // --- Variables de control ---
    private float proximoSalto = 0f;
    private int direccion = 1;
    //---- Variables booleanas----
    private bool enSuelo;
    private bool estabaEnSuelo;
    private bool necesitaGirar = false;
    private bool estaSaltando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (rb == null) Debug.LogError("El Sapo necesita un Rigidbody2D.");
        estabaEnSuelo = true;
    }
    void Update()
    {
        ActualizarAnimaciones();
        if (!enSuelo && estaSaltando)
        {
            VerificarObstaculosEnAire();
        }
    }

    void FixedUpdate()
    {
        estabaEnSuelo = enSuelo;
        enSuelo = Physics2D.Raycast(groundCheckOrigin.position, Vector2.down, groundCheckDistance, groundLayer);

        // Si acaba de tocar el suelo
        if (enSuelo && !estabaEnSuelo)
        {
            estaSaltando = false;
        }

        if (enSuelo && Time.time >= proximoSalto)
        {
            // Si necesita girar, hacerlo antes de saltar
            if (necesitaGirar)
            {
                Girar();
                necesitaGirar = false;
            }
            
            Saltar();
            proximoSalto = Time.time + tiempoEntreSaltos;
        }
    }
    void VerificarObstaculos()
    {
        // Verificar si hay pared adelante
        bool paredAdelante = Physics2D.Raycast(wallCheckOrigin.position,Vector2.right * direccion,wallCheckDistance, groundLayer);

        // Verificar si hay suelo adelante (para no caer al vacío)
        Vector2 checkSueloPos = groundCheckOrigin.position + Vector3.right * 0.6f * direccion;
        bool sueloAdelante = Physics2D.Raycast(checkSueloPos,Vector2.down,groundCheckDistance + 0.3f,groundLayer);

        // Debug rays
        Debug.DrawRay(groundCheckOrigin.position, Vector2.down * groundCheckDistance, enSuelo ? Color.green : Color.red);
        Debug.DrawRay(wallCheckOrigin.position, Vector2.right * direccion * wallCheckDistance, paredAdelante ? Color.red : Color.blue);
        Debug.DrawRay(checkSueloPos, Vector2.down * (groundCheckDistance + 0.3f), sueloAdelante ? Color.green : Color.yellow);

        // Marcar que necesita girar si encuentra obstáculo
        if (paredAdelante || !sueloAdelante)
        {
            necesitaGirar = true;
        }
    }

    void VerificarObstaculosEnAire()
    {
        // Verificar si hay pared adelante durante el salto
        bool paredAdelante = Physics2D.Raycast(wallCheckOrigin.position, Vector2.right * direccion, wallCheckDistance, groundLayer);

        // Debug ray para pared en aire
        Debug.DrawRay(wallCheckOrigin.position, Vector2.right * direccion * wallCheckDistance, 
                     paredAdelante ? Color.magenta : Color.cyan);

        // Si encuentra pared durante el salto, cambiar dirección inmediatamente
        if (paredAdelante)
        {
            Girar();
            
            // Ajustar la velocidad horizontal para cambiar dirección inmediatamente
            Vector2 nuevaVelocidad = rb.linearVelocity;
            nuevaVelocidad.x = fuerzaHorizontal * direccion;
            rb.linearVelocity = nuevaVelocidad;
            
        }
    }

    void ActualizarAnimaciones()
    {
        if (anim == null) return;

        // Actualizar parámetro básico de suelo
        anim.SetBool("enSuelo", enSuelo);

        // Detectar cuando ACABA de tocar el suelo (aterriza)
        if (enSuelo && !estabaEnSuelo)
        {
            anim.SetTrigger("Aterrizar");
        }

        // Detectar cuando ACABA de dejar el suelo (inicia salto)
        if (!enSuelo && estabaEnSuelo)
        {
            anim.SetTrigger("Saltar");
        }
    }

    
    void Saltar()
    {
        // Aplicar fuerza de salto
        rb.linearVelocity = new Vector2(fuerzaHorizontal * direccion, fuerzaSalto);
        estaSaltando = true;
    }

   void Girar()
    {
        direccion *= -1;
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
        transform.localScale = escala;
    }

     public void ForzarGiro()
    {
        necesitaGirar = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si chocó con una pared lateral durante el salto
        if (!enSuelo && collision.gameObject.CompareTag("ground"))
        {
            // Verificar si fue una colisión lateral
            ContactPoint2D contact = collision.contacts[0];
            if (Mathf.Abs(contact.normal.x) > 0.7f) // Colisión lateral
            {
                Girar();
                
                // Ajustar velocidad para rebotar
                Vector2 nuevaVelocidad = rb.linearVelocity;
                nuevaVelocidad.x = fuerzaHorizontal * direccion;
                rb.linearVelocity = nuevaVelocidad;
                
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Gizmo para detección de suelo
        Gizmos.color = enSuelo ? Color.green : Color.red;
        if (groundCheckOrigin != null)
            Gizmos.DrawLine(groundCheckOrigin.position, groundCheckOrigin.position + Vector3.down * groundCheckDistance);

        // Gizmo para detección de pared
        Gizmos.color = Color.blue;
        if (wallCheckOrigin != null)
            Gizmos.DrawLine(wallCheckOrigin.position, wallCheckOrigin.position + Vector3.right * direccion * wallCheckDistance);

        // Gizmo para verificar suelo adelante
        Gizmos.color = Color.cyan;
        if (groundCheckOrigin != null)
        {
            Vector3 checkSueloPos = groundCheckOrigin.position + Vector3.right * 0.6f * direccion;
            Gizmos.DrawLine(checkSueloPos, checkSueloPos + Vector3.down * (groundCheckDistance + 0.3f));
        }

        // Gizmo adicional para detección en aire (color diferente)
        if (!enSuelo && wallCheckOrigin != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(wallCheckOrigin.position, wallCheckOrigin.position + Vector3.right * direccion * wallCheckDistance);
        }
    }
}