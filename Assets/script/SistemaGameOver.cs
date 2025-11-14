using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controla la pantalla de Game Over
/// Muestra estadísticas, maneja botones de reinicio/salida
/// </summary>
public class SistemaGameOver : MonoBehaviour
{
    // === SINGLETON (solo una instancia) ===
    public static SistemaGameOver Instancia { get; private set; }
    
    [Header("Referencias UI")]
    [Tooltip("Panel completo de Game Over (debe estar desactivado al inicio)")]
    public GameObject panelGameOver;
    
    [Tooltip("Texto que muestra el nivel alcanzado")]
    public TextMeshProUGUI textoNivel;
    
    [Tooltip("Texto que muestra el dinero recolectado")]
    public TextMeshProUGUI textoDinero;
    
    [Tooltip("Texto que muestra el tiempo jugado")]
    public TextMeshProUGUI textoTiempo;
    
    [Header("Configuración")]
    [Tooltip("Nombre de la escena del menú principal")]
    public string nombreEscenaMenu = "MenuPrincipal";
    
    // === Variables privadas ===
    private bool gameOverActivo = false;
    
    void Awake()
    {
        // Implementar Singleton
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Asegurar que el panel esté oculto al inicio
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }
        
        gameOverActivo = false;
    }
    
    /// <summary>
    /// Muestra la pantalla de Game Over con las estadísticas finales
    /// Este método debe ser llamado desde el script de vida cuando el jugador muere
    /// </summary>
    public void MostrarGameOver(int nivelActual, int dineroTotal, float tiempoJugado)
    {
        if (gameOverActivo)
            return; // Evitar mostrar múltiples veces
        
        Debug.Log("=== GAME OVER ===");
        
        gameOverActivo = true;
        
        // Pausar el juego
        Time.timeScale = 0f;
        
        // Actualizar textos con estadísticas
        ActualizarEstadisticas(nivelActual, dineroTotal, tiempoJugado);
        
        // Mostrar panel
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
        else
        {
            Debug.LogError("¡PanelGameOver no está asignado en el Inspector!");
        }
    }
    
    /// <summary>
    /// Actualiza los textos de estadísticas en la UI
    /// </summary>
    void ActualizarEstadisticas(int nivel, int dinero, float tiempo)
    {
        // Actualizar nivel
        if (textoNivel != null)
        {
            if (nivel > 0)
                textoNivel.text = "Nivel: " + nivel;
            else
                textoNivel.text = "Nivel: --";
        }
        
        // Actualizar dinero
        if (textoDinero != null)
        {
            textoDinero.text = "Dinero: $ " + dinero.ToString();
        }
        
        // Actualizar tiempo (formato MM:SS)
        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempo / 60f);
            int segundos = Mathf.FloorToInt(tiempo % 60f);
            textoTiempo.text = string.Format("Tiempo: {0:00}:{1:00}", minutos, segundos);
        }
    }
    
    /// <summary>
    /// Reinicia el nivel actual desde el principio
    /// Llamado desde el botón "Volver a jugar"
    /// </summary>
    public void VolverAJugar()
    {
        Debug.Log("Reiniciando nivel...");
        
        // Restaurar tiempo
        Time.timeScale = 1f;
        
        // Recargar escena actual
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }
    
    /// <summary>
    /// Vuelve al menú principal
    /// Llamado desde el botón "Volver al menú"
    /// </summary>
    public void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú principal...");
        
        // Restaurar tiempo
        Time.timeScale = 1f;
        
        // Cargar menú
        if (EscenaExiste(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogError("La escena '" + nombreEscenaMenu + "' no existe en Build Settings!");
        }
    }
    
    /// <summary>
    /// Cierra el juego completamente
    /// Llamado desde el botón "Salir del juego"
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        
        // Restaurar tiempo
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Verifica si una escena existe en Build Settings
    /// </summary>
    bool EscenaExiste(string nombreEscena)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string rutaEscena = SceneUtility.GetScenePathByBuildIndex(i);
            string nombre = System.IO.Path.GetFileNameWithoutExtension(rutaEscena);
            
            if (nombre == nombreEscena)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Propiedad pública para saber si Game Over está activo
    /// </summary>
    public bool EstaGameOverActivo()
    {
        return gameOverActivo;
    }
}