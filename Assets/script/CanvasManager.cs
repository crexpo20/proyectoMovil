using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    
    [Header("Referencias UI - HUD")]
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoBombas;
    public TextMeshProUGUI textoCuerdas;
    public TextMeshProUGUI textoOro;
    
    [Header("Referencias UI - Controles")]
    public FixedJoystick joystick;
    public GameObject botonSaltar;
    public GameObject botonAtaque;
    public GameObject botonBomba;
    public GameObject botonCuerda;
    public Button botonPausaUI;
    
    [Header("Referencias UI - Paneles")]
    public GameObject panelPausa;
    public Button botonReanudar;
    public Button botonReiniciar;
    public Button botonMenuPrincipal;

    // Datos del juego
    private int oro = 0;
    private bool juegoPausado = false;
    private Personaje_movimiento personaje;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnEscenaCargada;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ConfigurarBotones();
        OcultarPausa();
        
        BuscarPersonaje();
        
        Personaje_movimiento.UsoBomba += OnUsoBomba;
        Personaje_movimiento.UsoCuerda += OnUsoCuerda;
        moneda.Ororeco += OnOroRecolectado;
        bombas.Bombrec += OnBombaRecolectada;
        cuerdas.CuerdaRec += OnCuerdaRecolectada;
    }

    private void Update()
    {
        ActualizarUI();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }

    private void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        Invoke("ReconectarPersonaje", 0.1f);
    }

    private void ReconectarPersonaje()
    {
        personaje = FindObjectOfType<Personaje_movimiento>();
        if (personaje == null) return;

        personaje.joystick = joystick;

        var botonSaltoScript = botonSaltar.GetComponent<BotonSaltoInstantaneo>();
        if (botonSaltoScript != null) 
            botonSaltoScript.personaje = personaje;

        var botonAtaqueScript = botonAtaque.GetComponent<BotonAtaque>();
        if (botonAtaqueScript != null)
            botonAtaqueScript.personaje = personaje;

        var botonBombaScript = botonBomba.GetComponent<BotonBomba>();
        if (botonBombaScript != null)
            botonBombaScript.personaje = personaje;

        var botonCuerdaScript = botonCuerda.GetComponent<BotonCuerda>();
        if (botonCuerdaScript != null)
            botonCuerdaScript.personaje = personaje;
    }

    private void BuscarPersonaje()
    {
        personaje = FindObjectOfType<Personaje_movimiento>();
        if (personaje != null)
        {
            ReconectarControles();
        }
        else
        {
            InvokeRepeating("BuscarPersonajeContinuo", 0.5f, 0.5f);
        }
    }

    private void BuscarPersonajeContinuo()
    {
        personaje = FindObjectOfType<Personaje_movimiento>();
        if (personaje != null)
        {
            ReconectarControles();
            CancelInvoke("BuscarPersonajeContinuo");
        }
    }

    private void ReconectarControles()
    {
        if (personaje == null) return;

        if (joystick != null)
        {
            personaje.joystick = joystick;
        }

        var botonSaltoScript = botonSaltar.GetComponent<BotonSaltoInstantaneo>();
        if (botonSaltoScript != null)
            botonSaltoScript.personaje = personaje;

        var botonAtaqueScript = botonAtaque.GetComponent<BotonAtaque>();
        if (botonAtaqueScript != null)
            botonAtaqueScript.personaje = personaje;

        var botonBombaScript = botonBomba.GetComponent<BotonBomba>();
        if (botonBombaScript != null)
            botonBombaScript.personaje = personaje;

        var botonCuerdaScript = botonCuerda.GetComponent<BotonCuerda>();
        if (botonCuerdaScript != null)
            botonCuerdaScript.personaje = personaje;
    }

    private void ConfigurarBotones()
    {
        // Configurar botón de pausa UI
        if (botonPausaUI != null)
        {
            botonPausaUI.onClick.RemoveAllListeners();
            botonPausaUI.onClick.AddListener(TogglePausa);
        }
        
        // Configurar botones del panel de pausa
        if (botonReanudar != null)
        {
            botonReanudar.onClick.RemoveAllListeners();
            botonReanudar.onClick.AddListener(TogglePausa);
        }
        
        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.RemoveAllListeners();
            botonReiniciar.onClick.AddListener(ReiniciarNivel);
        }
        
        if (botonMenuPrincipal != null)
        {
            botonMenuPrincipal.onClick.RemoveAllListeners();
            botonMenuPrincipal.onClick.AddListener(IrMenuPrincipal);
        }
    }

    #region SISTEMA DE PAUSA UNIFICADO
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
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // En el build (Android/PC)
            Application.Quit();
        #endif
    }

    private void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        juegoPausado = false;
        
        // Destruir sistemas persistentes para reinicio limpio
        if (Instance != null)
            Destroy(Instance.gameObject);
        
        SceneManager.LoadScene("MenuPrincipal");
        
        Debug.Log("Cargando menú principal...");
    }
    #endregion

    #region ACTUALIZACIÓN DE UI
    public void ActualizarUI()
    {
        if (personaje != null)
        {
            if (textoVidas != null) 
                textoVidas.text = $"{personaje.GetVida()}";
            
            if (textoBombas != null) 
                textoBombas.text = $"{personaje.GetBombCount()}";
            
            if (textoCuerdas != null) 
                textoCuerdas.text = $"{personaje.GetRopeCount()}";
        }
        
        if (textoOro != null) 
            textoOro.text = $"{oro}";
    }
    #endregion

    #region MANEJO DE EVENTOS
    private void OnUsoBomba(int cantidad)
    {
        ActualizarUI();
    }

    private void OnUsoCuerda(int cantidad)
    {
        ActualizarUI();
    }

    private void OnOroRecolectado(int cantidad)
    {
        oro += cantidad;
        ActualizarUI();
    }

    private void OnBombaRecolectada(int cantidad)
    {
        if (personaje != null)
        {
            personaje.AddBombs(cantidad);
            ActualizarUI();
        }
    }

    private void OnCuerdaRecolectada(int cantidad)
    {
        if (personaje != null)
        {
            personaje.AddCuerdas(cantidad);
            ActualizarUI();
        }
    }
    #endregion

    #region MÉTODOS PÚBLICOS
    public void AgregarOro(int cantidad)
    {
        oro += cantidad;
        ActualizarUI();
    }

    public int GetOro() => oro;
    public int GetVidas() => personaje != null ? personaje.GetVida() : 0;
    public int GetBombas() => personaje != null ? personaje.GetBombCount() : 0;
    public int GetCuerdas() => personaje != null ? personaje.GetRopeCount() : 0;
    #endregion

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
        
        // Desuscribirse de eventos
        Personaje_movimiento.UsoBomba -= OnUsoBomba;
        Personaje_movimiento.UsoCuerda -= OnUsoCuerda;
        moneda.Ororeco -= OnOroRecolectado;
        bombas.Bombrec -= OnBombaRecolectada;
        cuerdas.CuerdaRec -= OnCuerdaRecolectada;
        
        // Asegurar que el tiempo se restaure
        Time.timeScale = 1f;
    }
}