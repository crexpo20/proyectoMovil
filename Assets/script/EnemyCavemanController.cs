using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCavemanController : MonoBehaviour
{
    public Transform personaje;
    public float detectionRadius = 0.5f;
    public float speed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, personaje.position);
        
        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (personaje.position - transform.position).normalized;
            // Solo movimiento horizontal (eje X)
            movement = new Vector2(direction.x, 0); 

            // Lógica para girar el sprite en la dirección del movimiento 🔄
            if (movement.x > 0)
            {
                // Moviéndose a la derecha: Escala normal (ej. 1)
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            else if (movement.x < 0)
            {
                // Moviéndose a la izquierda: Escala invertida en X (ej. -1)
                transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
            }
            // Si movement.x es 0, no se mueve, se mantiene la última orientación.
        }
        else
        {
            movement = Vector2.zero;
        }

        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

        // La función OnDrawGizmosSelected debe estar fuera de FixedUpdate
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}