using UnityEngine;
using UnityEngine.EventSystems;

public class BotonAtaque : MonoBehaviour, IPointerDownHandler
{
    public Personaje_movimiento personaje;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (personaje != null)
            personaje.AtaqueInstantaneo();
    }
}
