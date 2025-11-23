using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Configuración de Audio")]
    public float volumenGeneral = 1f;
    public float volumenMusica = 1f;
    public float volumenSFX = 1f;

    [Header("Música de Fondo")]
    public AudioClip musicaMenu;
    public AudioClip musicaGameplay;
    public AudioClip musicaVictoria;
    public AudioClip musicaDerrota;
    
    private AudioSource fuenteMusica;

    [Header("Efectos de Sonido")]
    public AudioClip sonidoSalto;
    public AudioClip sonidoAtaque;
    public AudioClip sonidoBomba;
    public AudioClip sonidoCuerda;
    public AudioClip sonidoMoneda;
    public AudioClip sonidoDaño;
    public AudioClip sonidoMuerte;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoBoton;

    private AudioSource fuenteSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Crear fuentes de audio
            fuenteMusica = gameObject.AddComponent<AudioSource>();
            fuenteSFX = gameObject.AddComponent<AudioSource>();
            
            // Configurar fuente de música
            fuenteMusica.loop = true;
            fuenteMusica.volume = volumenGeneral * volumenMusica;
            
            // Configurar fuente de SFX
            fuenteSFX.volume = volumenGeneral * volumenSFX;
            
            // Reproducir música del menú al inicio
            ReproducirMusica(musicaMenu);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnEscenaCargada;
    }
    #region MÉTODOS DE MÚSICA

    private void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        // Cambiar música según la escena cargada
        CambiarMusicaPorEscena(escena.name);
    }
    public void CambiarMusicaPorEscena(string nombreEscena)
    {
        if (nombreEscena.Contains("Inicio") || nombreEscena == "MenuPrincipal")
        {
            ReproducirMusica(musicaMenu);
        }
        else if (nombreEscena.Contains("Cueva_1") || nombreEscena.Contains("Tutorial"))
        {
            ReproducirMusica(musicaGameplay);
        }
    }
    public void ReproducirMusica(AudioClip clip)
    {
        if (clip == null || fuenteMusica.clip == clip) return;
        
        fuenteMusica.Stop();
        fuenteMusica.clip = clip;
        fuenteMusica.Play();
    }


    public void PausarMusica()
    {
        fuenteMusica.Pause();
    }

    public void ReanudarMusica()
    {
        fuenteMusica.Play();
    }

    public void DetenerMusica()
    {
        fuenteMusica.Stop();
    }
    #endregion

    #region MÉTODOS DE EFECTOS DE SONIDO
    public void ReproducirSFX(AudioClip clip, float volumenPersonalizado = 1f)
    {
        if (clip == null) return;
        
        fuenteSFX.PlayOneShot(clip, volumenGeneral * volumenSFX * volumenPersonalizado);
    }

    // Métodos específicos para sonidos comunes
    public void ReproducirSalto() => ReproducirSFX(sonidoSalto);
    public void ReproducirAtaque() => ReproducirSFX(sonidoAtaque);
    public void ReproducirBomba() => ReproducirSFX(sonidoBomba);
    public void ReproducirCuerda() => ReproducirSFX(sonidoCuerda);
    public void ReproducirMoneda() => ReproducirSFX(sonidoMoneda);
    public void ReproducirDaño() => ReproducirSFX(sonidoDaño);
    public void ReproducirMuerte() => ReproducirSFX(sonidoMuerte);
    public void ReproducirVictoria() => ReproducirSFX(sonidoVictoria);
    public void ReproducirBoton() => ReproducirSFX(sonidoBoton);
    #endregion

    #region CONFIGURACIÓN DE VOLUMEN
    public void SetVolumenGeneral(float volumen)
    {
        volumenGeneral = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }

    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }

    public void SetVolumenSFX(float volumen)
    {
        volumenSFX = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }

    private void ActualizarVolumenes()
    {
        fuenteMusica.volume = volumenGeneral * volumenMusica;
        fuenteSFX.volume = volumenGeneral * volumenSFX;
    }
    #endregion
}