using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flechaMovement : MonoBehaviour
{
    public float Velocidad;
    public Vector2 Direccion;
    private Rigidbody2D Rigidbody2D;

    void Start()
    {
	Rigidbody2D = GetComponent<Rigidbody2D>();        
    }
    
    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = Direccion * Velocidad;
    }
}
