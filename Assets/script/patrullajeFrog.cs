using UnityEngine;

public class SapoPatrullaje : MonoBehaviour
{
    [Header("Configuración de Salto")]
    [SerializeField] private float fuerzaSalto = 6f;      // Qué tan alto salta
    [SerializeField] private float fuerzaHorizontal = 2f; // Qué tanto avanza en el salto
    [SerializeField] private float tiempoEntreSaltos = 2f; // Cada cuántos segundos salta

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
    private bool enSalto;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (rb == null) Debug.LogError("El Sapo necesita un Rigidbody2D.");
    }

    void FixedUpdate()
    {
        // Verificar si está en el suelo
        enSuelo = Physics2D.Raycast(groundCheckOrigin.position, Vector2.down, groundCheckDistance, groundLayer);
        if(enSuelo == true)
        {
            anim.SetBool("enSalto",false);
        }

        // Detección de pared o vacío
        bool paredAdelante = Physics2D.Raycast(wallCheckOrigin.position, Vector2.right * direccion, wallCheckDistance, groundLayer);
        bool sueloAdelante = Physics2D.Raycast(groundCheckOrigin.position + Vector3.right * 0.2f * direccion, Vector2.down, groundCheckDistance, groundLayer);

        Debug.DrawRay(groundCheckOrigin.position, Vector2.down * groundCheckDistance, Color.green);
        Debug.DrawRay(wallCheckOrigin.position, Vector2.right * direccion * wallCheckDistance, Color.blue);

        if (paredAdelante || !sueloAdelante)
        {
            //Girar();
        }

        // Si está en suelo y es momento de saltar
        if (enSuelo && Time.time >= proximoSalto)
        {
            Saltar();
            proximoSalto = Time.time + tiempoEntreSaltos;
        }
    }

    void Saltar()
    {
        // Reinicia velocidad vertical para saltos más controlados
        rb.linearVelocity = new Vector2(0, 0);
        // Aplica impulso de salto
        rb.AddForce(new Vector2(fuerzaHorizontal * direccion, fuerzaSalto), ForceMode2D.Impulse);

        // Activar animación si existe
        if (anim != null)
        {
            anim.SetBool("enSalto",true);
        }
    }

    void Girar()
    {
        direccion *= -1;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja rayos en el editor para depuración
        Gizmos.color = Color.yellow;
        if (groundCheckOrigin != null)
            Gizmos.DrawLine(groundCheckOrigin.position, groundCheckOrigin.position + Vector3.down * groundCheckDistance);
        if (wallCheckOrigin != null)
            Gizmos.DrawLine(wallCheckOrigin.position, wallCheckOrigin.position + Vector3.right * wallCheckDistance);
    }
}