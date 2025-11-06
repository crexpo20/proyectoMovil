using UnityEngine;
using TMPro;

public class bombRec : MonoBehaviour
{
    [SerializeField] private int Bomgana;
    [SerializeField] private Personaje_movimiento personaje;
    [SerializeField] private TextMeshProUGUI textoBom;
    void Start()
    {
        Bomgana = personaje.GetBombCount();
        actutex();
    }
    private void Sumarbomb(int cantBom)
    {
        Bomgana += cantBom;
        personaje.SetBombCount(cantBom);
        if (Bomgana < 0) Bomgana = 0;
        actutex();
    }
    private void UsarBomba(int cantBom)
    {
        Bomgana += cantBom; 
        if (Bomgana < 0) Bomgana = 0;
        actutex();
    }
    private void actutex()
    {
        textoBom.text = Bomgana.ToString();
        
    }
    private void OnEnable()
    {
        bombas.Bombrec += Sumarbomb;
        Personaje_movimiento.UsoBomba += UsarBomba;
        
    }
    private void OnDisable()
    {
        bombas.Bombrec -= Sumarbomb;
        Personaje_movimiento.UsoBomba -= UsarBomba;
        
    }
    
}
