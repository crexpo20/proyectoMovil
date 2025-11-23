using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class cajaloot : MonoBehaviour
{
    [Header("Loot Posible")]
    [SerializeField] private GameObject[] itemsLoot;
    [SerializeField] private GameObject[] enemigosPosibles;
    
    [Header("Configuración Aleatoria")]
    [SerializeField] private int minItems = 1;
    [SerializeField] private int maxItems = 3;
    [SerializeField] private float radioDispersion = 0.8f;

    [Header("Probabilidades (%)")]
    [SerializeField] [Range(0, 100)] private int chanceLoot = 60;
    [SerializeField] [Range(0, 100)] private int chanceEnemigo = 25;
    [SerializeField] [Range(0, 100)] private int chanceVacio = 15;

    [Header("Configuración Spawn Enemigos")]
    [SerializeField] private LayerMask sueloLayer;
    [SerializeField] private float distanciaBusquedaSuelo = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("weapon"))
        {
            recolectar();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("weapon"))
        {
            recolectar();
        }
    }

    private void recolectar()
    {
        GenerarContenidoAleatorio();
        Destroy(gameObject);
    }

    private void GenerarContenidoAleatorio()
    {
        int random = UnityEngine.Random.Range(0, 100);
        
        if (random < chanceLoot)
        {
            GenerarItemsAleatorios();
        }
        else if (random < chanceLoot + chanceEnemigo)
        {
            GenerarEnemigoAleatorio();
        }
        else
        {
            Debug.Log("Caja vacía :(");
        }
    }

    private void GenerarItemsAleatorios()
    {
        if (itemsLoot.Length == 0) return;

        int cantidad = UnityEngine.Random.Range(minItems, maxItems + 1);
        
        for (int i = 0; i < cantidad; i++)
        {
            int index = UnityEngine.Random.Range(0, itemsLoot.Length);
            Vector3 posicion = CalcularPosicionDispersa();
            Instantiate(itemsLoot[index], posicion, Quaternion.identity);
        }
        
        Debug.Log($"Generados {cantidad} items");
    }

    private void GenerarEnemigoAleatorio()
    {
        if (enemigosPosibles.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, enemigosPosibles.Length);
            Vector3 posicionSpawn = CalcularPosicionSpawnEnemigo();
            Instantiate(enemigosPosibles[index], posicionSpawn, Quaternion.identity);
            Debug.Log("¡Enemigo aparecido en posición correcta!");
        }
        else
        {
            GenerarItemsAleatorios();
        }
    }

    private Vector3 CalcularPosicionSpawnEnemigo()
    {
        // Buscar suelo debajo de la caja
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanciaBusquedaSuelo, sueloLayer);
        
        if (hit.collider != null)
        {
            // Spawn en el suelo encontrado
            Debug.DrawRay(transform.position, Vector2.down * hit.distance, Color.green, 2f);
            return hit.point;
        }
        else
        {
            // Si no encuentra suelo, usar posición de la caja con offset hacia abajo
            Debug.DrawRay(transform.position, Vector2.down * distanciaBusquedaSuelo, Color.red, 2f);
            Debug.LogWarning("No se encontró suelo para spawnear enemigo, usando posición por defecto");
            return transform.position + Vector3.down * 1f;
        }
    }

    private Vector3 CalcularPosicionDispersa()
    {
        Vector2 offset = UnityEngine.Random.insideUnitCircle * radioDispersion;
        return transform.position + (Vector3)offset;
    }
}