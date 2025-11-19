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
            // No genera nada - caja vacía
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
            Instantiate(enemigosPosibles[index], transform.position, Quaternion.identity);
            Debug.Log("¡Enemigo aparecido!");
        }
        else
        {
            // Fallback a loot si no hay enemigos
            GenerarItemsAleatorios();
        }
    }

    private Vector3 CalcularPosicionDispersa()
    {
        Vector2 offset = UnityEngine.Random.insideUnitCircle * radioDispersion;
        return transform.position + (Vector3)offset;
    }
}