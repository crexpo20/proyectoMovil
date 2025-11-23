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
    private void OnCollisionEnter2D(Collision2D collision)
    {
       

        if (collision.gameObject.CompareTag("Player"))
        {
           if (AudioManager.Instance != null)
            AudioManager.Instance.ReproducirMoneda();
           recolectar();
        }
    }
        private void recolectar() {
        Ororeco?.Invoke(oroganado);
        Destroy(gameObject);

         }
}
