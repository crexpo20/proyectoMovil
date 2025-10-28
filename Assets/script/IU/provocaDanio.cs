using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class provocaDanio : MonoBehaviour
{
    public  Personaje_movimiento Vidajuador;
    public bool puededa;
    private float cold = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
         
        Personaje_movimiento monje = collision.GetComponent<Personaje_movimiento>();
        if (monje != null)
        {
            monje.hit();
        }
    }

}
