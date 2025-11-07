using UnityEngine;
using TMPro;

public class CuerdasRec : MonoBehaviour
{
    [SerializeField] private int Cuerdgana;
    [SerializeField] private Personaje_movimiento personaje;
    [SerializeField] private TextMeshProUGUI textcuerd;
    void Start()
    {
        Cuerdgana = personaje.GetRopeCount();
        actutex();
    }
    private void Sumarcuer(int cantcuer)
    {
        Cuerdgana += cantcuer;
        personaje.SetRopeCount(cantcuer);
        if (Cuerdgana < 0) Cuerdgana = 0;
        actutex();
    }
    private void UsarCuerda(int cantcuer)
    {
        Cuerdgana += cantcuer; 
        if (Cuerdgana < 0) Cuerdgana = 0;
        actutex();
    }
    private void actutex()
    {
        textcuerd.text = Cuerdgana.ToString();
    }
    private void OnEnable()
    {
        cuerdas.CuerdaRec += Sumarcuer;
        Personaje_movimiento.UsoCuerda += UsarCuerda;
    }
    private void OnDisable()
    {
        cuerdas.CuerdaRec -= Sumarcuer;
        Personaje_movimiento.UsoCuerda -= UsarCuerda;
    }
}
