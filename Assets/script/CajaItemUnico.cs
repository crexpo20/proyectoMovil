using UnityEngine;

public class CajaItemUnico : MonoBehaviour
{
    [Header("Item Único")]
    [SerializeField] private GameObject[] itemsPosibles;
    
    [Header("Configuración")]
    [SerializeField] private float radioDispersion = 0.5f;
    [SerializeField] private bool siempreDaItem = true;
    
    [Header("Probabilidades (si siempreDaItem = false)")]
    [SerializeField] [Range(0, 100)] private int chanceItem = 80;
    [SerializeField] [Range(0, 100)] private int chanceVacio = 20;

    [Header("Efectos (Opcional)")]
    [SerializeField] private GameObject efectoDestruccion;
    [SerializeField] private AudioClip sonidoDestruccion;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("weapon"))
        {
            RomperCaja();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("weapon"))
        {
            RomperCaja();
        }
    }

    private void RomperCaja()
    {
        GenerarItemUnico();
        MostrarEfectos();
        Destroy(gameObject);
    }

    private void GenerarItemUnico()
    {
        // Decidir si genera item o no
        if (!siempreDaItem)
        {
            int random = Random.Range(0, 100);
            if (random >= chanceItem)
            {
                Debug.Log("Caja vacía");
                return;
            }
        }

        // Verificar que hay items disponibles
        if (itemsPosibles.Length == 0)
        {
            Debug.LogWarning("No hay items configurados en la caja");
            return;
        }

        // Seleccionar un item aleatorio
        int index = Random.Range(0, itemsPosibles.Length);
        GameObject itemSeleccionado = itemsPosibles[index];
        
        // Calcular posición con pequeña dispersión
        Vector3 posicion = transform.position + (Vector3)Random.insideUnitCircle * radioDispersion;
        
        // Crear el item
        Instantiate(itemSeleccionado, posicion, Quaternion.identity);
        
        Debug.Log($"Item generado: {itemSeleccionado.name}");
    }

    private void MostrarEfectos()
    {
        // Efecto visual de destrucción
        if (efectoDestruccion != null)
        {
            Instantiate(efectoDestruccion, transform.position, Quaternion.identity);
        }
        
        // Sonido de destrucción
        if (sonidoDestruccion != null)
        {
            AudioSource.PlayClipAtPoint(sonidoDestruccion, transform.position);
        }
    }

    // Visualización en el Editor
    private void OnDrawGizmosSelected()
    {
        // Mostrar radio de dispersión
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDispersion);
        
        // Mostrar área de la caja
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
    }
}