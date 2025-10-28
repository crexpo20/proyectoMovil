using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System ;
public class moneda : MonoBehaviour

{
    public static Action<int> Ororeco; 
    [SerializeField] private int oroganado;
    private void OnTriggerEnter2D(Collider2D collision)
    {
       

        if (collision.CompareTag("Player"))
        {
            recolectar();
        }
    }
        private void recolectar() {
        Ororeco?.Invoke(oroganado);
        Destroy(gameObject);

         }
}
