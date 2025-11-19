using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorJugador : MonoBehaviour
{
    public static ControladorJugador Instance;
    
    [SerializeField] private string puntoSpawnInicial = "entrada";
    [SerializeField] private float offsetAdelante = 1.5f;
    [SerializeField] private float posicionZ = -2f;
    
    // Evento para notificar cuando el jugador está listo
    public System.Action<Transform> OnJugadorListo;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Suscribirse al evento de cambio de escena
            SceneManager.sceneLoaded += OnEscenaCargada;
            
            Debug.Log("Jugador inicializado con sistema de eventos");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Notificar que el jugador está listo
        NotificarJugadorListo();
    }
    
    private void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        Debug.Log($"Jugador detectó escena cargada: {escena.name}");
        
        // Spawnear automáticamente en cada nueva escena
        ColocarEnPuntoSpawn(puntoSpawnInicial, offsetAdelante);
        
        // Notificar a la cámara que el jugador está listo
        NotificarJugadorListo();
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnEscenaCargada;
    }

    public void ColocarEnPuntoSpawn(string nombrePunto, float offsetPersonalizado = -1f)
    {
        float offset = offsetPersonalizado >= 0 ? offsetPersonalizado : offsetAdelante;
        
        GameObject puntoSpawn = GameObject.Find(nombrePunto);
        if (puntoSpawn != null)
        {
            Vector3 posicionAdelante = puntoSpawn.transform.position + 
                                     puntoSpawn.transform.right * offset;
            
            // FORZAR Z = -2
            posicionAdelante.z = posicionZ;
            
            transform.position = posicionAdelante;
            
            Debug.Log($"Jugador colocado en: {nombrePunto} con offset: {offset}");
            
            // Notificar después de reposicionar
            NotificarJugadorListo();
        }
        else
        {
            Debug.LogWarning($"No se encontró el punto de spawn: {nombrePunto}");
        }
    }

    private void NotificarJugadorListo()
    {
        // Buscar todas las cámaras en la escena y notificarles
        FollowCamera[] camaras = FindObjectsOfType<FollowCamera>();
        foreach (FollowCamera camara in camaras)
        {
            camara.AsignarTarget(this.transform);
        }
        
        // También disparar el evento
        OnJugadorListo?.Invoke(this.transform);
        
        Debug.Log($"🎮 Jugador notificado como listo. Cámaras encontradas: {camaras.Length}");
    }
}