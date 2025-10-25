using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Personaje_movimiento : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float jumpForce = 300f;
    public float climbSpeerd = 1f; 

    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private Animator Animator;
    private Animator subida;
    private float moveInput;
    private bool Grounded;
    private bool ladders;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        movimiento_vertical();
        Climb();
        CheckForLadders();
    }
    private void movimiento_vertical()
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

        if (Input.GetKeyDown(KeyCode.Space) && Grounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

    }
    private void Climb()
    {
        if (!ladders) {
            rb.gravityScale = 1.5f;
            return; 
            }
        var getDirection = Input.GetAxis("Vertical");
        if (ladders && Input.GetAxis("Vertical") !=0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeerd * getDirection);
            rb.gravityScale = 0; 
        }
    }
    private void CheckForLadders()
    {
        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("ladders")))
        {
            Animator.SetBool("isClimbing",true);
            ladders = true;

        }
        else
        {
            Animator.SetBool("isClimbing",false);
            ladders = false;
        }
    }
}
