using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class vidaPrueba : MonoBehaviour
{
    public Personaje_movimiento Vidajugador;
    public TextMeshProUGUI textMesh;
    void Start()
    {
        //Vidajugador = gameObject.Find("personaje").GetComponent<Personaje_movimiento>();
    }
    void Update()
    {
        if(Vidajugador != null && textMesh != null)
        {
            textMesh.text = Vidajugador.vidamaxima.ToString();
        }
    }
    
}
