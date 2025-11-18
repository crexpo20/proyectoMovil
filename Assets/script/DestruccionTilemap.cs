using UnityEngine;
using UnityEngine.Tilemaps;

public class DestruccionTilemap : MonoBehaviour
{
    [Header("Configuración Destrucción")]
    public Tilemap tilemapDestructible;
    public float radioDestruccion = 2f;
    
    [Header("Efectos Visuales (Opcional)")]
    public GameObject efectoDestruccionPrefab;
    public AudioClip sonidoDestruccion;
    
    void Start()
    {
        // Si no se asignó manualmente, buscar el Tilemap en este GameObject
        if (tilemapDestructible == null)
        {
            tilemapDestructible = GetComponent<Tilemap>();
        }
    }
    
    public void CrearHuecoExplosion(Vector3 posicionMundo, float radio)
    {
        if (tilemapDestructible == null)
        {
            return;
        }
        
        Vector3Int centroTile = tilemapDestructible.WorldToCell(posicionMundo);
        float tamañoTile = tilemapDestructible.cellSize.x;
        int radioEnTiles = Mathf.CeilToInt(radio / tamañoTile);
        
        int tilesDestruidos = 0;
        
        for (int x = -radioEnTiles; x <= radioEnTiles; x++)
        {
            for (int y = -radioEnTiles; y <= radioEnTiles; y++)
            {
                Vector3Int posicionTile = centroTile + new Vector3Int(x, y, 0);
                
                float distancia = Mathf.Sqrt(x * x + y * y);
                if (distancia <= radioEnTiles && tilemapDestructible.HasTile(posicionTile))
                {
                    tilemapDestructible.SetTile(posicionTile, null);
                    tilesDestruidos++;
                    
                    if (efectoDestruccionPrefab != null)
                    {
                        Vector3 posicionMundoTile = tilemapDestructible.CellToWorld(posicionTile) + tilemapDestructible.cellSize / 2f;
                        Instantiate(efectoDestruccionPrefab, posicionMundoTile, Quaternion.identity);
                    }
                }
            }
        }
        
        if (sonidoDestruccion != null && tilesDestruidos > 0)
        {
            AudioSource.PlayClipAtPoint(sonidoDestruccion, posicionMundo);
        }
    }
}