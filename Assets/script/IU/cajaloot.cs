using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
public class cajaloot : MonoBehaviour
{

    [SerializeField] private GameObject[] lootcaja;
    //-------colisiones de la caja---------------
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
        generarLoot();
        Destroy(gameObject);
       
    }
    private void generarLoot() {
        foreach (GameObject objeto in lootcaja) {

            GameObject objetocreado = Instantiate(objeto, transform.position, Quaternion.identity);
        }
    
    }
}
