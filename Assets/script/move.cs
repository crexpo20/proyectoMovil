using UnityEngine;

public class move : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Vector2 Control;
    public Joystick joy;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 mover = new Vector2(joy.Horizontal,joy.Vertical);
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Control * Time.deltaTime);
    }
}
