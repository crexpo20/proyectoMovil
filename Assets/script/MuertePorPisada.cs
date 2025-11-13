using UnityEngine;
using UnityEngine.Events;

public class MuertePorPisada : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int vidaMaxima = 1;
    [SerializeField] private int vidaActual;

    [Header("Configuración de Muerte por Pisada")]
    [SerializeField] private float fuerzaRebote = 8f;
    [SerializeField] private LayerMask capaJugador;
    [SerializeField] private float umbralPisada = 0.9f; // Más sensible para detectar desde arriba
    [SerializeField] private int dañoPisada = 1;

     [Header("Configuración de Daño por Látigo")]
    [SerializeField] private LayerMask capaLatigo;
    [SerializeField] private int dañoLatigo = 1;

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
    //------------ colisiones ------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcesarColision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // También detectar si el jugador se mantiene encima
        ProcesarColision(collision);
    }
     private void OnTriggerEnter2D(Collider2D other)
    {
        ProcesarTrigger(other);
    }
    // -------------- metodos -----------------
    private void ProcesarColision(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & capaJugador) != 0 && !estaMuerta)
        {
            bool esPisada = EsPisadaDesdeArriba(collision) || DetectarJugadorEncima();

            if (esPisada)
            {
                RecibirDaño(dañoPisada);
                Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();

                if (rbJugador != null)
                {
                    rbJugador.linearVelocity = new Vector2(rbJugador.linearVelocity.x, fuerzaRebote);
                }
            }
        }
    }

    public void RecibirDaño(int cantidad)
    {
        if (estaMuerta) return;

        if (danioScript != null)
        {
            danioScript.enabled = false;
        }

        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            //se puede agregar sonido de muerte aca
        }
    }
    private void ProcesarTrigger(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & capaLatigo) != 0 && !estaMuerta)
        {
            RecibirDaño(dañoLatigo);

            // sonido para latigo


            AplicarRetroceso(other.transform.position);
        }
    }
    private void AplicarRetroceso(Vector3 posicionLatigo)
    {
        if (rb != null)
        {
            Vector2 direccion = (transform.position - posicionLatigo).normalized;
            float fuerzaRetroceso = 5f;

            rb.linearVelocity = new Vector2(direccion.x * fuerzaRetroceso, rb.linearVelocity.y);
        }
    }
    private bool EsPisadaDesdeArriba(Collision2D collision)
    {
        foreach (ContactPoint2D contacto in collision.contacts)
        {
            Vector2 direccion = (contacto.point - (Vector2)puntoSuperior.position).normalized;

            Debug.DrawRay(contacto.point, direccion * 0.5f, Color.cyan, 1f);
            
            if (direccion.y < -umbralPisada && JugadorSeMueveHaciaAbajo(collision.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    private bool DetectarJugadorEncima()
    {
        Vector2 origen = new Vector2(puntoSuperior.position.x, puntoSuperior.position.y);
        RaycastHit2D hit = Physics2D.CircleCast(origen, radioDeteccion, Vector2.up, 0.1f, capaJugador);

        Debug.DrawRay(origen, Vector2.up * 0.1f, hit.collider != null ? Color.green : Color.red, 0.1f);
        
        if (hit.collider != null && JugadorSeMueveHaciaAbajo(hit.collider.gameObject))
        {
            return true;
        }
        
        return false;
    }

    private bool JugadorSeMueveHaciaAbajo(GameObject jugador)
    {
        Rigidbody2D rbJugador = jugador.GetComponent<Rigidbody2D>();
        if (rbJugador != null)
        {
            bool seMueveHaciaAbajo = rbJugador.linearVelocity.y < -0.1f;
            return seMueveHaciaAbajo;
        }
        return false;
    }

    private void Morir()
    {
        if (estaMuerta) return;

        estaMuerta = true;

        if (patrullaje != null)
            patrullaje.enabled = false;

        if (colisionadorSerpiente != null)
            colisionadorSerpiente.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Destroy(gameObject);

    }
    public void RecibirDañoExterno(int cantidad)
    {
        RecibirDaño(cantidad);
    }

    private bool TieneParametro(Animator animator, string parametro)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parametro) return true;
        }
        return false;
    }
    public int GetVidaActual() => vidaActual;
    public int GetVidaMaxima() => vidaMaxima;
    public bool EstaMuerto() => estaMuerta;


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