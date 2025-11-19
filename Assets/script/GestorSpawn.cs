using UnityEngine;
using System.Collections.Generic;

public static class GestorSpawn
{
    private static Dictionary<string, PuntoSpawn> puntosSpawn = new Dictionary<string, PuntoSpawn>();

    public static void RegistrarPuntoSpawn(PuntoSpawn punto)
    {
        if (punto != null)
        {
            puntosSpawn[punto.nombrePunto] = punto;
        }
    }

    public static void DesregistrarPuntoSpawn(PuntoSpawn punto)
    {
        if (punto != null && puntosSpawn.ContainsKey(punto.nombrePunto))
        {
            puntosSpawn.Remove(punto.nombrePunto);
        }
    }

    public static Vector3 ObtenerPosicionSpawn(string nombrePunto)
    {
        if (puntosSpawn.ContainsKey(nombrePunto))
        {
            return puntosSpawn[nombrePunto].transform.position;
        }
        
        Debug.LogWarning($"No se encontró el punto de spawn: {nombrePunto}");
        return Vector3.zero;
    }

    public static bool ExistePuntoSpawn(string nombrePunto)
    {
        return puntosSpawn.ContainsKey(nombrePunto);
    }
}
