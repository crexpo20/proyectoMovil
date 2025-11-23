using UnityEngine;
using UnityEngine.SceneManagement;

public class SalidaTutorial : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreMenuPrincipal = "MenuPrincipal";
    public string tagJugador = "Player";
    
    [Header("Efectos Opcionales")]
    public ParticleSystem efectoSalida;
    public AudioClip sonidoSalida;
    
    private bool salidaActivada = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!salidaActivada && other.CompareTag(tagJugador))
        {
            ActivarSalida();
        }
    }
    
    private void ActivarSalida()
    {
        salidaActivada = true;
        Debug.Log("🎮 Jugador llegó a la salida del tutorial");
        
        // Reproducir efectos
        if (efectoSalida != null)
            efectoSalida.Play();
            
        if (sonidoSalida != null)
            AudioSource.PlayClipAtPoint(sonidoSalida, transform.position);
        
        // Esperar un momento antes de cambiar de escena (para ver los efectos)
        Invoke("CompletarTutorial", 1.5f);
    }
    
    private void CompletarTutorial()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    
    private void LimpiarSistemasPersistentes()
    {
        // Destruir CanvasManager si existe
        CanvasManager canvasManager = FindObjectOfType<CanvasManager>();
        if (canvasManager != null)
        {
            Destroy(canvasManager.gameObject);
        }
        
    }
    
    // Método para debug - forzar salida con tecla
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) // Tecla para testear
        {
            ActivarSalida();
        }
    }
}