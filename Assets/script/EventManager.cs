using UnityEngine;

public  class  EventManager : MonoBehaviour
{
    public  static event System.Action<int> OnBombCollected;

    public static void BombCollected(int amount)
    {
        OnBombCollected?.Invoke(amount);
    }
}
