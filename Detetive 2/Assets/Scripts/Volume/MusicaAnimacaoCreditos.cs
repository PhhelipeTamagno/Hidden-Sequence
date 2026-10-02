using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaAnimacaoCreditos : MonoBehaviour
{
    [Header("Musica")]
    public AudioSource audioSource;

    [Header("Cenas onde a musica continua")]
    public string cenaAnimacao = "Animacao";
    public string cenaCreditos = "Creditos";

    private static MusicaAnimacaoCreditos instancia;

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        if (cena.name != cenaAnimacao && cena.name != cenaCreditos)
        {
            if (audioSource != null)
                audioSource.Stop();

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }
}