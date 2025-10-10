using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class provocaDanio : MonoBehaviour
{
    public  vidaPersonaje Vidajuador;
    public bool puededa;
    private float cold = 3f;

    /*  private void OnTriggerEnter2D(Collider2D other) {
           Debug.Log("danio");
           if (other.gameObject.CompareTag("Player") && puededa) {
              Vidajuador.recibirdanio(1);
               Debug.Log("danio2");
              puededa = false;
              StartCoroutine(Cooldawndanio());
          }
       }
       IEnumerator Cooldawndanio() {
           yield return new WaitForSeconds(cold);
           puededa = true;

       } */
    private void OnTriggerEnter2D(Collider2D collision)
    {
         
        vidaPersonaje monje = collision.GetComponent<vidaPersonaje>();
        if (monje != null)
        {
            monje.hit();
        }
    }

}
