using UnityEngine;
using TMPro;

public class sistemademone : MonoBehaviour
{
    
    [SerializeField] private int oroganado;
    [SerializeField] private TextMeshProUGUI textomoneda;
    void Start() {
        actutex();
    }
    private void Sumardiaman(int cantoro)
    {
        oroganado += cantoro;
        actutex();
    }
    private void actutex()
    {
textomoneda.text = oroganado.ToString(); 
    }
    private void OnEnable() {
        moneda.Ororeco += Sumardiaman;
    }
    private void OnDisable() {
        moneda.Ororeco -= Sumardiaman;
    }
}