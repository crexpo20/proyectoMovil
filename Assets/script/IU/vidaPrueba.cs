using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class vidaPrueba : MonoBehaviour
{
    public int vidamaxima = 3;
    
    public TextMeshProUGUI textMesh;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        textMesh.text = vidamaxima.ToString("3");
    }
    public void danio()
    {
        vidamaxima = vidamaxima - 1;
    }
}
