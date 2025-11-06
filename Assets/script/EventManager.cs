using UnityEngine;

public  class  EventManager : MonoBehaviour
{
    public static event System.Action<int> OnBombCollected;
    public static event System.Action<int> OnBombUsed;

    public static void BombCollected(int amount)
    {
        OnBombCollected?.Invoke(amount);
    }
    public static void BombUsed(int amount)
    {
        OnBombUsed?.Invoke(amount);
    }

}
