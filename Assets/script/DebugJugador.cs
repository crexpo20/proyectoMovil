using UnityEngine;

public class DebugJugador : MonoBehaviour
{
    private Renderer rendererJugador;
    private bool debugActivo = true;

    private void Start()
    {
        rendererJugador = GetComponent<Renderer>();
        InvokeRepeating("VerificarEstado", 0f, 2f); // Verificar cada 2 segundos
    }

    private void VerificarEstado()
    {
        if (!debugActivo) return;

        Debug.Log("=== DEBUG JUGADOR ===");
        Debug.Log($"Activo en jerarquía: {gameObject.activeInHierarchy}");
        Debug.Log($"Activo: {gameObject.activeSelf}");
        
        if (rendererJugador != null)
        {
            Debug.Log($"Renderer enabled: {rendererJugador.enabled}");
            Debug.Log($"Renderer visible: {rendererJugador.isVisible}");
            Debug.Log($"Material: {rendererJugador.material?.name}");
        }
        else
        {
            Debug.LogError("❌ No hay Renderer en el jugador");
        }

        // Verificar todos los componentes
        Component[] componentes = GetComponents<Component>();
        Debug.Log($"Componentes encontrados: {componentes.Length}");
        
        // Verificar hijos
        if (transform.childCount > 0)
        {
            Debug.Log($"Hijos: {transform.childCount}");
            foreach (Transform hijo in transform)
            {
                Debug.Log($"- Hijo: {hijo.name} (Activo: {hijo.gameObject.activeInHierarchy})");
            }
        }
    }

    private void Update()
    {
        // Tecla D para toggle debug
        if (Input.GetKeyDown(KeyCode.D))
        {
            debugActivo = !debugActivo;
            Debug.Log($"Debug {(debugActivo ? "activado" : "desactivado")}");
        }

        // Tecla R para forzar reactivación
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReactivarJugador();
        }
    }

    public void ReactivarJugador()
    {
        Debug.Log("⚡ Forzando reactivación del jugador");
        
        // Reactivar GameObject
        gameObject.SetActive(true);
        
        // Reactivar Renderer
        if (rendererJugador != null)
        {
            rendererJugador.enabled = true;
        }
        
        // Reactivar todos los hijos
        foreach (Transform hijo in transform)
        {
            hijo.gameObject.SetActive(true);
            Renderer rendererHijo = hijo.GetComponent<Renderer>();
            if (rendererHijo != null) rendererHijo.enabled = true;
        }
        
        Debug.Log("✅ Jugador reactivado");
    }
}