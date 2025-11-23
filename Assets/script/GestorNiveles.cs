using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GestorNiveles : MonoBehaviour
{
    public static GestorNiveles Instance;

    [Header("Niveles Aleatorios")]
    public string[] nivelesAleatorios;

    
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
        Debug.Log($"=== INICIANDO CARGA DE NIVEL ===");
        Debug.Log($"Escena solicitada: {nombreEscena}");
        Debug.Log($"Escena actual: {SceneManager.GetActiveScene().name}");

        string escenaACargar = nombreEscena;
        
        // Si no se especifica escena, buscar la siguiente en la lista
        if (string.IsNullOrEmpty(escenaACargar))
        {
            escenaACargar = ObtenerSiguienteNivel();
            Debug.Log($"Siguiente nivel automático: {escenaACargar}");
        }
        
        if (!string.IsNullOrEmpty(escenaACargar))
        {
            Debug.Log($"✅ Cargando nivel: {escenaACargar}");
            SceneManager.LoadScene(escenaACargar);
        }
        else
        {
            Debug.LogWarning("❌ No se pudo determinar el siguiente nivel");
            CargarEscenaFinal();
        }
    }

    public void CargarEscenaFinal()
    {
        Debug.Log($"Cargando escena final: {escenaFinal}");
        
        // Destruir ambos para un reinicio limpio
        if (ControladorJugador.Instance != null)
        {
            Destroy(ControladorJugador.Instance.gameObject);
            Debug.Log("Jugador destruido para volver al menú");
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
        Debug.Log($"Buscando siguiente nivel para: {escenaActual}");
        
        for (int i = 0; i < nombresNiveles.Length - 1; i++)
        {
            if (nombresNiveles[i] == escenaActual)
            {
                string siguiente = nombresNiveles[i + 1];
                Debug.Log($"✅ Encontrado siguiente nivel: {siguiente}");
                return siguiente;
            }
        }
        
        Debug.LogWarning($"❌ No se encontró {escenaActual} en la lista de niveles");
        Debug.Log($"Niveles configurados: {string.Join(", ", nombresNiveles)}");
        return null;
    }

    public void CargarNivelEspecifico(string nombreNivel)
    {
        Debug.Log($"Cargando nivel específico: {nombreNivel}");
        
        // Verificar si la escena existe
        if (Application.CanStreamedLevelBeLoaded(nombreNivel))
        {
            SceneManager.LoadScene(nombreNivel);
        }
        else
        {
            Debug.LogError($"❌ No se puede cargar la escena: {nombreNivel}");
            Debug.Log($"¿Está agregada en Build Settings?");
        }
    }
}