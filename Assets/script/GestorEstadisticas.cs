using UnityEngine;

/// <summary>
/// Mantiene registro de las estadísticas del jugador durante la partida
/// Dinero recolectado, tiempo jugado, nivel actual, etc.
/// </summary>
public class GestorEstadisticas : MonoBehaviour
{
    // === SINGLETON ===
    public static GestorEstadisticas Instancia { get; private set; }
    
    [Header("Estadísticas Actuales")]
    [Tooltip("Dinero total recolectado en esta partida")]
    public int dineroTotal = 0;
    
    [Tooltip("Nivel actual (por ahora en desarrollo)")]
    public int nivelActual = 0;
    
    [Header("Solo Lectura (Debug)")]
    [SerializeField]
    [Tooltip("Tiempo jugado en segundos")]
    private float tiempoJugado = 0f;
    
    // Propiedades públicas de solo lectura
    public float TiempoJugado { get { return tiempoJugado; } }
    
    void Awake()
    {
        // Singleton
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Inicializar estadísticas
        dineroTotal = 0;
        nivelActual = 0;
        tiempoJugado = 0f;
        
        Debug.Log("Gestor de Estadísticas iniciado");
    }
    
    void Update()
    {
        // Incrementar tiempo jugado (solo si no está pausado)
        if (Time.timeScale > 0)
        {
            tiempoJugado += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Agrega dinero al total recolectado
    /// Llamar desde el script que detecta recolección de oro
    /// </summary>
    /// <param name="cantidad">Cantidad de dinero a agregar</param>
    public void AgregarDinero(int cantidad)
    {
        dineroTotal += cantidad;
        Debug.Log("Dinero recolectado: +" + cantidad + " | Total: " + dineroTotal);
    }
    
    /// <summary>
    /// Establece el nivel actual
    /// </summary>
    /// <param name="nivel">Número del nivel</param>
    public void EstablecerNivel(int nivel)
    {
        nivelActual = nivel;
        Debug.Log("Nivel actual: " + nivelActual);
    }
    
    /// <summary>
    /// Obtiene todas las estadísticas actuales
    /// </summary>
    public void ObtenerEstadisticas(out int nivel, out int dinero, out float tiempo)
    {
        nivel = nivelActual;
        dinero = dineroTotal;
        tiempo = tiempoJugado;
    }
    
    /// <summary>
    /// Reinicia todas las estadísticas (útil al comenzar nueva partida)
    /// </summary>
    public void ReiniciarEstadisticas()
    {
        dineroTotal = 0;
        nivelActual = 0;
        tiempoJugado = 0f;
        
        Debug.Log("Estadísticas reiniciadas");
    }
    
    /// <summary>
    /// Muestra estadísticas en consola (para debugging)
    /// </summary>
    public void MostrarEstadisticas()
    {
        Debug.Log("=== ESTADÍSTICAS ===");
        Debug.Log("Nivel: " + nivelActual);
        Debug.Log("Dinero: $" + dineroTotal);
        Debug.Log("Tiempo: " + FormatearTiempo(tiempoJugado));
    }
    
    /// <summary>
    /// Formatea el tiempo en formato MM:SS
    /// </summary>
    string FormatearTiempo(float tiempo)
    {
        int minutos = Mathf.FloorToInt(tiempo / 60f);
        int segundos = Mathf.FloorToInt(tiempo % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }
}