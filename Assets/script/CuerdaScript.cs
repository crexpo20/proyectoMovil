using System.Collections.Generic;
using UnityEngine;

public class CuerdaScript : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject segmentoPrefab;
    [SerializeField] private float segmentHeight = 0.66f;
    [SerializeField] private int segmentosMaximos = 5;
    public LayerMask obstacleLayer = 1;
    
    private List<GameObject> segmentos = new List<GameObject>();
    private BoxCollider2D areaTrepar;

    public void GenerarCuerdaDesdePersonaje(Vector2 posicionPersonaje)
    {
        transform.position = posicionPersonaje;
        
        float alturaFinal = CalcularAlturaFinal(posicionPersonaje);
        
        int segmentosACrear = Mathf.FloorToInt(alturaFinal / segmentHeight);
        segmentosACrear = Mathf.Min(segmentosACrear, segmentosMaximos);
        
        if (segmentosACrear > 0)
        {
            GenerarSegmentos(segmentosACrear);
            ConfigurarAreaTrepar(segmentosACrear * segmentHeight);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private float CalcularAlturaFinal(Vector2 posicionPersonaje)
    {
        float alturaDeseada = segmentosMaximos * segmentHeight;
        
        RaycastHit2D hit = Physics2D.Raycast(posicionPersonaje + Vector2.up * 0.1f,Vector2.up,alturaDeseada,obstacleLayer);
        
        if (hit.collider != null)
        {
            float alturaObstaculo = hit.distance;
           return alturaObstaculo;
        }
        else
        {
            return alturaDeseada;
        }
    }

    private void GenerarSegmentos(int cantidad)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        segmentos.Clear();
        
        for (int i = 0; i < cantidad; i++)
        {
            float offsetY = i * segmentHeight;
            Vector3 posicionLocal = new Vector3(0, offsetY, 0);
            
            GameObject segmento = Instantiate(segmentoPrefab, transform);
            segmento.transform.localPosition = posicionLocal;
            segmentos.Add(segmento);
    
        }
    }

    private void ConfigurarAreaTrepar(float alturaTotal)
    {
        if (areaTrepar != null) DestroyImmediate(areaTrepar);

        areaTrepar = gameObject.AddComponent<BoxCollider2D>();
        areaTrepar.isTrigger = true;
        areaTrepar.size = new Vector2(0.4f, alturaTotal);
        areaTrepar.offset = new Vector2(0, alturaTotal / 2f);

        gameObject.tag = "ladders";
    }
    private void Update()
    {
        for (int i = 0; i < segmentos.Count; i++)
        {
            if (segmentos[i] != null)
            {
                Debug.DrawRay(segmentos[i].transform.position, Vector3.up * 0.1f, Color.green);
                if (i < segmentos.Count - 1 && segmentos[i + 1] != null)
                {
                    Debug.DrawLine(segmentos[i].transform.position, segmentos[i + 1].transform.position, Color.blue);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (areaTrepar != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                transform.position + (Vector3)areaTrepar.offset, 
                areaTrepar.size
            );
        }
    }
}