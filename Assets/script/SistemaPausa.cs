using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Sistema completo de pausa del juego
/// Maneja: pausar tiempo, mostrar menú, detectar ESC, botones de pausa
/// </summary>
public class SistemaPausa : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Panel completo del menú de pausa (GameObject con todos los elementos)")]
    public GameObject panelPausa;
    
    [Tooltip("Botón de pausa visible en el juego (opcional, para móvil)")]
    public Button botonPausaUI;
    
    [Header("Configuración")]
    [Tooltip("Permitir pausar con tecla ESC")]
    public bool permitirTeclaESC = true;
    
    [Tooltip("Nombre de la escena del menú principal")]
    public string nombreEscenaMenuPrincipal = "MenuPrincipal";
    
    // Estado del juego
    private bool estaPausado = false;
    
    /// <summary>
    /// Propiedad pública para que otros scripts sepan si está pausado
    /// </summary>
    public bool EstaPausado { get { return estaPausado; } }
    
    void Start()
    {
        // Asegurar que el panel esté oculto al inicio
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
        
        // Configurar botón de pausa si existe
        if (botonPausaUI != null)
        {
            botonPausaUI.onClick.AddListener(TogglePausa);
        }
        
        // Asegurar que el tiempo esté corriendo
        Time.timeScale = 1f;
        estaPausado = false;
    }
    
    void Update()
    {
        // Detectar tecla ESC (solo en PC)
        if (permitirTeclaESC && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }
    
    /// <summary>
    /// Alterna entre pausar y reanudar el juego
    /// </summary>
    public void TogglePausa()
    {
        if (estaPausado)
        {
            Reanudar();
        }
        else
        {
            Pausar();
        }
    }
    
    /// <summary>
    /// Pausa el juego (congela tiempo y muestra menú)
    /// </summary>
    public void Pausar()
    {
        Debug.Log("Juego pausado");
        
        // Congelar el tiempo del juego
        Time.timeScale = 0f;
        
        // Mostrar panel de pausa
        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
        }
        
        // Actualizar estado
        estaPausado = true;
        
        // Opcional: Ocultar botón de pausa cuando el menú está abierto
        if (botonPausaUI != null)
        {
            botonPausaUI.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Reanuda el juego (despausa y oculta menú)
    /// Este método se llama desde el botón "Resumen"
    /// </summary>
    public void Reanudar()
    {
        Debug.Log("Juego reanudado");
        
        // Restaurar velocidad normal del tiempo
        Time.timeScale = 1f;
        
        // Ocultar panel de pausa
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
        
        // Actualizar estado
        estaPausado = false;
        
        // Mostrar botón de pausa nuevamente
        if (botonPausaUI != null)
        {
            botonPausaUI.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Vuelve al menú principal
    /// Este método se llama desde el botón "Menú Inicio"
    /// </summary>
    public void IrAlMenuPrincipal()
    {
        Debug.Log("Volviendo al menú principal...");
        
        // IMPORTANTE: Restaurar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;
        
        // Cargar escena del menú
        if (EscenaExiste(nombreEscenaMenuPrincipal))
        {
            SceneManager.LoadScene(nombreEscenaMenuPrincipal);
        }
        else
        {
            Debug.LogError("La escena '" + nombreEscenaMenuPrincipal + "' no existe en Build Settings!");
        }
    }
    
    /// <summary>
    /// Cierra el juego completamente
    /// Este método se llama desde el botón "Salir del juego"
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        
        // Restaurar tiempo por si acaso
        Time.timeScale = 1f;
        
        // En el editor de Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // En el build (Android/PC)
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Reinicia el nivel actual
    /// Método extra útil para un botón de "Reintentar"
    /// </summary>
    public void ReiniciarNivel()
    {
        Debug.Log("Reiniciando nivel...");
        
        // Restaurar tiempo
        Time.timeScale = 1f;
        
        // Recargar escena actual
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
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
    /// Limpieza al destruir el objeto
    /// </summary>
    void OnDestroy()
    {
        // Remover listener del botón
        if (botonPausaUI != null)
        {
            botonPausaUI.onClick.RemoveListener(TogglePausa);
        }
        
        // Asegurar que el tiempo se restaure
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Asegurar que el tiempo se restaure al desactivar el script
    /// </summary>
    void OnDisable()
    {
        Time.timeScale = 1f;
    }
}