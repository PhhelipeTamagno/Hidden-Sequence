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
    public GameObject mira;


    [Header("Indicador de Destino")]
    public IndicadorDestino indicadorDestino;


    [Header("Popup Cena do Crime")]
    public CenaDoCrimeIntro cenaDoCrimeIntro;


    [Header("Câmera do Carro")]
    public float sensibilidadeMouse = 3f;
    public float distanciaCamera = 6f;
    public float alturaCamera = 2f;
    public float anguloMinimoVertical = -20f;
    public float anguloMaximoVertical = 60f;


    private float rotacaoHorizontal;
    private float rotacaoVertical = 20f;


    private MaterialPropertyBlock propertyBlock;
    private bool destacado = false;


    private GameObject playerAtual;
    private PlayerMovement playerMovementAtual;
    private PlayerInteracao playerInteracaoAtual;

    private Camera cameraPlayerAtual;

    private bool playerDentro = false;


    private CursorLockMode cursorLockAnterior;
    private bool cursorVisivelAnterior;


    // Impede sair do carro no mesmo frame em que entrou
    private int frameDeEntrada = -1;


    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();


        if (renderers == null ||
            renderers.Length == 0 ||
            ContemNulos(renderers))
        {
            renderers = GetComponentsInChildren<Renderer>();
        }


        if (carroController != null)
            carroController.enabled = false;


        if (cameraCarro != null)
            cameraCarro.gameObject.SetActive(false);


        // Indicador começa desligado
        if (indicadorDestino != null)
            indicadorDestino.mostrarIndicador = false;
    }


    bool ContemNulos(Renderer[] array)
    {
        foreach (var r in array)
        {
            if (r == null)
                return true;
        }

        return false;
    }


    // =========================================================
    // DESTAQUE
    // =========================================================

    public void Destacar()
    {
        if (destacado)
            return;


        destacado = true;


        foreach (var rend in renderers)
        {
            if (rend == null)
                continue;


            rend.GetPropertyBlock(propertyBlock);


            propertyBlock.SetColor(
                "_EmissionColor",
                corBrilho * intensidadeBrilho
            );


            rend.SetPropertyBlock(propertyBlock);
        }
    }


    public void RemoverDestaque()
    {
        if (!destacado)
            return;


        destacado = false;


        foreach (var rend in renderers)
        {
            if (rend == null)
                continue;


            rend.GetPropertyBlock(propertyBlock);


            propertyBlock.SetColor(
                "_EmissionColor",
                Color.black
            );


            rend.SetPropertyBlock(propertyBlock);
        }
    }


    // =========================================================
    // ENTRAR NO CARRO
    // =========================================================

    public void EntrarCarro(GameObject player)
    {
        if (playerDentro)
            return;


        playerDentro = true;


        // Impede que o mesmo E seja usado para sair
        frameDeEntrada = Time.frameCount;


        playerAtual = player;


        playerMovementAtual =
            player.GetComponent<PlayerMovement>();


        playerInteracaoAtual =
            player.GetComponent<PlayerInteracao>();


        RemoverDestaque();


        // Desliga movimento do player
        if (playerMovementAtual != null)
            playerMovementAtual.enabled = false;


        // Desliga interação do player
        if (playerInteracaoAtual != null)
            playerInteracaoAtual.enabled = false;


        // Esconde modelo do player
        SetVisibilidadePlayer(false);


        // Pega câmera do player
        if (playerInteracaoAtual != null &&
            playerInteracaoAtual.cameraTransform != null)
        {
            cameraPlayerAtual =
                playerInteracaoAtual.cameraTransform
                .GetComponent<Camera>();
        }


        if (cameraPlayerAtual != null)
            cameraPlayerAtual.gameObject.SetActive(false);


        // Liga câmera do carro
        if (cameraCarro != null)
            cameraCarro.gameObject.SetActive(true);


        // Liga controle do carro
        if (carroController != null)
            carroController.enabled = true;


        // =====================================================
        // ATIVA INDICADOR
        // =====================================================

        if (indicadorDestino != null)
        {
            // IMPORTANTE:
            // Agora o indicador acompanha o CARRO,
            // não o player.
            Transform referenciaCarro =
                carroController != null
                ? carroController.transform
                : transform;


            indicadorDestino.DefinirJogador(
                referenciaCarro
            );


            indicadorDestino.mostrarIndicador = true;
        }


        // Esconde mira
        if (mira != null)
            mira.SetActive(false);


        // Câmera começa olhando para frente
        rotacaoHorizontal =
            carroController != null
            ? carroController.transform.eulerAngles.y
            : transform.eulerAngles.y;


        rotacaoVertical = 20f;


        // Guarda estado do cursor
        cursorLockAnterior = Cursor.lockState;
        cursorVisivelAnterior = Cursor.visible;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        // Popup
        if (cenaDoCrimeIntro != null)
            cenaDoCrimeIntro.MostrarPopupCenaDoCrime();


        Debug.Log("Player entrou no carro.");
    }


    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (!playerDentro)
            return;


        // Impede saída no mesmo frame da entrada
        if (Time.frameCount == frameDeEntrada)
            return;


        if (Input.GetKeyDown(KeyCode.E))
        {
            SairCarro();
        }
    }


    // =========================================================
    // SAIR DO CARRO
    // =========================================================

    public void SairCarro()
    {
        if (!playerDentro)
            return;


        playerDentro = false;


        // =====================================================
        // PEGA POSIÇÃO ATUAL DO CARRO
        // =====================================================

        Transform carroTransform =
            carroController != null
            ? carroController.transform
            : transform;


        Vector3 posicaoCarro =
            carroTransform.position;


        // Sai pela direita do carro
        Vector3 posicaoSaida =
            posicaoCarro +
            carroTransform.right * 2.5f;


        // Mantém altura do player
        if (playerAtual != null)
        {
            posicaoSaida.y =
                playerAtual.transform.position.y;


            playerAtual.transform.position =
                posicaoSaida;
        }


        // =====================================================
        // DEVOLVE PLAYER
        // =====================================================

        SetVisibilidadePlayer(true);


        if (playerMovementAtual != null)
            playerMovementAtual.enabled = true;


        if (playerInteracaoAtual != null)
            playerInteracaoAtual.enabled = true;


        // =====================================================
        // CÂMERAS
        // =====================================================

        if (cameraCarro != null)
            cameraCarro.gameObject.SetActive(false);


        if (cameraPlayerAtual != null)
            cameraPlayerAtual.gameObject.SetActive(true);


        // =====================================================
        // DESLIGA CARRO
        // =====================================================

        if (carroController != null)
            carroController.enabled = false;


        // =====================================================
        // DESLIGA INDICADOR
        // =====================================================

        if (indicadorDestino != null)
        {
            indicadorDestino.mostrarIndicador = false;
        }


        // =====================================================
        // MIRA
        // =====================================================

        if (mira != null)
            mira.SetActive(true);


        // =====================================================
        // CURSOR
        // =====================================================

        Cursor.lockState = cursorLockAnterior;
        Cursor.visible = cursorVisivelAnterior;


        Debug.Log(
            "Player saiu do carro na posição: " +
            posicaoSaida
        );


        // =====================================================
        // LIMPA REFERÊNCIAS
        // =====================================================

        playerAtual = null;
        playerMovementAtual = null;
        playerInteracaoAtual = null;
        cameraPlayerAtual = null;


        frameDeEntrada = -1;
    }


    // =========================================================
    // VISIBILIDADE DO PLAYER
    // =========================================================

    void SetVisibilidadePlayer(bool visivel)
    {
        if (playerAtual == null)
            return;


        Renderer[] rends =
            playerAtual.GetComponentsInChildren<Renderer>();


        foreach (var rend in rends)
        {
            rend.enabled = visivel;
        }
    }


    // =========================================================
    // CÂMERA
    // =========================================================

    void LateUpdate()
    {
        if (!playerDentro)
            return;


        if (cameraCarro != null)
        {
            AtualizarCameraCarro();
        }
    }


    void AtualizarCameraCarro()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            sensibilidadeMouse;


        float mouseY =
            Input.GetAxis("Mouse Y") *
            sensibilidadeMouse;


        rotacaoHorizontal += mouseX;


        rotacaoVertical -= mouseY;


        rotacaoVertical =
            Mathf.Clamp(
                rotacaoVertical,
                anguloMinimoVertical,
                anguloMaximoVertical
            );


        Quaternion rotacao =
            Quaternion.Euler(
                rotacaoVertical,
                rotacaoHorizontal,
                0f
            );


        Vector3 direcaoParaTras =
            rotacao * Vector3.back;


        Transform carroTransform =
            carroController != null
            ? carroController.transform
            : transform;


        Vector3 pontoFoco =
            carroTransform.position +
            Vector3.up * alturaCamera;


        Vector3 posicaoDesejada =
            pontoFoco +
            direcaoParaTras *
            distanciaCamera;


        cameraCarro.transform.position =
            posicaoDesejada;


        cameraCarro.transform.LookAt(
            pontoFoco
        );
    }
}