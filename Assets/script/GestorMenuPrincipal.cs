using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la navegación del menú principal
/// Maneja los botones: Jugar, Tutorial, Opciones, Salir
/// </summary>
public class GestorMenuPrincipal : MonoBehaviour
{
    [Header("Nombres de las Escenas")]
    [Tooltip("Nombre exacto de la escena del tutorial (debe estar en Build Settings)")]
    public string nombreEscenaTutorial = "Tutorial";
    
    [Tooltip("Nombre de la escena del juego principal (cuando la tengas)")]
    public string nombreEscenaJuego = "Nivel1";
    
    [Tooltip("Nombre de la escena de opciones (cuando la tengas)")]
    public string nombreEscenaOpciones = "Opciones";
    
    /// <summary>
    /// Se llama cuando el jugador presiona el botón JUGAR
    /// Carga la escena principal del juego
    /// </summary>
    public void IrAJugar()
    {
        Debug.Log("Cargando escena: " + nombreEscenaJuego);
        
        // Por ahora, si no existe la escena, carga el tutorial
        if (EscenaExiste(nombreEscenaJuego))
        {
            SceneManager.LoadScene(nombreEscenaJuego);
        }
        else
        {
            Debug.LogWarning("La escena '" + nombreEscenaJuego + "' no existe. Cargando Tutorial...");
            SceneManager.LoadScene(nombreEscenaTutorial);
        }
    }
    
    /// <summary>
    /// Se llama cuando el jugador presiona el botón TUTORIAL
    /// Carga la escena del tutorial
    /// </summary>
    public void IrATutorial()
    {
        Debug.Log("Cargando Tutorial...");
        
        if (EscenaExiste(nombreEscenaTutorial))
        {
            SceneManager.LoadScene(nombreEscenaTutorial);
        }
        else
        {
            Debug.LogError("¡La escena '" + nombreEscenaTutorial + "' no existe en Build Settings!");
        }
    }
    
    /// <summary>
    /// Se llama cuando el jugador presiona el botón OPCIONES
    /// Carga la escena de opciones (audio, controles, etc.)
    /// </summary>
    public void IrAOpciones()
    {
        Debug.Log("Abriendo Opciones...");
        
        if (EscenaExiste(nombreEscenaOpciones))
        {
            SceneManager.LoadScene(nombreEscenaOpciones);
        }
        else
        {
            Debug.LogWarning("La escena de Opciones aún no existe. Créala cuando la necesites.");
            // Por ahora no hace nada
        }
    }
    
    /// <summary>
    /// Se llama cuando el jugador presiona SALIR DEL JUEGO
    /// Cierra la aplicación
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        
        // En el editor de Unity, esto detiene el modo Play
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // En el build (celular/PC), esto cierra la aplicación
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Verifica si una escena existe en Build Settings
    /// </summary>
    /// <param name="nombreEscena">Nombre de la escena a verificar</param>
    /// <returns>True si existe, False si no</returns>
    private bool EscenaExiste(string nombreEscena)
    {
        // Buscar en todas las escenas del Build Settings
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
    /// Método opcional: Recargar la escena actual
    /// Útil para reiniciar el nivel
    /// </summary>
    public void RecargarEscenaActual()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }
    
    /// <summary>
    /// Método opcional: Volver al menú principal desde cualquier escena
    /// </summary>
    public void VolverAlMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}