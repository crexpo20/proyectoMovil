using UnityEngine;
using TMPro;

public class bombRec : MonoBehaviour
{
    [SerializeField] private int Bomgana;
    [SerializeField] private TextMeshProUGUI textoBom;
    void Start()
    {
        actutex();
    }
    private void Sumarbomb(int cantBom)
    {
        Bomgana += cantBom;
        actutex();
    }
    private void actutex()
    {
        textoBom.text = Bomgana.ToString();
    }
    private void OnEnable()
    {
        bombas.Bombrec += Sumarbomb;
    }
    private void OnDisable()
    {
        bombas.Bombrec -= Sumarbomb;
    }
}
