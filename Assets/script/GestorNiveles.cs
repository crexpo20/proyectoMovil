using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestorNiveles : MonoBehaviour
{
    public static GestorNiveles Instance;

    [Header("Niveles Aleatorios")]
    public string[] nivelesAleatorios;

    [Header("Estado Actual")]
    public string nivelActual = "";
    
    [Header("Configuración de Niveles")]
    public string[] nombresNiveles;
    public string escenaFinal = "MenuPrincipal";
    public bool debugMode = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (debugMode) Debug.Log("GestorNiveles inicializado correctamente");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CargarSiguienteNivel(string nombreEscena = "")
    {
        string escenaACargar = nombreEscena;
        
        if (string.IsNullOrEmpty(escenaACargar))
        {
            escenaACargar = ObtenerSiguienteNivel();
        }
        
        if (!string.IsNullOrEmpty(escenaACargar))
        {
            nivelActual = escenaACargar;
            SceneManager.LoadScene(escenaACargar);
        }
        else
        {
            CargarEscenaFinal();
        }
    }

    public void CargarEscenaFinal()
    {
        
        // Destruir ambos para un reinicio limpio
        if (ControladorJugador.Instance != null)
        {
            Destroy(ControladorJugador.Instance.gameObject);
        }
            
        Destroy(gameObject);
        SceneManager.LoadScene(escenaFinal);
    }
    public void CargarNivelAleatorio()
{
    if (nivelesAleatorios == null || nivelesAleatorios.Length == 0)
    {
        return;
    }

    int indice = Random.Range(0, nivelesAleatorios.Length);
    string nivelElegido = nivelesAleatorios[indice];


    SceneManager.LoadScene(nivelElegido);
}

    private string ObtenerSiguienteNivel()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        
        for (int i = 0; i < nombresNiveles.Length - 1; i++)
        {
            if (nombresNiveles[i] == escenaActual)
            {
                string siguiente = nombresNiveles[i + 1];
                return siguiente;
            }
        }
        
        return null;
    }

    public void CargarNivelEspecifico(string nombreNivel)
    {
        
        if (Application.CanStreamedLevelBeLoaded(nombreNivel))
        {
            nivelActual = nombreNivel;
            SceneManager.LoadScene(nombreNivel);
        }
    }
}