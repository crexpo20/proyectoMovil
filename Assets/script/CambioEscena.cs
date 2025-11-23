using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    [SerializeField] private string nombrePuntoSpawn = "entrada";
    
    public void CambiarEscena(string nombreEscena)
    {
        PlayerPrefs.SetString("PuntoSpawnDeseado", nombrePuntoSpawn);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(nombreEscena);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CambiarEscena("NombreDeTuEscena");
        }
    }
}