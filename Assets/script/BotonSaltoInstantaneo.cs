using UnityEngine;
using UnityEngine.EventSystems;

public class BotonSaltoInstantaneo : MonoBehaviour, IPointerDownHandler
{
     public Personaje_movimiento personaje;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (personaje != null)
            personaje.SaltarInstantaneo();
    }
}
