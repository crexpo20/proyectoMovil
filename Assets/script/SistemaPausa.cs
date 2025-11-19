using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Sistema completo de pausa del juego
/// Maneja: pausar tiempo, mostrar menú, detectar ESC, botones de pausa
/// </summary>
public class SistemaPausa : MonoBehaviour
{
    public static SistemaPausa Instance;

    [Header("Referencias UI")]
    public GameObject panelPausa;
    public Button botonReanudar;
    public Button botonReiniciar;
    public Button botonMenuPrincipal;
    
    [Header("Configuración")]
    public bool juegoPausado = false;
    
    // Propiedad pública para que CanvasManager pueda verificar el estado
    public bool JuegoPausado => juegoPausado;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // No usar DontDestroyOnLoad aquí, el CanvasManager ya lo hace
        }
    }

    private void Start()
    {
        ConfigurarBotonesPausa();
        OcultarPausa();
    }

    private void ConfigurarBotonesPausa()
    {
        botonReanudar?.onClick.RemoveAllListeners();
        botonReiniciar?.onClick.RemoveAllListeners();
        botonMenuPrincipal?.onClick.RemoveAllListeners();

        botonReanudar?.onClick.AddListener(() => TogglePausa());
        botonReiniciar?.onClick.AddListener(() => ReiniciarNivel());
        botonMenuPrincipal?.onClick.AddListener(() => IrMenuPrincipal());
    }

    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;
        
        if (juegoPausado)
        {
            MostrarPausa();
            Time.timeScale = 0f;
        }
        else
        {
            OcultarPausa();
            Time.timeScale = 1f;
        }
        
        Debug.Log($"⏸️ Juego {(juegoPausado ? "pausado" : "reanudado")}");
    }

    private void MostrarPausa()
    {
        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    private void OcultarPausa()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    private void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        juegoPausado = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausado = false;
        
        // Destruir sistemas persistentes
        if (CanvasManager.Instance != null)
            Destroy(CanvasManager.Instance.gameObject);
        
        if (ControladorJugador.Instance != null)
            Destroy(ControladorJugador.Instance.gameObject);
            
        SceneManager.LoadScene("MenuPrincipal");
    }

    private void Update()
    {
        // Tecla Escape para pausar (para testing en PC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }
}