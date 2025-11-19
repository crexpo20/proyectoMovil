using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    [SerializeField] private string nombrePuntoSpawn = "entrada";
    
    // Llamar este método desde un botón UI o trigger
    public void CambiarEscena(string nombreEscena)
    {
        // Guardar el punto de spawn deseado antes de cambiar de escena
        PlayerPrefs.SetString("PuntoSpawnDeseado", nombrePuntoSpawn);
        PlayerPrefs.Save();
        
        // Cargar la nueva escena
        SceneManager.LoadScene(nombreEscena);
    }

    // Método alternativo para usar en colisiones
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CambiarEscena("NombreDeTuEscena"); // Cambia por el nombre de tu escena
        }
    }
}