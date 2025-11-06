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
    public int cuerdas = 0;
    public float longitudCuerda = 4f; // 5 bloques
    public LayerMask groundLayer;
    public float offsetDeteccion = 0.2f;
    public GameObject BombaPrefab;
    public GameObject cuerdaPrefab;
    public static System.Action<int> UsoBomba;
    public static System.Action<int> UsoCuerda;



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
    private bool lanzarBomba = false;
    private bool lanzarCuerdas = false;
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (botonSalto != null) botonSalto.onClick.AddListener(OnBotonSaltoPresionado);

        if (botonBomba != null) botonBomba.onClick.AddListener(OnBotonBombaPresionado);

        if (botonCuerda != null) botonCuerda.onClick.AddListener(OnBotonCuerdaPresionado);
        
    }

    void Update()
    {
        movimiento_vertical();
        Climb();
        ColocarBomba();
        ColocarCuerda();
    }
    //------------Metodos para escena---------
    void reiniciarecena() {
        int curretSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(curretSceneIndex);    
    }

    // ----------Metododos para  botones----------
    void OnBotonSaltoPresionado()
    {
        quiereSaltar = true;
    }
    void OnBotonBombaPresionado()
    {
        lanzarBomba = true;
    }
    void OnBotonCuerdaPresionado()
    {
        lanzarCuerdas = true;
    }
    //---------Metodos para cuerdas------------
    public void AddCuerdas(int Crecolectado)
    {
        cuerdas += Crecolectado;
        if (cuerdas < 0) cuerdas = 0;
    }
    private void ColocarCuerda()
    {
        if ((permitirTeclado && Input.GetKeyDown(KeyCode.X)) || lanzarCuerdas)
        {
            if (cuerdas > 0)
            {
                LanzarCuerda();
                lanzarCuerdas = false;
                cuerdas--;
                UsoCuerda?.Invoke(-1);
            }
        }
    }
    private void LanzarCuerda()
    {
        if (cuerdaPrefab == null)
        {
            return;
        }
        GameObject cuerda = Instantiate(cuerdaPrefab, transform.position, Quaternion.identity);
        CuerdaScript cuerdaScript = cuerda.GetComponent<CuerdaScript>();
        if (cuerdaScript != null)
        {
            cuerdaScript.GenerarCuerdaDesdePersonaje(transform.position);
        }
        else
        {
            Destroy(cuerda);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.up * longitudCuerda);
    }
    public int GetRopeCount()
    {
        return cuerdas;
    }
    public void SetRopeCount(int cantidad)
    {
        cuerdas += cantidad;
    }

    //---------Metodos para bombas--------------
    public void AddBombs(int Brecolectado)
    {
        bombas += Brecolectado;
        if (bombas < 0) bombas = 0;

    }
    private void ColocarBomba()
    {
        if (permitirTeclado && Input.GetKeyDown(KeyCode.C) || lanzarBomba)
        {
            if (bombas > 0)
            {
                bomb();
                lanzarBomba = false;
                bombas--;
                UsoBomba?.Invoke(-1);
            }
        }
    }
    public int GetBombCount()
    {
        return bombas;
    }
    public void SetBombCount(int cantidad)
    {
        bombas += cantidad;
    }
     private void bomb()
    {
        Vector3 direction;
        if (transform.localScale.x == 1) direction = Vector2.right;
        else direction = Vector2.left;
        GameObject prebomb = Instantiate(BombaPrefab, transform.position + direction *0.1f, Quaternion.identity);
        bomba_script bombaScript = prebomb.GetComponent<bomba_script>();
        if (bombaScript != null)
        {
            bombaScript.SetDireccion(direction);
        }
    }


    //----------Metodos para el daño ----------------
    public void hit()
    {
        RecibirDaño(1, transform.position + Vector3.left);
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


    
    //-----------Metodos para movimiento--------------
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
    
    //-------------Metodo para esacalar----------------
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
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeerd * getDirection);
            Animator.SetBool("isClimbing", true);
        }
        else
        {
            rb.gravityScale = 0.2f; 
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

    //---------- Colisiones ---------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy") || other.CompareTag("tramp"))
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
            Animator.SetBool("isClimbing", false);
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