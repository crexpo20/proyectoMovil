using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIPersistente : MonoBehaviour
{
    private static UIPersistente instance;
    private Personaje_movimiento player;

    [Header("Configuración de Escenas")]
    public string[] escenasOcultas;

    [Header("Referencias de UI")]
    public TextMeshProUGUI textoBombas;
    public TextMeshProUGUI textoCuerdas;
    public TextMeshProUGUI textoVida;
    public GameObject panelMovil;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Personaje_movimiento.UsoBomba += ActualizarBombasUI;
            Personaje_movimiento.UsoCuerda += ActualizarCuerdasUI;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool ocultarUI = false;
        
        foreach (string nombre in escenasOcultas)
        {
            if (scene.name == nombre)
            {
                ocultarUI = true;
                break;
            }
        }

        gameObject.SetActive(!ocultarUI);

        // Buscar al jugador solo si la UI está activa
        if (!ocultarUI)
        {
            BuscarJugador();
            ConfigurarControlesMoviles();
        }
        else
        {
            player = null;
        }
    }

    private void BuscarJugador()
    {
        player = FindObjectOfType<Personaje_movimiento>();
        
        if (player == null)
        {
            Invoke("BuscarJugador", 0.1f);
        }
        else
        {
            ActualizarUICompleta();
        }
    }
    private void ConfigurarControlesMoviles()
    {
        if (panelMovil != null)
        {
            #if UNITY_ANDROID || UNITY_IOS
                panelMovil.SetActive(true);
            #else
                panelMovil.SetActive(false);
            #endif
        }
    }

    void Update()
    {
     
    }

    private void ActualizarUICompleta()
    {
        if (player != null)
        {
            // Actualizar bombas
            if (textoBombas != null)
                textoBombas.text = player.GetBombCount().ToString();
            
            // Actualizar cuerdas
            if (textoCuerdas != null)
                textoCuerdas.text = player.GetRopeCount().ToString();
            
            // Actualizar vida
            if (textoVida != null)
            {
                textoVida.text = player.GetVida().ToString();;
            }
        }
    }
    // Métodos llamados por eventos
    private void ActualizarBombasUI(int cambio)
    {
        if (textoBombas != null && player != null)
            textoBombas.text = player.GetBombCount().ToString();
    }

    private void ActualizarCuerdasUI(int cambio)
    {
        if (textoCuerdas != null && player != null)
            textoCuerdas.text = player.GetRopeCount().ToString();
    }

    // ------Metodos publicos para botones--------
    public void PausarJuego()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void RecargarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Personaje_movimiento.UsoBomba -= ActualizarBombasUI;
        Personaje_movimiento.UsoCuerda -= ActualizarCuerdasUI;
    }
}
