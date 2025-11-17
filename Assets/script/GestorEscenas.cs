using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorEscenas : MonoBehaviour
{
    public static GestorEscenas Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Cargar la UI persistente al inicio
            CargarUIPersistente();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Asegurarse de que la UI esté cargada
        CargarUIPersistente();
    }
    
    private void CargarUIPersistente()
    {
        // Verificar si la escena de UI ya está cargada
        bool uiYaCargada = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == "UIPersistente")
            {
                uiYaCargada = true;
                break;
            }
        }
        
        // Cargar la escena de UI si no está cargada
        if (!uiYaCargada)
        {
            SceneManager.LoadScene("UIPersistente", LoadSceneMode.Additive);
        }
    }
    
    // Métodos públicos para cambiar de escena
    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }
    
    public void CargarEscenaAdditive(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena, LoadSceneMode.Additive);
    }
    
    public void RecargarEscenaActual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void SalirJuego()
    {
        Application.Quit();
    }
}
