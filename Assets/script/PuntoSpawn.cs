using UnityEngine;

public class PuntoSpawn : MonoBehaviour
{
    public string nombrePunto = "entrada";
    
    private void Awake()
    {
        // Registrar este punto de spawn en el gestor
        GestorSpawn.RegistrarPuntoSpawn(this);
    }

    private void OnDestroy()
    {
        // Desregistrar cuando se destruya
        GestorSpawn.DesregistrarPuntoSpawn(this);
    }
}