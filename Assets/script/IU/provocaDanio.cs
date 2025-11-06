using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class provocaDanio : MonoBehaviour
{
    public vidaPersonaje Vidajuador;
    public bool puededa;
    public float cold = 1.5f;
    private float lastAttackTime = -10f;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //este es una modificaciiion para cuando mate opor arriba no ataque o quite vida al personaje
	if (!enabled) return;

	vidaPersonaje monje = collision.GetComponent<vidaPersonaje>();

        if (monje != null)
        {
	    if(Time.time >= lastAttackTime + cold)
	    {
	        lastAttackTime = Time.time;
   	         // Activa animación de ataque
                if (anim != null)
                {
                    anim.SetTrigger("Attack");
                }
	    }
            // Mantengo tu daño original tal cual estaba
            monje.hit();
         
        }
        //vidanum.danio();
    }

}