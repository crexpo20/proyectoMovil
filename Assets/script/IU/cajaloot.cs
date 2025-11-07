using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
public class cajaloot : MonoBehaviour
{

    [SerializeField] private GameObject[] lootcaja;
    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
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
