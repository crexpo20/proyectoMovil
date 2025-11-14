using UnityEngine;
using System.Collections;

public class EspadaTrap : MonoBehaviour
{
    [Header("Configuración de Espadas")]
    public GameObject spriteEspadas; // Solo UN sprite con las tres espadas
    public float tiempoActivacion = 0.5f;
    public float tiempoActivas = 1.5f;
    public float tiempoReactivacion = 2f;
    
    [Header("Daño")]
    public int dañoCausado = 1;
    
    [Header("Posiciones")]
    public Vector3 posicionOcultas = Vector3.zero;
    public Vector3 posicionActivadas = new Vector3(0f, 0.5f, 0f);

    private bool jugadorEnArea = false;
    private bool espadasActivas = false;
    private bool puedeActivar = true;
    private Coroutine corutinaActivacion;

    private void Start()
    {
        // Ocultar espadas al inicio
        if (spriteEspadas != null)
        {
            spriteEspadas.transform.localPosition = posicionOcultas;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puedeActivar && !espadasActivas)
        {
            jugadorEnArea = true;
            ActivarTrampa();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puedeActivar && !espadasActivas)
        {
            jugadorEnArea = true;
            // Solo activar si no hay una corutina en curso
            if (corutinaActivacion == null)
            {
                ActivarTrampa();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnArea = false;
        }
    }

    private void ActivarTrampa()
    {
        if (corutinaActivacion == null)
        {
            corutinaActivacion = StartCoroutine(CicloActivacion());
        }
    }

    private IEnumerator CicloActivacion()
    {
        puedeActivar = false;
        
        // 1. Las espadas salen
        yield return StartCoroutine(MoverEspadas(posicionOcultas, posicionActivadas, tiempoActivacion));
        
        espadasActivas = true;
        
        // 2. Verificar si el jugador sigue en el área para causar daño
        if (jugadorEnArea)
        {
            CausarDanioAlJugador();
        }
        
        // 3. Esperar tiempo con espadas activas
        yield return new WaitForSeconds(tiempoActivas);
        
        // 4. Las espadas se retraen
        yield return StartCoroutine(MoverEspadas(posicionActivadas, posicionOcultas, tiempoActivacion));
        
        espadasActivas = false;
        
        // 5. Esperar tiempo de reactivación
        yield return new WaitForSeconds(tiempoReactivacion);
        
        puedeActivar = true;
        corutinaActivacion = null;
        
        // 6. Si el jugador todavía está en el área, reactivar
        if (jugadorEnArea && puedeActivar)
        {
            ActivarTrampa();
        }
    }

    private IEnumerator MoverEspadas(Vector3 desde, Vector3 hasta, float duracion)
    {
        float tiempoTranscurrido = 0f;
        
        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracion;
            
            // Suavizado del movimiento
            t = t * t * (3f - 2f * t);
            
            Vector3 posicionActual = Vector3.Lerp(desde, hasta, t);
            
            if (spriteEspadas != null)
            {
                spriteEspadas.transform.localPosition = posicionActual;
            }
            
            yield return null;
        }
        
        // Asegurar posición final exacta
        if (spriteEspadas != null)
        {
            spriteEspadas.transform.localPosition = hasta;
        }
    }

    private void CausarDanioAlJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            Personaje_movimiento jugadorScript = jugador.GetComponent<Personaje_movimiento>();
            if (jugadorScript != null)
            {
                jugadorScript.RecibirDaño(dañoCausado, transform.position);
            }
        }
    }

    // Método para debug visual en el Editor
    private void OnDrawGizmosSelected()
    {
        if (spriteEspadas != null)
        {
            // Dibujar posición de espadas ocultas
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + posicionOcultas, new Vector3(1f, 0.1f, 1f));
            
            // Dibujar posición de espadas activadas
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + posicionActivadas, new Vector3(1f, 0.1f, 1f));
        }
        
        // Dibujar área de detección
        Gizmos.color = Color.yellow;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)collider.offset, collider.bounds.size);
        }
    }
}