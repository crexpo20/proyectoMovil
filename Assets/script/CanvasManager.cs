using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public Button botonAtaque;
    public Button botonBomba;
    public Button botonCuerda;
    public Button botonSaltar;
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
        // Singleton para el Canvas persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnEscenaCargada;
            
            Debug.Log("✅ CanvasManager unificado inicializado");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ConfigurarBotones();
        OcultarPausa();
        
        // Buscar personaje al inicio
        BuscarPersonaje();
        
        // Suscribirse a eventos de items
        Personaje_movimiento.UsoBomba += OnUsoBomba;
        Personaje_movimiento.UsoCuerda += OnUsoCuerda;
        moneda.Ororeco += OnOroRecolectado;
        bombas.Bombrec += OnBombaRecolectada;
        cuerdas.CuerdaRec += OnCuerdaRecolectada;
    }

    private void Update()
    {
        // Actualizar UI continuamente
        ActualizarUI();
        
        // Detectar tecla ESC para pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausa();
        }
    }

    private void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        Debug.Log($"🔄 CanvasManager en nueva escena: {escena.name}");
        
        ConfigurarBotones();

        // Re-conectar con el personaje en la nueva escena
        StartCoroutine(ReconectarConPersonaje());
    }

    private IEnumerator ReconectarConPersonaje()
    {
        // Esperar que la escena esté completamente cargada
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        BuscarPersonaje();
        
        // Forzar reconexión de controles
        if (personaje != null)
        {
            ReconectarControles();
        }
    }

    private void BuscarPersonaje()
    {
        personaje = FindObjectOfType<Personaje_movimiento>();
        if (personaje != null)
        {
            Debug.Log("✅ CanvasManager encontró al personaje");
            ReconectarControles();
        }
        else
        {
            Debug.Log("🔍 CanvasManager buscando personaje...");
            InvokeRepeating("BuscarPersonajeContinuo", 0.5f, 0.5f);
        }
    }

    private void BuscarPersonajeContinuo()
    {
        personaje = FindObjectOfType<Personaje_movimiento>();
        if (personaje != null)
        {
            Debug.Log("✅ CanvasManager encontró al personaje (búsqueda continua)");
            ReconectarControles();
            CancelInvoke("BuscarPersonajeContinuo");
        }
    }

    private void ReconectarControles()
    {
        if (personaje != null)
        {
            // ✅ IMPORTANTE: Reasignar el joystick
            if (joystick != null)
            {
                personaje.joystick = joystick;
                Debug.Log("🕹️ Joystick reconectado al personaje");
            }
            
            // ✅ Reasignar botones usando reflexión
            ReasignarBotones();
            ConfigurarBotones();
            ActualizarUI();
        }
    }

    private void ReasignarBotones()
    {
        if (personaje == null) return;

        var personajeType = personaje.GetType();
        
        // Reasignar botón de salto
        var botonSaltoField = personajeType.GetField("botonSalto", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (botonSaltoField != null && botonSaltar != null)
        {
            botonSaltoField.SetValue(personaje, botonSaltar);
            Debug.Log("🔄 Botón saltar reconectado");
        }
        
        // Reasignar botón de bomba
        var botonBombaField = personajeType.GetField("botonBomba", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (botonBombaField != null && botonBomba != null)
        {
            botonBombaField.SetValue(personaje, botonBomba);
            Debug.Log("🔄 Botón bomba reconectado");
        }
        
        // Reasignar botón de cuerda
        var botonCuerdaField = personajeType.GetField("botonCuerda", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (botonCuerdaField != null && botonCuerda != null)
        {
            botonCuerdaField.SetValue(personaje, botonCuerda);
            Debug.Log("🔄 Botón cuerda reconectado");
        }
        
        // Reasignar botón de ataque
        var botonAtaqueField = personajeType.GetField("botonAtaque", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (botonAtaqueField != null && botonAtaque != null)
        {
            botonAtaqueField.SetValue(personaje, botonAtaque);
            Debug.Log("🔄 Botón ataque reconectado");
        }
    }

    private void ConfigurarBotones()
    {
        // Limpiar listeners previos
        botonAtaque?.onClick.RemoveAllListeners();
        botonBomba?.onClick.RemoveAllListeners();
        botonCuerda?.onClick.RemoveAllListeners();
        botonSaltar?.onClick.RemoveAllListeners();
        botonPausaUI?.onClick.RemoveAllListeners();
        botonReanudar?.onClick.RemoveAllListeners();
        botonReiniciar?.onClick.RemoveAllListeners();
        botonMenuPrincipal?.onClick.RemoveAllListeners();

        // Configurar botones de acciones
        botonAtaque?.onClick.AddListener(() => PresionarAtaque());
        botonBomba?.onClick.AddListener(() => PresionarBomba());
        botonCuerda?.onClick.AddListener(() => PresionarCuerda());
        botonSaltar?.onClick.AddListener(() => PresionarSalto());
        botonPausaUI?.onClick.AddListener(() => TogglePausa());

        // Configurar menú pausa
        botonReanudar?.onClick.AddListener(() => TogglePausa());
        botonReiniciar?.onClick.AddListener(() => ReiniciarNivel());
        botonMenuPrincipal?.onClick.AddListener(() => IrMenuPrincipal());
    }

    #region CONTROL DEL PERSONAJE (Métodos públicos para presionar botones)
    private void PresionarSalto()
    {
        if (personaje != null && !juegoPausado)
        {
            var personajeType = personaje.GetType();
            var quiereSaltarField = personajeType.GetField("quiereSaltar", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (quiereSaltarField != null)
            {
                quiereSaltarField.SetValue(personaje, true);
            }
        }
    }

    private void PresionarAtaque()
    {
        if (personaje != null && !juegoPausado)
        {
            var personajeType = personaje.GetType();
            var lanzarAtaqueField = personajeType.GetField("lanzarAtaque", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (lanzarAtaqueField != null)
            {
                lanzarAtaqueField.SetValue(personaje, true);
            }
        }
    }

    private void PresionarBomba()
    {
        if (personaje != null && !juegoPausado)
        {
            var personajeType = personaje.GetType();
            var lanzarBombaField = personajeType.GetField("lanzarBomba", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (lanzarBombaField != null)
            {
                lanzarBombaField.SetValue(personaje, true);
            }
        }
    }

    private void PresionarCuerda()
    {
        if (personaje != null && !juegoPausado)
        {
            var personajeType = personaje.GetType();
            var lanzarCuerdasField = personajeType.GetField("lanzarCuerdas", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (lanzarCuerdasField != null)
            {
                lanzarCuerdasField.SetValue(personaje, true);
            }
        }
    }
    #endregion

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
        
        Debug.Log($"⏸️ Juego {(juegoPausado ? "pausado" : "reanudado")}");
    }

    private void MostrarPausa()
    {
        if (panelPausa != null)
            panelPausa.SetActive(true);
        
        if (botonPausaUI != null)
            botonPausaUI.gameObject.SetActive(false);
    }

    private void OcultarPausa()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);
        
        if (botonPausaUI != null)
            botonPausaUI.gameObject.SetActive(true);
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
        
        // Destruir sistemas persistentes para reinicio limpio
        if (ControladorJugador.Instance != null)
            Destroy(ControladorJugador.Instance.gameObject);
        
        Destroy(gameObject);
        SceneManager.LoadScene("MenuPrincipal");
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
        Debug.Log($"💰 Oro recolectado: +{cantidad} (Total: {oro})");
    }

    private void OnBombaRecolectada(int cantidad)
    {
        if (personaje != null)
        {
            personaje.AddBombs(cantidad);
            ActualizarUI();
            Debug.Log($"💣 Bomba recolectada: +{cantidad}");
        }
    }

    private void OnCuerdaRecolectada(int cantidad)
    {
        if (personaje != null)
        {
            personaje.AddCuerdas(cantidad);
            ActualizarUI();
            Debug.Log($"🪢 Cuerda recolectada: +{cantidad}");
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