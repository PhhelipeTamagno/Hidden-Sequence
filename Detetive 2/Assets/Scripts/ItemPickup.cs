using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item")]
    public string nomeItem = "Item";
    public Sprite icone;

    [Header("Configuração")]
    public bool coletaAutomatica = true; // true = encosta e pega / false = precisa apertar tecla
    public KeyCode teclaColeta = KeyCode.E;

    private bool jogadorPerto = false;
    private Hotbar hotbar;

    void Start()
    {
        // Garante que o collider é trigger
        GetComponent<Collider>().isTrigger = true;
    }

    void Update()
    {
        if (!coletaAutomatica && jogadorPerto && Input.GetKeyDown(teclaColeta))
        {
            TentarColetar();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        hotbar = other.GetComponentInChildren<Hotbar>();
        if (hotbar == null)
            hotbar = FindObjectOfType<Hotbar>(); // fallback, caso a Hotbar não esteja no player

        jogadorPerto = true;

        if (coletaAutomatica)
        {
            TentarColetar();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = false;
    }

    void TentarColetar()
    {
        if (hotbar == null)
            return;

        bool coletou = hotbar.AdicionarItem(nomeItem, icone);

        if (coletou)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Hotbar cheia! Não foi possível pegar: " + nomeItem);
            // Aqui você pode adicionar um popup tipo "Inventário cheio" se quiser
        }
    }
}