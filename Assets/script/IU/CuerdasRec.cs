using UnityEngine;
using TMPro;

public class CuerdasRec : MonoBehaviour
{
    [SerializeField] private int Cuerdgana;
    [SerializeField] private TextMeshProUGUI textcuerd;
    void Start()
    {
        actutex();
    }
    private void Sumarcuer(int cantcuer)
    {
        Cuerdgana += cantcuer;
        actutex();
    }
    private void actutex()
    {
        textcuerd.text = Cuerdgana.ToString();
    }
    private void OnEnable()
    {
        cuerdas.CuerdaRec += Sumarcuer;
    }
    private void OnDisable()
    {
        cuerdas.CuerdaRec -= Sumarcuer;
    }
}
