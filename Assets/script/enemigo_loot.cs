using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class enemigo_loot : MonoBehaviour
{
    [SerializeField] private GameObject[] posiblesLoot;
    [SerializeField] private float delayLoot = 0.5f;
    //-------colisiones de la caja---------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("weapon"))
        {
            StartCoroutine(RomperJarron());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("weapon"))
        {
            StartCoroutine(RomperJarron());
        }
    }
    private IEnumerator RomperJarron()
    {
        yield return new WaitForSeconds(delayLoot);

        GenerarLoot();

        Destroy(gameObject);
    }
    private void GenerarLoot()
    {
        if (posiblesLoot.Length == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, posiblesLoot.Length);
        GameObject lootSeleccionado = posiblesLoot[randomIndex];

        Instantiate(lootSeleccionado, transform.position, Quaternion.identity);
    }

}
