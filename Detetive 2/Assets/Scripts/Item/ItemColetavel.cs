using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    public enum TipoItem
    {
        Faca,
        GarrafaTermica
    }

    [Header("Tipo do item")]
    public TipoItem tipoItem;

    [Header("Distância para coletar")]
    public float distancia = 3f;

    private Transform jogador;
    private ItensEncontrados itensEncontrados;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            jogador = player.transform;

        itensEncontrados = FindObjectOfType<ItensEncontrados>();
    }

    void Update()
    {
        if (jogador == null || itensEncontrados == null)
            return;

        float distanciaAtual = Vector3.Distance(
            transform.position,
            jogador.position
        );

        if (distanciaAtual <= distancia &&
            Input.GetKeyDown(KeyCode.E))
        {
            Coletar();
        }
    }

    void Coletar()
    {
        if (tipoItem == TipoItem.GarrafaTermica)
        {
            itensEncontrados.EncontrarGarrafa();
        }

        if (tipoItem == TipoItem.Faca)
        {
            itensEncontrados.EncontrarFaca();
        }

        Destroy(gameObject);
    }
}