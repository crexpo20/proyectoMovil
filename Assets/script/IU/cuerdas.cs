using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class cuerdas : MonoBehaviour
{
    public static Action<int> CuerdaRec;
    private int cuerda=1;
    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Player"))
        {
            recolectar();
        }
    }
    private void recolectar()
    {
        CuerdaRec?.Invoke(cuerda);
        Destroy(gameObject);

    }
}

