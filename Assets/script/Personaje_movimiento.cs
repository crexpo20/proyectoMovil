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
    public float longitudCuerda = 4f;
    public LayerMask groundLayer;
    public float offsetDeteccion = 0.2f;
    public GameObject BombaPrefab;
    public GameObject cuerdaPrefab;
    public static System.Action<int> UsoBomba;
    public static System.Action<int> UsoCuerda;

    [Header("Ataque personaje")]
    public GameObject latigoPrefab;
    public float cooldownAtaque = 0.5f;
    public int danioLatigo = 1;
    private float cooldownTimer = 0f;


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

    [Tooltip("Permitir controles de teclado (para testing en PC)")]
    public bool permitirTeclado = true;
    
    [Header("Componentes")]
    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private Animator Animator;
    private float moveInput;
    [Header("Estado")]   
    private bool Grounded;
    private bool ladders;
    private bool isInvulnerable = false;
    private bool isKnockback = false;
    private bool atacando = false;
    private bool puedeAtacar = true;

    private SpriteRenderer spriteRenderer;

    public bool canDoubleJump = false;
    private bool hasDoubleJumped = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        movimiento_vertical();
        Climb();
        ActualizarCooldown();
        if (permitirTeclado)
        {
            if (Input.GetKeyDown(KeyCode.Space)) SaltarInstantaneo();
            if (Input.GetKeyDown(KeyCode.Z)) AtaqueInstantaneo();
            if (Input.GetKeyDown(KeyCode.C)) LanzarBombaInstantaneo();
            if (Input.GetKeyDown(KeyCode.X)) LanzarCuerdaInstantaneo();
        }
    }
    //------------Metodos para escena---------
    void reiniciarecena() {
        int curretSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(curretSceneIndex);    
    }

    //---------Metodos para cuerdas------------
    public void AddCuerdas(int Crecolectado)
    {
        cuerdas += Crecolectado;
        if (cuerdas < 0) cuerdas = 0;
    }
    private void ColocarCuerda()
    {
        if ((permitirTeclado && Input.GetKeyDown(KeyCode.X)))
        {
            if (cuerdas > 0)
            {
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
        if (permitirTeclado && Input.GetKeyDown(KeyCode.C) )
        {
            if (bombas > 0)
            {
                bomb();
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
        GameObject prebomb = Instantiate(BombaPrefab, transform.position + direction * 0.1f, Quaternion.identity);
        bomba_script bombaScript = prebomb.GetComponent<bomba_script>();
        if (bombaScript != null)
        {
            bombaScript.SetDireccion(direction);
        }
    }
    //----------Metodos para Ataque y daño ---------
    private void IniciarAtaque()
    {
        if ((permitirTeclado && Input.GetKeyDown(KeyCode.Z)) )
        {
            if (puedeAtacar && !atacando)
            {
                Atacar();
            }
        }
    }
    private void Atacar()
    {
        if (puedeAtacar && !atacando)
        {
            atacando = true;
            puedeAtacar = false;
            cooldownTimer = cooldownAtaque;
            
            // Reproducir animación de ataque
            if (Animator != null)
            {
                Animator.SetTrigger("atacar");
            }
            
            // Crear el látigo
            AtacarConLatigo();
            
        }
    }
    private void AtacarConLatigo()
    {
        if (latigoPrefab == null)
        {
            Debug.LogError("latigoPrefab no asignado!");
            return;
        }

        // Crear el látigo
        GameObject latigo = Instantiate(latigoPrefab);
        
        // Configurar el látigo - BUSCAR "Latigo" no "LatigoScript"
        Latigo latigoComponent = latigo.GetComponent<Latigo>();
        if (latigoComponent != null)
        {
            Vector2 direccionAtaque = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            latigoComponent.Configurar(direccionAtaque, transform.position);
        }
        else
        {
            Destroy(latigo);

        }
    }
    private void ActualizarCooldown()
{
    if (!puedeAtacar)
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            puedeAtacar = true;
            atacando = false;
        }
    }
}

    //----------Metodos para el daño recibido ----------------
    public int GetVida()
    {
        return vidamaxima;
    }
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

        
        bool inputSalto = (permitirTeclado && Input.GetKeyDown(KeyCode.Space));
        if (inputSalto && Grounded && !isKnockback)
        {
            rb.AddForce(Vector2.up * jumpForce);
            hasDoubleJumped = false;
        }
        else if (canDoubleJump && !hasDoubleJumped) 
        {
            rb.AddForce(Vector2.up *100f);
          
            hasDoubleJumped = true;
        }


        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    public void UnlockDoubleJump()
    {
        canDoubleJump = true;
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
    //--------metodos extra -------------
    public void SaltarInstantaneo()
    {
        if (Grounded && !isKnockback)
        {
            rb.AddForce(Vector2.up * jumpForce);
            hasDoubleJumped = false;
        }
        else if (canDoubleJump && !hasDoubleJumped)
        {
            rb.AddForce(Vector2.up * jumpForce * 0.9f);
            hasDoubleJumped = true;
        }
    }

    public void AtaqueInstantaneo()
    {
        if (puedeAtacar && !atacando)
        {
            Atacar();
        }
    }

    public void LanzarBombaInstantaneo()
    {
        if (bombas > 0)
        {
            bomb();
            bombas--;
            UsoBomba?.Invoke(-1);
        }
    }

    public void LanzarCuerdaInstantaneo()
    {
        if (cuerdas > 0)
        {
            LanzarCuerda();
            cuerdas--;
            UsoCuerda?.Invoke(-1);
        }
    }

    // === NUEVO: Limpiar listener del botón al destruir ===
    void OnDestroy()
    {

    }
}