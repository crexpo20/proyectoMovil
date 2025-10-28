using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class vidaPersonaje: MonoBehaviour
{
    public int vidamaxima = 3 ;
    public int vidaactual ;
    public Image[] VidaImagen;
  
    void Start() {
   
    }
    void actualizarinterface() {
        for (int i = 0; i < VidaImagen.Length; i++)
      {
            VidaImagen[i].enabled = i < vidamaxima;
        }
  

    }
  void reiniciarecena() {
            int curretSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(curretSceneIndex);    
       }
    public void hit()
    {
        vidamaxima = vidamaxima - 1;
        actualizarinterface();
        if (vidamaxima == 0)
        {
            Destroy(gameObject);

        }


    }
}
