using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaAnimacaoCreditos : MonoBehaviour
{
    [Header("Musica")]
    public AudioSource audioSource;

    [Header("Cenas onde a musica continua")]
    public string cenaAnimacao = "Animacao";
    public string cenaCreditos = "Creditos";

    [Header("Carro")]
    public Rigidbody carroRigidbody;

    private static MusicaAnimacaoCreditos instancia;

    private bool encostouNaParede = false;

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

        if (carroRigidbody == null)
        {
            carroRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if (carroRigidbody == null || !encostouNaParede)
            return;

        Vector3 velocidade = carroRigidbody.linearVelocity;

        float velocidadeParaFrente = Vector3.Dot(velocidade, transform.forward);

        if (velocidadeParaFrente > 0f)
        {
            velocidade -= transform.forward * velocidadeParaFrente;
            carroRigidbody.linearVelocity = velocidade;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.ToLower() == "parede")
        {
            encostouNaParede = true;

            if (carroRigidbody != null)
            {
                Vector3 velocidade = carroRigidbody.linearVelocity;

                float velocidadeParaFrente = Vector3.Dot(velocidade, transform.forward);

                if (velocidadeParaFrente > 0f)
                {
                    velocidade -= transform.forward * velocidadeParaFrente;
                    carroRigidbody.linearVelocity = velocidade;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name.ToLower() == "parede")
        {
            encostouNaParede = false;
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