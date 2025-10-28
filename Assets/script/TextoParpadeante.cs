using UnityEngine;
using TMPro;

/// <summary>
/// Hace que un texto parpadee (fade in/out) de forma suave
/// Perfecto para textos de "Presiona cualquier tecla"
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextoParpadeante : MonoBehaviour
{
    [Header("Configuración del Parpadeo")]
    [Tooltip("Velocidad del parpadeo (mayor = más rápido)")]
    [Range(0.5f, 5f)]
    public float velocidadParpadeo = 1.5f;
    
    [Tooltip("Transparencia mínima (0 = invisible, 1 = opaco)")]
    [Range(0f, 1f)]
    public float alphaMinimo = 0.2f;
    
    [Tooltip("Transparencia máxima")]
    [Range(0f, 1f)]
    public float alphaMaximo = 1f;
    
    private TextMeshProUGUI texto;
    private Color colorOriginal;
    private float tiempoTranscurrido = 0f;
    
    void Start()
    {
        texto = GetComponent<TextMeshProUGUI>();
        colorOriginal = texto.color;
    }
    
    void Update()
    {
        // Incrementar tiempo
        tiempoTranscurrido += Time.deltaTime * velocidadParpadeo;
        
        // Calcular alpha usando una curva suave (seno)
        float alpha = Mathf.Lerp(alphaMinimo, alphaMaximo, 
                                 (Mathf.Sin(tiempoTranscurrido) + 1f) / 2f);
        
        // Aplicar nuevo color con alpha modificado
        Color nuevoColor = colorOriginal;
        nuevoColor.a = alpha;
        texto.color = nuevoColor;
    }
}