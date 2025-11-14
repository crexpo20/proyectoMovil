using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Effect")]
public class pewerEfecct : ScriptableObject
{
    public string effectName;
    public float duration = 5f;
    public float multiplier = 2f;

    public void Apply(GameObject target)
    {
        Personaje_movimiento player = target.GetComponent<Personaje_movimiento>();
        if (player != null)
        {
            if (effectName == "SpeedBoost")
            {
               // player.IncreaseSpeed(multiplier, duration);
            }
            else if (effectName == "DoubleJump")
            {
                player.EnableDoubleJump(duration);
            }
            else if (effectName == "WallFriction")
            {
                player.EnableWallClimb(duration);
            }
        }
    }
}
