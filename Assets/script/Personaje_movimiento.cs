using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Personaje_movimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 20f;
    public float jumpForce = 300f;
    public float climbSpeerd = 3f;

    [Header("Items")]
    public int bombas = 0;
    public int cuerdas = 3;

    [Header("vida y danio UI")]
    public int vidamaxima = 3;
    [Tooltip("Fuerza del retroceso aplica en la direccion opuesta")]
    public float knockbackforce = 8f;
    [Tooltip("Duracion del retroceso en segundos ")]
    public float knockbackDuration = 0.25f;
    [Tooltip("Duracion de la inviulnerabilidad en segundos despues del golpe")]
    public float invulnerabilityDuration = 1.0f;
    [Tooltip("Frecuencia de parpadeo durante la invulnerabilidad")]
    public float flashFrequency = 10f;

    [Header("Controles Táctiles")]
    [Tooltip("Arrastra aquí el Fixed Joystick")]
    public FixedJoystick joystick;
    [Tooltip("Arrastra aquí el botón de salto")]
    public Button botonSalto;
    [Tooltip("Arrastra aquí el botón de atque")]
    public Button botonAtaque;
    [Tooltip("Arrastra aquí el botón de bomba")]
    public Button botonBomba;
    [Tooltip("Arrastra aquí el botón de cuerda")]
    public Button botonCuerda;

    [Tooltip("Permitir controles de teclado (para testing en PC)")]
    public bool permitirTeclado = true;
    
    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private Animator Animator;
    private Animator subida;
    private float moveInput;
    private bool Grounded;
    private bool ladders;
    private bool isInvulnerable = false;
    private bool isKnockback = false;
    private bool quiereSaltar = false;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        EventManager.OnBombCollected += AddBombs;

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
   
    public void AddBombs(int Brecolectado)
    {
        bombas += Brecolectado;
        Debug.Log("Bombas totales: " + bombas);
    }
    void reiniciarecena() {
        int curretSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(curretSceneIndex);    
    }

    public void hit()
    {
        RecibirDaño(1, transform.position + Vector3.left);
    }
    
    void OnBotonSaltoPresionado()
    {
        quiereSaltar = true;
    }

    private void movimiento_vertical()
    {
        if (!isKnockback)
        {
            moveInput = ObtenerInputHorizontal();
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            Animator.SetBool("caminar", Mathf.Abs(moveInput) > 0.01f);
        }
        
        Debug.DrawRay(transform.position, Vector3.down * 0.64f, Color.red);
        Grounded = Physics2D.Raycast(transform.position, Vector3.down, 0.64f);

        // === MODIFICADO: Salto ahora funciona con teclado Y botón táctil ===
         bool inputSalto = (permitirTeclado && Input.GetKeyDown(KeyCode.Space)) || quiereSaltar;
        if (inputSalto && Grounded && !isKnockback)
        {
            rb.AddForce(Vector2.up * jumpForce);
            quiereSaltar = false;
        }
        else quiereSaltar = false;

        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }
    
    // === NUEVO MÉTODO: Obtiene input horizontal del joystick o teclado ===
    private float ObtenerInputHorizontal()
    {
        float input = 0f;
        if (joystick != null)
            input = joystick.Horizontal;
        if (permitirTeclado && Mathf.Abs(input) < 0.1f)
            input = Input.GetAxisRaw("Horizontal");
        return input;
    }
    
    private void Climb()
    {
        if (!ladders) 
        {
            rb.gravityScale = 1.5f;
            Animator.SetBool("isClimbing", false);
            return; 
        }
        
        float getDirection = ObtenerInputVertical();
        
        if (Mathf.Abs(getDirection) > 0.1f)
        {
            // Trepando activamente
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeerd * getDirection);
            Animator.SetBool("isClimbing", true);
        }
        else
        {
            // En escalera pero quieto - pequeño ajuste para evitar que se quede pegado
            rb.gravityScale = 0.2f; // Gravedad reducida para que pueda bajar naturalmente
            Animator.SetBool("isClimbing", false);
        }
    }
    
    // === NUEVO MÉTODO: Obtiene input vertical para escaleras ===
    private float ObtenerInputVertical()
    {
        float input = 0f;
        
        if (joystick != null)
        {
            input = joystick.Vertical;
        }
        
        if (permitirTeclado && Mathf.Abs(input) < 0.1f)
        {
            input = Input.GetAxis("Vertical");
        }
        
        return input;
    }

    private void CheckForLadders()
    {
        Collider2D[] hitColliders = new Collider2D[10];
        int numColliders = boxCollider.Overlap(new ContactFilter2D().NoFilter(), hitColliders);
        
        bool tocandoEscalera = false;
        
        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i] != null && hitColliders[i].CompareTag("ladders"))
            {
                tocandoEscalera = true;
                break;
            }
        }
        
        ladders = tocandoEscalera;
        
        // La animación depende si está en escaleras Y moviéndose
        float inputVertical = ObtenerInputVertical();
        bool estaTrepando = tocandoEscalera && Mathf.Abs(inputVertical) > 0.1f;
        Animator.SetBool("isClimbing", estaTrepando);
    }

    public void RecibirDaño(int daño, Vector2 fuentePos)
    {
        if (isInvulnerable) return;

        vidamaxima -= daño;

        if (vidamaxima <= 0)
        {
            if (Animator != null) Animator.SetTrigger("die");
            enabled = false;
            StartCoroutine(DelayAndReload(1.0f));
            return;
        }

        Vector2 direccion = ((Vector2)transform.position - fuentePos).normalized;
        direccion.y = Mathf.Clamp(direccion.y + 0.5f, -1f, 1f);

        StartCoroutine(ProcesoKnockbackYInvulnerable(direccion));

        //if (Animator != null) Animator.SetTrigger("hit");
    }

    private IEnumerator DelayAndReload(float delay)
    {
        yield return new WaitForSeconds(delay);
        reiniciarecena();
    }

    private IEnumerator ProcesoKnockbackYInvulnerable(Vector2 direccion)
    {
        isKnockback = true;
        isInvulnerable = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccion * knockbackforce, ForceMode2D.Impulse);

        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isKnockback = false;

        float invElapsed = 0f;
        if (spriteRenderer != null)
        {
            while (invElapsed < invulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                invElapsed += (1f / flashFrequency);
                yield return new WaitForSeconds(1f / flashFrequency);
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityDuration);
        }

        isInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            RecibirDaño(1, other.transform.position);
        }
        else if (other.CompareTag("ladders"))
        {
            ladders = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ladders"))
        {
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
        EventManager.OnBombCollected -= AddBombs;
    }
}