using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    
    [Header("Límites de Cámara (Opcional)")]
    public bool useBounds = false;
    public float minX, maxX, minY, maxY;
    
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // Buscar al jugador al inicio
        BuscarJugador();
        
        // Si no lo encuentra, intentar cada medio segundo
        if (target == null)
        {
            InvokeRepeating("BuscarJugador", 0.5f, 0.5f);
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            SeguirJugador();
        }
        else
        {
            // Seguir buscando al jugador si no se ha encontrado
            BuscarJugador();
        }
    }

    private void SeguirJugador()
    {
        // Posición deseada de la cámara
        Vector3 desiredPosition = target.position + offset;
        
        // Aplicar límites si están activados
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // Suavizado del movimiento
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }

    public void BuscarJugador()
    {
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        
        if (jugador != null)
        {
            target = jugador.transform;
            Debug.Log($"✅ Cámara encontró al jugador: {jugador.name}");
            
            // Dejar de buscar una vez encontrado
            CancelInvoke("BuscarJugador");
            
            // Posicionar la cámara inmediatamente en el jugador
            if (target != null)
            {
                Vector3 startPosition = target.position + offset;
                transform.position = startPosition;
                Debug.Log($"Cámara posicionada en: {startPosition}");
            }
        }
        else
        {
            Debug.Log("🔍 Cámara buscando jugador...");
        }
    }

    // Método público para asignar target manualmente
    public void AsignarTarget(Transform nuevoTarget)
    {
        target = nuevoTarget;
        if (target != null)
        {
            Debug.Log($"🎯 Target asignado manualmente: {target.name}");
        }
    }

    // Llamar este método cuando el jugador se respawnee
    public void OnJugadorRespawn()
    {
        BuscarJugador();
    }
}