using UnityEngine;

public class PuntoSalida : MonoBehaviour
{
    [Header("Configuración Salida")]
    public string nombreSiguienteEscena;
    public bool esUltimoNivel = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Player tocó la salida. Siguiente escena: {nombreSiguienteEscena}");
            GestionarSalida();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player chocó con la salida. Siguiente escena: {nombreSiguienteEscena}");
            GestionarSalida();
        }
    }

    private void GestionarSalida()
    {
        if (GestorNiveles.Instance == null)
        {
            Debug.LogError("No se encontró GestorNiveles en la escena");
            return;
        }

        if (esUltimoNivel)
        {
            Debug.Log("Es el último nivel, cargando escena final");
            GestorNiveles.Instance.CargarEscenaFinal();
        }
        else
        {
            Debug.Log($"Cargando siguiente nivel: {nombreSiguienteEscena}");
            GestorNiveles.Instance.CargarSiguienteNivel(nombreSiguienteEscena);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 2f, 1f));
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2f);
    }
}