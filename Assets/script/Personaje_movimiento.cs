using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Personaje_movimiento : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float jumpForce = 300f;
    public float climbSpeerd = 1f;

    // === NUEVAS VARIABLES PARA CONTROLES TÁCTILES ===
    [Header("Controles Táctiles")]
    [Tooltip("Arrastra aquí el Fixed Joystick desde Hierarchy")]
    public FixedJoystick joystick;
    
    [Tooltip("Arrastra aquí el botón de salto cuando lo crees")]
    public Button botonSalto;
    
    [Tooltip("Permitir controles de teclado (para testing en PC)")]
    public bool permitirTeclado = true;

    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private Animator Animator;
    private Animator subida;
    private float moveInput;
    private bool Grounded;
    private bool ladders;
    private bool quiereSaltar = false; // Nueva variable para manejar salto desde botón

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        
        // Configurar botón de salto si existe
        if (botonSalto != null)
        {
            botonSalto.onClick.AddListener(OnBotonSaltoPresionado);
        }
    }

    void Update()
    {
        movimiento_vertical();
        Climb();
        CheckForLadders();
    }
    
    // === NUEVO MÉTODO: Se ejecuta cuando se presiona el botón de salto ===
    void OnBotonSaltoPresionado()
    {
        quiereSaltar = true;
    }

    private void movimiento_vertical()
    {
        // === MODIFICADO: Ahora obtiene input del joystick O teclado ===
        moveInput = ObtenerInputHorizontal();
        
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        Animator.SetBool("caminar", moveInput != 0.0f);

        Debug.DrawRay(transform.position, Vector3.down * 0.64f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.64f))
        {
            Grounded = true;
        }
        else Grounded = false;

        // === MODIFICADO: Salto ahora funciona con teclado Y botón táctil ===
        bool inputSalto = (permitirTeclado && Input.GetKeyDown(KeyCode.Space)) || quiereSaltar;
        
        if (inputSalto && Grounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            quiereSaltar = false; // Resetear después de usar
        }

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
    
    // === NUEVO MÉTODO: Obtiene input horizontal del joystick o teclado ===
    private float ObtenerInputHorizontal()
    {
        float input = 0f;
        
        // Prioridad al joystick si existe
        if (joystick != null)
        {
            input = joystick.Horizontal;
        }
        
        // Fallback a teclado si no hay input del joystick
        if (permitirTeclado && Mathf.Abs(input) < 0.1f)
        {
            input = Input.GetAxisRaw("Horizontal");
        }
        
        return input;
    }
    
    private void Climb()
    {
        if (!ladders) {
            rb.gravityScale = 1.5f;
            return; 
        }
        
        // === MODIFICADO: Escaleras también usan joystick (dirección vertical) ===
        float getDirection = ObtenerInputVertical();
        
        if (ladders && getDirection != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeerd * getDirection);
            rb.gravityScale = 0; 
        }
    }
    
    // === NUEVO MÉTODO: Obtiene input vertical para escaleras ===
    private float ObtenerInputVertical()
    {
        float input = 0f;
        
        // Prioridad al joystick
        if (joystick != null)
        {
            input = joystick.Vertical;
        }
        
        // Fallback a teclado
        if (permitirTeclado && Mathf.Abs(input) < 0.1f)
        {
            input = Input.GetAxis("Vertical");
        }
        
        return input;
    }
    
    private void CheckForLadders()
    {
        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("ladders")))
        {
            Animator.SetBool("isClimbing", true);
            ladders = true;
        }
        else
        {
            Animator.SetBool("isClimbing", false);
            ladders = false;
        }
    }
    
    // === NUEVO: Limpiar listener del botón al destruir ===
    void OnDestroy()
    {
        if (botonSalto != null)
        {
            botonSalto.onClick.RemoveListener(OnBotonSaltoPresionado);
        }
    }
}