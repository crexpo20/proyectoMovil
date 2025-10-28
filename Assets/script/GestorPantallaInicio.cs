using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controla la pantalla de inicio/splash screen
/// Detecta cualquier input (tecla, mouse, toque) y carga el menú principal
/// </summary>
public class GestorPantallaInicio : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Nombre de la escena del menú principal")]
    public string nombreEscenaMenu = "MenuPrincipal";
    
    [Tooltip("Tiempo mínimo antes de poder continuar (segundos)")]
    [Range(0f, 3f)]
    public float tiempoEsperaMinimo = 1f;
    
    [Tooltip("Usar efecto de fade out al cambiar de escena")]
    public bool usarFadeOut = true;
    
    [Tooltip("Duración del fade out (segundos)")]
    [Range(0.1f, 2f)]
    public float duracionFadeOut = 0.5f;
    
    [Header("Referencias (Opcional)")]
    [Tooltip("Panel negro para fade out (opcional)")]
    public CanvasGroup panelFade;
    
    // Variables privadas
    private bool puedeAvanzar = false;
    private bool estaTransicionando = false;
    private float tiempoTranscurrido = 0f;
    
    void Start()
    {
        // Inicializar panel de fade si existe
        if (panelFade != null)
        {
            panelFade.alpha = 0f;
        }
    }
    
    void Update()
    {
        // Incrementar tiempo
        tiempoTranscurrido += Time.deltaTime;
        
        // Habilitar avance después del tiempo mínimo
        if (!puedeAvanzar && tiempoTranscurrido >= tiempoEsperaMinimo)
        {
            puedeAvanzar = true;
            Debug.Log("Ya puedes presionar cualquier tecla para continuar");
        }
        
        // Detectar input solo si puede avanzar y no está transicionando
        if (puedeAvanzar && !estaTransicionando)
        {
            if (DetectarCualquierInput())
            {
                IrAlMenuPrincipal();
            }
        }
    }
    
    /// <summary>
    /// Detecta cualquier tipo de input: teclado, mouse o toque táctil
    /// </summary>
    /// <returns>True si se detectó algún input</returns>
    bool DetectarCualquierInput()
    {
        // Detectar cualquier tecla
        if (Input.anyKeyDown)
        {
            Debug.Log("Tecla presionada detectada");
            return true;
        }
        
        // Detectar click de mouse (para PC)
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Debug.Log("Click de mouse detectado");
            return true;
        }
        
        // Detectar toque táctil (para móvil)
        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);
            if (toque.phase == TouchPhase.Began)
            {
                Debug.Log("Toque táctil detectado");
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Inicia la transición al menú principal
    /// </summary>
    void IrAlMenuPrincipal()
    {
        estaTransicionando = true;
        Debug.Log("Cargando menú principal...");
        
        if (usarFadeOut && panelFade != null)
        {
            // Cargar con fade out
            StartCoroutine(TransicionConFade());
        }
        else
        {
            // Cargar directo
            CargarMenuPrincipal();
        }
    }
    
    /// <summary>
    /// Corrutina para fade out suave
    /// </summary>
    IEnumerator TransicionConFade()
    {
        float tiempoInicio = Time.time;
        
        // Fade out (negro aparece gradualmente)
        while (Time.time < tiempoInicio + duracionFadeOut)
        {
            float progreso = (Time.time - tiempoInicio) / duracionFadeOut;
            panelFade.alpha = progreso;
            yield return null;
        }
        
        panelFade.alpha = 1f;
        
        // Cargar escena
        CargarMenuPrincipal();
    }
    
    /// <summary>
    /// Carga la escena del menú principal
    /// </summary>
    void CargarMenuPrincipal()
    {
        if (EscenaExiste(nombreEscenaMenu))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
        else
        {
            Debug.LogError("¡La escena '" + nombreEscenaMenu + "' no existe en Build Settings!");
            Debug.LogError("Ve a File → Build Settings y agrega la escena MenuPrincipal");
        }
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
    /// Método público para omitir la pantalla (útil para testing)
    /// </summary>
    public void OmitirPantallaInicio()
    {
        if (!estaTransicionando)
        {
            IrAlMenuPrincipal();
        }
    }
}