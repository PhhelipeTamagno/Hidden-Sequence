using UnityEngine;

public class CarroInteragir : MonoBehaviour
{
    [Header("Visual de Destaque")]
    public Renderer[] renderers;
    public Color corBrilho = Color.yellow;
    [Range(0f, 5f)]
    public float intensidadeBrilho = 1.5f;

    [Header("Direção")]
    public CarroController carroController;

    [Header("Câmera")]
    public Camera cameraCarro;

    private MaterialPropertyBlock propertyBlock;
    private bool destacado = false;

    private GameObject playerAtual;
    private PlayerMovement playerMovementAtual;
    private PlayerInteracao playerInteracaoAtual;
    private Camera cameraPlayerAtual;
    private bool playerDentro = false;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (renderers == null || renderers.Length == 0 || ContemNulos(renderers))
            renderers = GetComponentsInChildren<Renderer>();

        if (carroController != null)
            carroController.enabled = false;

        if (cameraCarro != null)
            cameraCarro.gameObject.SetActive(false);
    }

    bool ContemNulos(Renderer[] array)
    {
        foreach (var r in array)
        {
            if (r == null) return true;
        }
        return false;
    }

    public void Destacar()
    {
        if (destacado) return;
        destacado = true;

        foreach (var rend in renderers)
        {
            if (rend == null) continue;
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_EmissionColor", corBrilho * intensidadeBrilho);
            rend.SetPropertyBlock(propertyBlock);
        }
    }

    public void RemoverDestaque()
    {
        if (!destacado) return;
        destacado = false;

        foreach (var rend in renderers)
        {
            if (rend == null) continue;
            rend.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_EmissionColor", Color.black);
            rend.SetPropertyBlock(propertyBlock);
        }
    }

    public void EntrarCarro(GameObject player)
    {
        if (playerDentro) return;
        playerDentro = true;

        playerAtual = player;
        playerMovementAtual = player.GetComponent<PlayerMovement>();
        playerInteracaoAtual = player.GetComponent<PlayerInteracao>();

        RemoverDestaque();

        if (playerMovementAtual != null)
            playerMovementAtual.enabled = false;

        if (playerInteracaoAtual != null)
            playerInteracaoAtual.enabled = false;

        SetVisibilidadePlayer(false);

        if (playerInteracaoAtual != null && playerInteracaoAtual.cameraTransform != null)
            cameraPlayerAtual = playerInteracaoAtual.cameraTransform.GetComponent<Camera>();

        if (cameraPlayerAtual != null)
            cameraPlayerAtual.gameObject.SetActive(false);

        if (cameraCarro != null)
            cameraCarro.gameObject.SetActive(true);

        if (carroController != null)
            carroController.enabled = true;

        Debug.Log("Player entrou no carro.");
    }

    public void SairCarro()
{
    if (!playerDentro) return;
    playerDentro = false;

    // Reposiciona o player ao lado do carro, na posição atual dele
    if (playerAtual != null)
    {
        Vector3 posicaoSaida = transform.position + transform.right * 2.5f + Vector3.up * 0.2f;
        playerAtual.transform.position = posicaoSaida;
    }

    SetVisibilidadePlayer(true);

    if (playerMovementAtual != null)
        playerMovementAtual.enabled = true;

    if (playerInteracaoAtual != null)
        playerInteracaoAtual.enabled = true;

    if (cameraCarro != null)
        cameraCarro.gameObject.SetActive(false);

    if (cameraPlayerAtual != null)
        cameraPlayerAtual.gameObject.SetActive(true);

    if (carroController != null)
        carroController.enabled = false;

    Debug.Log("Player saiu do carro.");

    playerAtual = null;
    playerMovementAtual = null;
    playerInteracaoAtual = null;
    cameraPlayerAtual = null;
}

    void SetVisibilidadePlayer(bool visivel)
    {
        if (playerAtual == null) return;

        Renderer[] rends = playerAtual.GetComponentsInChildren<Renderer>();
        foreach (var rend in rends)
        {
            rend.enabled = visivel;
        }
    }

    void Update()
    {
        if (playerDentro && Input.GetKeyDown(KeyCode.E))
        {
            SairCarro();
        }
    }
}