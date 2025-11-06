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
     private int Bomba=2;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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
