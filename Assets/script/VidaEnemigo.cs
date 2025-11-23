using UnityEngine;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Vida enemigos")]
    [SerializeField] private int VidaActual = 1;

    [Header("Colliders de danio")]
    [SerializeField] private Collider2D colliderAtaque;
    [SerializeField] private Collider2D colliderSalto;
    
    [Header("Daño por Tipo")]
    [SerializeField] private int danioPorAtaque = 1;
    [SerializeField] private int danioPorSalto = 1;

    private bool estaMuerto = false;

    void Start()
    {
        if (colliderAtaque == null)
            colliderAtaque = GetComponent<Collider2D>();
            
        if (colliderSalto == null)
            colliderSalto = GetComponent<Collider2D>();
    }

    public void RecibirDanioAtaque()
    {
        VidaActual -= danioPorAtaque;
        if (VidaActual <= 0)
        {
            Morir();
        }
    }

    public void RecibirDanioSalto()
    {
        VidaActual -= danioPorSalto;
        if (VidaActual <= 0)
        {
            Morir();
        }
    }
    private void Morir()
    {
        Destroy(gameObject);
    }
    public int GetVidaActual()
    {
        return VidaActual;
    }
    
}
