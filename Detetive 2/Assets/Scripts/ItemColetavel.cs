using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    public enum TipoItem
    {
        Faca,
        GarrafaTermica
    }

    [Header("Item")]
    public TipoItem tipoItem;

    [Header("Ícone da Hotbar")]
    public Sprite icone;

    [Header("Interação")]
    public float distancia = 3f;

    private Transform jogador;
    private Hotbar hotbar;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            jogador = player.transform;
            hotbar = FindObjectOfType<Hotbar>();
        }
    }

    void Update()
    {
        if (jogador == null || hotbar == null)
            return;

        float distanciaAtual =
            Vector3.Distance(transform.position, jogador.position);

        if (distanciaAtual <= distancia &&
            Input.GetKeyDown(KeyCode.E))
        {
            Coletar();
        }
    }

    void Coletar()
    {
        string nome = "";

        if (tipoItem == TipoItem.Faca)
            nome = "Faca";

        if (tipoItem == TipoItem.GarrafaTermica)
            nome = "GarrafaTermica";

        bool conseguiuAdicionar =
            hotbar.AdicionarItem(nome, icone);

        if (conseguiuAdicionar)
        {
            Destroy(gameObject);
        }
    }
}