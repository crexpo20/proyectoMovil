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
            GestionarSalida();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GestionarSalida();
        }
    }

    private void GestionarSalida()
    {
        if (GestorNiveles.Instance == null)
        {
            return;
        }

        if (esUltimoNivel)
        {
            GestorNiveles.Instance.CargarFindelJuego();
        }
        else
        {
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