using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class bombas : MonoBehaviour
{
    public static Action<int> Bombrec;
     private int Bomba=1;
    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            recolectar();
        }
    }
    private void recolectar()
    {
        Bombrec?.Invoke(Bomba);
        Destroy(gameObject);

    }
}
