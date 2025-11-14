using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class vidaPersonaje : MonoBehaviour
{
    public int vidamaxima = 3;
    public Image[] VidaImagen;
    
    // === NUEVO: Variable para evitar múltiples llamadas a Game Over ===
    private bool estaMuerto = false;
    
    void Start()
    {
        // Inicializar interfaz
        actualizarinterface();
        estaMuerto = false;
    }
    
    /// <summary>
    /// Actualiza los corazones/vidas en la UI
    /// </summary>
    void actualizarinterface() 
    {
        for (int i = 0; i < VidaImagen.Length; i++)
        {
            VidaImagen[i].enabled = i < vidamaxima;
        }
    }
    
    /// <summary>
    /// DEPRECADO - Ya no se usa, ahora usamos Game Over
    /// Mantenerlo por compatibilidad pero no se llama
    /// </summary>
    void reiniciarecena() 
    {
        int curretSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(curretSceneIndex);    
    }
    
    /// <summary>
    /// Llamado cuando el personaje recibe daño
    /// </summary>
    public void hit()
    {
        // Si ya está muerto, no procesar más daño
        if (estaMuerto)
            return;
        
        // Reducir vida
        vidamaxima = vidamaxima - 1;
        
        // Actualizar corazones en pantalla
        actualizarinterface();
        
        Debug.Log("¡Golpe recibido! Vida restante: " + vidamaxima);
        
        // Verificar si murió
        if (vidamaxima <= 0)
        {
            Morir();
        }
    }
    
    /// <summary>
    /// NUEVO MÉTODO: Maneja la muerte del personaje
    /// Muestra Game Over con estadísticas
    /// </summary>
    void Morir()
    {
        // Evitar llamadas múltiples
        if (estaMuerto)
            return;
        
        estaMuerto = true;
        
        Debug.Log("=== JUGADOR MURIÓ ===");
        
        // Obtener estadísticas del juego
        int nivel = 0;
        int dinero = 0;
        float tiempo = 0f;
        
        if (GestorEstadisticas.Instancia != null)
        {
            GestorEstadisticas.Instancia.ObtenerEstadisticas(out nivel, out dinero, out tiempo);
        }
        else
        {
            Debug.LogWarning("GestorEstadisticas no encontrado. Usando valores por defecto.");
        }
        
        // Mostrar pantalla de Game Over
        if (SistemaGameOver.Instancia != null)
        {
            SistemaGameOver.Instancia.MostrarGameOver(nivel, dinero, tiempo);
        }
        else
        {
            Debug.LogError("SistemaGameOver no encontrado! Asegúrate de tener el GestorGameOver en la escena.");
            
            // Fallback: reiniciar escena si no hay Game Over
            Invoke("reiniciarecena", 2f);
        }
        
        // Desactivar controles del jugador
        DesactivarPersonaje();
    }
    
    /// <summary>
    /// Desactiva el personaje al morir
    /// </summary>
    void DesactivarPersonaje()
    {
        // Desactivar el script de movimiento
        Personaje_movimiento movimiento = GetComponent<Personaje_movimiento>();
        if (movimiento != null)
        {
            movimiento.enabled = false;
        }
        
        // Desactivar el Rigidbody para que no se mueva
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false; // Desactiva las físicas
        }
        
        // Opcional: Hacer el sprite semi-transparente o cambiar color
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color color = sprite.color;
            color.a = 0.5f; // 50% transparente
            sprite.color = color;
        }
        
        // Opcional: Desactivar el collider para que no interactúe
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
    
    /// <summary>
    /// Método público para curar al personaje (para power-ups futuros)
    /// </summary>
    public void Curar(int cantidad)
    {
        if (estaMuerto)
            return;
        
        vidamaxima += cantidad;
        
        // No exceder el máximo (generalmente 3)
        if (vidamaxima > VidaImagen.Length)
        {
            vidamaxima = VidaImagen.Length;
        }
        
        actualizarinterface();
        
        Debug.Log("¡Curado! Vida actual: " + vidamaxima);
    }
    
    /// <summary>
    /// Método para reiniciar la vida (útil al comenzar nivel)
    /// </summary>
    public void ReiniciarVida()
    {
        vidamaxima = 3; // O el valor máximo inicial
        estaMuerto = false;
        actualizarinterface();
    }
}