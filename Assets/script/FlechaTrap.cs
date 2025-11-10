using UnityEngine;

public class FlechaTrap : MonoBehaviour
{
    // Asigna el Prefab de la flecha desde el Inspector
    public GameObject flechaPrefab; 
    
    // Punto desde donde se lanza la flecha (puede ser este mismo objeto)
    public Transform puntoDeLanzamiento; 
    
    public float tiempoEntreDisparos = 2f;
    private float tiempoSiguienteDisparo;
    
    // Dirección en la que se lanzará la flecha
    public Vector2 direccionDeLanzamiento = Vector2.left; 

    // Almacenamos si el jugador está en el área
    private bool jugadorDetectado = false; 

    private void Start()
    {
        tiempoSiguienteDisparo = Time.time;
    }

    private void Update()
    {
        // Si el jugador está detectado Y es tiempo de disparar
        if (jugadorDetectado && Time.time >= tiempoSiguienteDisparo)
        {
            LanzarFlecha();
            tiempoSiguienteDisparo = Time.time + tiempoEntreDisparos;
        }
    }

    // Usamos el trigger del Collider grande/playerCheck
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDetectado = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorDetectado = false;
        }
    }

    void LanzarFlecha()
    {
        // 1. Instancia la flecha desde el prefab
        GameObject nuevaFlecha = Instantiate(flechaPrefab, puntoDeLanzamiento.position, Quaternion.identity);
        
        // 2. Opcional: Rotar la flecha para que apunte en la dirección
        // (Asegúrate de que el sprite de la flecha esté diseñado para rotar correctamente)
        float angulo = Mathf.Atan2(direccionDeLanzamiento.y, direccionDeLanzamiento.x) * Mathf.Rad2Deg;
        nuevaFlecha.transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        
        // 3. Pasa la dirección al script de la flecha para que se mueva
        if (nuevaFlecha.TryGetComponent(out ProyectilFlecha flechaScript))
        {
            flechaScript.SetDirection(direccionDeLanzamiento);
        }
    }
}