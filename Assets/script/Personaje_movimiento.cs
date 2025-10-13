using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Personaje_movimiento : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public Transform groundCheck;
    public LayerMask groundLayer;


    private Rigidbody2D rb;
    private Animator Animator;
    private float moveInput;
    private bool Grounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        Animator.SetBool("caminar", moveInput != 0.0f);

        Debug.DrawRay(transform.position, Vector3.down * 0.64f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.64f))
        {
            Grounded = true;
        }
        else Grounded = false;

        if (Input.GetKeyDown(KeyCode.UpArrow) && Grounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }


}
