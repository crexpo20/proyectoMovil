using UnityEngine;

public class Latigo : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float duracionLatigo = 4f;
    [SerializeField] private float radioAtaque = 1.5f;
    [SerializeField] private int danioLatigo = 1;
    [SerializeField] private LayerMask enemigoLayer;
    
    private Animator animator;
    private bool haGolpeado = false;
    private Vector2 direccionAtaque;

    void Start()
    {
        animator = GetComponent<Animator>();

        Destroy(gameObject, duracionLatigo);
    }
    void Update()
    {
        if (!haGolpeado)
        {
            DetectarEnemigos();
        }
    }

    public void Configurar(Vector2 direccion, Vector2 posicionPersonaje)
    {
        direccionAtaque = direccion;
        
        PosicionarLatigo(posicionPersonaje, direccion);
    }
    private void PosicionarLatigo(Vector2 posicionPersonaje, Vector2 direccion)
    {
        float offsetX = 0.8f; 
        float offsetY = -0.2f; 
        
        Vector2 posicionFinal = posicionPersonaje + new Vector2(direccion.x * offsetX, offsetY);
        
        transform.position = posicionFinal;
        
        if (direccion.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        
    }
    private void DetectarEnemigos()
    {
        Vector2 origen = transform.position;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origen, radioAtaque, direccionAtaque, 0f, enemigoLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("enemy"))
            {
                GolpearEnemigo(hit.collider);
            }
        }
    }


    private void GolpearEnemigo(Collider2D colliderEnemigo)
    {
        if (haGolpeado) return;
        MuertePorPisada enemigo = colliderEnemigo.GetComponent<MuertePorPisada>();

        if (enemigo != null && !enemigo.EstaMuerto())
        {
            enemigo.RecibirDañoExterno(danioLatigo);
            haGolpeado = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (haGolpeado) return;

        // Verificar si es un enemigo
        if (((1 << other.gameObject.layer) & enemigoLayer) != 0 || other.CompareTag("enemy"))
        {
            GolpearEnemigo(other);
        }
    }

    // ------Debug visual---------
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.yellow;
        Vector2 puntoInicio = (Vector2)transform.position + new Vector2(direccionAtaque.x * 0.3f, 0);
        Vector2 puntoFinal = puntoInicio + direccionAtaque * radioAtaque;
        
        Gizmos.DrawLine(puntoInicio, puntoFinal);
        Gizmos.DrawWireSphere(puntoInicio + direccionAtaque * (radioAtaque / 2f), radioAtaque / 2f);
    }
}