using UnityEngine;

public class bomba_script : MonoBehaviour
{
    public float Speed;
    [Header("Variables")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 Direccion;
    private float timer;
    [Header("booleanos")]
    [SerializeField] private bool hasLanded = false;
    private bool haExplotado = false;
    private bool parpadeando = false;

    [Header("explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDuration = 1.0f;
    [SerializeField] private float tiempoTotal = 3.0f;
    [SerializeField] private float inicioParpadeo = 1f;

    [Header("Daño bomba")]
    [SerializeField] private float radioExplosion = 2f;
    [SerializeField] private int danoBomba = 10;
    [SerializeField] private LayerMask layersAfectados;

    [Header("Destrucción Tilemap")]
    [SerializeField] private DestruccionTilemap[] destructoresTilemaps;

    [Header("Animator")]
    private Animator animator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        destructoresTilemaps = FindObjectsOfType<DestruccionTilemap>();
        Invoke("Explotar", tiempoTotal);
        Invoke("ComenzarParpadeo", tiempoTotal - inicioParpadeo);
    }

     void Update()
    {
        if (haExplotado) return;
        
        timer += Time.deltaTime;
        
        if (!parpadeando && timer >= tiempoTotal - inicioParpadeo)
        {
            parpadeando = true;
            animator.SetBool("IsWarning", true);
        }
        
        if (timer >= tiempoTotal)
        {
            Explotar();
        }
    }
    private void FixedUpdate()
    {
        if (hasLanded)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    public void SetDireccion(Vector3 direccion)
    {
        Direccion = direccion;
        if (rb != null)
        {
            rb.AddForce(Direccion * Speed, ForceMode2D.Impulse);
        }
    }
    void ComenzarParpadeo()
    {
        if (!haExplotado)
        {
            //animator.SetTrigger("Warning");
        }
    }
    public void Explotar()
    {
        if (haExplotado) return;
        haExplotado = true;

        AplicarDanioEnArea();

        if (destructoresTilemaps != null && destructoresTilemaps.Length > 0)
        {
            foreach (DestruccionTilemap destructor in destructoresTilemaps)
            {
                if (destructor != null)
                {
                    destructor.CrearHuecoExplosion(transform.position, radioExplosion);
                }
            }
        }
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, explosionDuration);
        Destroy(gameObject);
    }
    
    private void AplicarDanioEnArea()
    {   
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radioExplosion, layersAfectados);
        
        foreach (Collider2D collider in colliders)
        {
            // Evitar que la bomba se dañe a sí misma
            if (collider.gameObject == gameObject) continue;
            
            if (collider.CompareTag("enemy"))
            {
                MonoBehaviour[] scripts = collider.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    // Usar reflexión para encontrar método RecibirDanio
                    var metodo = script.GetType().GetMethod("RecibirDaño");
                    if (metodo != null)
                    {
                        metodo.Invoke(script, new object[] { danoBomba });
                        break;
                    }
                }
            }
            else if (collider.CompareTag("Player"))
            {
                Personaje_movimiento jugador = collider.GetComponent<Personaje_movimiento>();
                if (jugador != null)
                {
                    jugador.RecibirDaño(5, transform.position); 
                }
            }
            else if (collider.CompareTag("cajaTrampa"))
            {
                DestruirTrampa(collider.gameObject);
            }
        }
        
    }
     private void DestruirTrampa(GameObject trampa)
    {
        Destroy(trampa);
    }
    // En el script de las bombas, agrega:
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            hasLanded = true;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}
