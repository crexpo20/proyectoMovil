using UnityEngine;

public class ProyectilFlecha : MonoBehaviour
{
    public float velocidad = 10f;
    public float tiempoDeVidaMax = 5f; // Desaparece después de este tiempo si no choca
    public int dañoCausado = 1; // El daño que la flecha inflige

    private Rigidbody2D rb;
    private Vector2 direccion;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destruye el objeto después de 'tiempoDeVidaMax' si no ha chocado
        Destroy(gameObject, tiempoDeVidaMax); 
    }

    // Llamado por el script del emisor para iniciar el movimiento
    public void SetDirection(Vector2 dir)
    {
        direccion = dir.normalized;
        rb.linearVelocity = direccion * velocidad; 
    }

    // Maneja la colisión con un Collider NO-trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Colisiona con el Player (¡Causa daño!)
        if (collision.gameObject.CompareTag("Player"))
        {
            // OBTENEMOS EL SCRIPT DE MOVIMIENTO DEL JUGADOR
            Personaje_movimiento jugador = collision.gameObject.GetComponent<Personaje_movimiento>();
            
            if (jugador != null)
            {
                // Llamamos directamente al método de daño en el script de movimiento del jugador
                // Usamos la posición de la flecha (transform.position) como la fuente del golpe
                jugador.RecibirDaño(dañoCausado, transform.position); 
            }
            
            // La flecha debe desaparecer siempre al golpear al jugador
            Destroy(gameObject);
        }
        // 2. Colisiona con el Ground o cualquier otra cosa que deba detenerla
        // Puedes usar Tags como "Ground" o Layers
        else if (collision.gameObject.CompareTag("Ground")) 
        {
            // Desaparece al chocar con el suelo
            Destroy(gameObject); 
        }
        // 3. Colisión con cualquier otra cosa (enemigos, paredes, etc.)
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
             // Desaparece al chocar con el entorno
            Destroy(gameObject); 
        }
        
    }
}