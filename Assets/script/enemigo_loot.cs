using UnityEngine;
using System.Collections;

public class enemigo_loot : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] private GameObject[] posiblesLoot;
    [SerializeField] private float delayLoot = 0.5f;
    [SerializeField] private bool destroyOnAnyCollision = false;
    [SerializeField] private float spawnHeight = 5f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private AudioClip breakSound;
    
    private bool isBroken = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBroken && (collision.gameObject.CompareTag("weapon") || destroyOnAnyCollision))
        {
            StartCoroutine(RomperJarron());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isBroken && (collision.CompareTag("weapon") || destroyOnAnyCollision))
        {
            StartCoroutine(RomperJarron());
        }
    }

    private IEnumerator RomperJarron()
    {
        if (isBroken) yield break;
        isBroken = true;
        
        // Feedback visual opcional
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }
        
        // Feedback de audio opcional
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
        
        // Solo desactivar el renderer, NO el collider
        SetRendererVisible(false);
        
        yield return new WaitForSeconds(delayLoot);

        GenerarLoot();
        Destroy(gameObject);
    }

    private void GenerarLoot()
    {
        if (posiblesLoot.Length == 0) 
        {
            Debug.LogWarning("No hay objetos de loot configurados en el array posiblesLoot");
            return;
        }

        int randomIndex = Random.Range(0, posiblesLoot.Length);
        GameObject lootSeleccionado = posiblesLoot[randomIndex];

        if (lootSeleccionado != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;
            Instantiate(lootSeleccionado, spawnPosition, Quaternion.identity);
            Debug.Log($"Loot generado: {lootSeleccionado.name} en posición: {spawnPosition}");
        }
        else
        {
            Debug.LogWarning("El loot seleccionado es null en el array posiblesLoot");
        }
    }
    
    private void SetRendererVisible(bool visible)
    {
        // Solo desactivar el renderer, mantener el collider activo
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) 
        {
            renderer.enabled = visible;
            Debug.Log($"Renderer visible: {visible}");
        }
        
        // NO desactivar el collider aquí
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 spawnPoint = transform.position + Vector3.up * spawnHeight;
        Gizmos.DrawWireSphere(spawnPoint, 0.5f);
        Gizmos.DrawLine(transform.position, spawnPoint);
    }
}