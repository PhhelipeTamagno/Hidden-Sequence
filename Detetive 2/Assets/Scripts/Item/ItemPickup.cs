using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item")]
    public string nomeItem = "Digital";
    public Sprite icone;

    [Header("Configuração")]
    public KeyCode teclaColeta = KeyCode.E;

    [Header("Item especial")]
    public bool eDigital = false;

    [Header("Visual que será destruído")]
    public GameObject visualParaDestruir;

    private bool jogadorPerto = false;
    private Hotbar hotbar;
    private ItensEncontrados itensEncontrados;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        itensEncontrados = FindFirstObjectByType<ItensEncontrados>();
    }

    void Update()
    {
        
        if (jogadorPerto && Input.GetKeyDown(teclaColeta))
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
            hotbar = FindFirstObjectByType<Hotbar>();

        jogadorPerto = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
        }
    }

    void TentarColetar()
    {
        if (hotbar == null)
        {
            Debug.LogWarning("Hotbar não encontrada!");
            return;
        }
        bool coletou = hotbar.AdicionarItem(
            nomeItem,
            icone
        );
      
        if (!coletou)
        {
            Debug.Log("Hotbar cheia! Não foi possível pegar: " + nomeItem);
            return;
        }
        if (eDigital && itensEncontrados != null)
        {
            itensEncontrados.EncontrarDigital();
        }
        if (visualParaDestruir != null)
        {
            Destroy(visualParaDestruir);
        }
        Destroy(gameObject);
    }
}