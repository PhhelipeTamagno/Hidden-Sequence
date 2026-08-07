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

    [Header("UI")]
    [Tooltip("GameObject da mira (Image) que fica no Canvas")]
    public GameObject mira;

    [Header("Câmera do Carro - Controle com Mouse")]
    public float sensibilidadeMouse = 3f;
    public float distanciaCamera = 6f;
    public float alturaCamera = 2f;
    public float anguloMinimoVertical = -20f;
    public float anguloMaximoVertical = 60f;

    private float rotacaoHorizontal = 0f;
    private float rotacaoVertical = 20f;

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

        if (mira != null)
            mira.SetActive(false);

        // Inicializa a rotação da câmera olhando na direção atual do carro,
        // pra ela não "pular" pra um ângulo aleatório ao entrar
        rotacaoHorizontal = transform.eulerAngles.y;
        rotacaoVertical = 20f;

        // Trava e esconde o cursor pra liberar o mouse pro controle da câmera
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

        if (mira != null)
            mira.SetActive(true);

        // Libera o cursor de novo pro modo a pé / menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

    void LateUpdate()
    {
        // Atualiza a câmera depois de tudo se mover, evita tremedeira (jitter)
        if (playerDentro && cameraCarro != null)
        {
            AtualizarCameraCarro();
        }
    }

    void AtualizarCameraCarro()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        rotacaoHorizontal += mouseX;
        rotacaoVertical -= mouseY;
        rotacaoVertical = Mathf.Clamp(rotacaoVertical, anguloMinimoVertical, anguloMaximoVertical);

        Quaternion rotacao = Quaternion.Euler(rotacaoVertical, rotacaoHorizontal, 0f);
        Vector3 direcaoParaTras = rotacao * Vector3.back;

        Vector3 pontoFoco = transform.position + Vector3.up * alturaCamera;
        Vector3 posicaoDesejada = pontoFoco + direcaoParaTras * distanciaCamera;

        cameraCarro.transform.position = posicaoDesejada;
        cameraCarro.transform.LookAt(pontoFoco);
    }
}