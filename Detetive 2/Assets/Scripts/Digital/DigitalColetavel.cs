using UnityEngine;

public class DigitalColetavel : MonoBehaviour
{
    [Header("Referências")]
    public Light luzUV;

    [Header("Configuração")]
    public float distanciaParaEncontrar = 3f;
    public float distanciaParaColetar = 2f;

    [Header("Visual")]
    public GameObject digitalVisual;

    private Transform jogador;
    private ItensEncontrados itensEncontrados;

    private bool encontrada = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            jogador = player.transform;

        itensEncontrados = FindObjectOfType<ItensEncontrados>();

        // Começa escondida
        if (digitalVisual != null)
            digitalVisual.SetActive(false);
    }

    void Update()
    {
        if (jogador == null || itensEncontrados == null || luzUV == null)
            return;

        float distanciaJogador = Vector3.Distance(
            transform.position,
            jogador.position
        );

        float distanciaUV = Vector3.Distance(
            transform.position,
            luzUV.transform.position
        );

        // A UV precisa estar perto da digital
        bool iluminadaPelaUV =
            distanciaUV <= distanciaParaEncontrar;

        // Se a UV encontrou a digital
        if (!encontrada && iluminadaPelaUV)
        {
            MostrarDigital();
        }

        // Depois que apareceu, o jogador pode coletar
        if (encontrada &&
            distanciaJogador <= distanciaParaColetar &&
            Input.GetKeyDown(KeyCode.E))
        {
            Coletar();
        }
    }

    void MostrarDigital()
    {
        encontrada = true;

        if (digitalVisual != null)
            digitalVisual.SetActive(true);
    }

    void Coletar()
    {
        itensEncontrados.EncontrarDigital();

        Destroy(gameObject);
    }
}