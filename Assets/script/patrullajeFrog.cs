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
    private int direccion = 1; // 1 = derecha, -1 = izquierda
    //---- Variables booleanas----
    private bool enSuelo;
    private bool estabaEnSuelo;
    private bool necesitaGirar = false;

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
    }

    void FixedUpdate()
    {
        estabaEnSuelo = enSuelo;

        enSuelo = Physics2D.Raycast(groundCheckOrigin.position, Vector2.down, groundCheckDistance, groundLayer);
        
        if (enSuelo)
        {
            VerificarObstaculos();
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
    }

   void Girar()
    {
        direccion *= -1;
        Vector3 escala = transform.localScale;
        escala.x = Mathf.Abs(escala.x) * direccion;
        transform.localScale = escala;
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
    }
}