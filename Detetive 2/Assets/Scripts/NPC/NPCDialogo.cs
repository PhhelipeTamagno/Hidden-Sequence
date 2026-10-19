
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCDialogo : MonoBehaviour
{
    [Header("NPC")]
    public string nomeNPC = "Morador";

    [TextArea(2, 5)]
    public string[] falas =
    {
        "Olá, viajante!",
        "Precisa de alguma coisa?",
        "Tenha cuidado por aí."
    };

    [Header("Posição")]
    public Vector3 offsetDialogo = new Vector3(0, 2.5f, 0);
    public Vector3 offsetIndicador = new Vector3(0, 2f, 0);

    [Header("Indicador do mouse")]
    public Sprite imagemMouseEsquerdo;
    public Vector2 tamanhoIndicador = new Vector2(48, 48);

    [Header("Diálogo")]
    public Vector2 tamanhoCaixa = new Vector2(300, 120);

    private int falaAtual;
    private bool conversando;
    private bool mouseSobreNPC;

    private Canvas canvas;
    private GameObject painel;
    private GameObject indicador;
    private TextMeshProUGUI textoNome;
    private TextMeshProUGUI textoFala;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        CriarInterface();
    }

    void Update()
    {
        if (cam == null)
            cam = Camera.main;

        if (conversando)
        {
            AtualizarPosicaoDialogo();

            if (Input.GetMouseButtonDown(0))
                AvancarDialogo();

            return;
        }

        VerificarMouseSobreNPC();

        if (Input.GetMouseButtonDown(0))
            VerificarClique();
    }

    void VerificarMouseSobreNPC()
    {
        if (cam == null)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool estavaSobreNPC = mouseSobreNPC;
        mouseSobreNPC = false;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            NPCDialogo npc = hit.collider.GetComponentInParent<NPCDialogo>();

            if (npc == this)
                mouseSobreNPC = true;
        }

        if (indicador != null)
            indicador.SetActive(mouseSobreNPC);

        if (mouseSobreNPC)
            AtualizarPosicaoIndicador();
    }

    void VerificarClique()
    {
        if (cam == null)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            NPCDialogo npc = hit.collider.GetComponentInParent<NPCDialogo>();

            if (npc != null)
                npc.IniciarDialogo();
        }
    }

    public void IniciarDialogo()
    {
        if (falas == null || falas.Length == 0)
            return;

        NPCDialogo[] npcs = FindObjectsByType<NPCDialogo>(
            FindObjectsSortMode.None
        );

        foreach (NPCDialogo npc in npcs)
        {
            if (npc != this && npc.conversando)
                npc.FecharDialogo();
        }

        falaAtual = 0;
        conversando = true;

        painel.SetActive(true);
        indicador.SetActive(false);

        AtualizarTexto();
        AtualizarPosicaoDialogo();
    }

    void AvancarDialogo()
    {
        falaAtual++;

        if (falaAtual >= falas.Length)
        {
            FecharDialogo();
            return;
        }

        AtualizarTexto();
    }

    void FecharDialogo()
    {
        conversando = false;
        painel.SetActive(false);

        // O indicador volta a aparecer quando o mouse
        // estiver sobre o NPC.
        indicador.SetActive(false);
    }

    void AtualizarTexto()
    {
        textoNome.text = nomeNPC;
        textoFala.text = falas[falaAtual];
    }

    void AtualizarPosicaoDialogo()
    {
        if (cam == null || painel == null)
            return;

        Vector3 pos = transform.position + offsetDialogo;
        Vector3 tela = cam.WorldToScreenPoint(pos);

        if (tela.z <= 0)
        {
            painel.SetActive(false);
            return;
        }

        painel.SetActive(true);
        painel.transform.position = tela;
    }

    void AtualizarPosicaoIndicador()
    {
        if (cam == null || indicador == null)
            return;

        Vector3 pos = transform.position + offsetIndicador;
        Vector3 tela = cam.WorldToScreenPoint(pos);

        if (tela.z <= 0)
        {
            indicador.SetActive(false);
            return;
        }

        indicador.transform.position = tela;
    }

    void CriarInterface()
    {
        GameObject canvasObj = new GameObject(
            "Canvas_Dialogo_" + nomeNPC
        );

        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        CriarIndicador(canvas.transform);
        CriarPainelDialogo(canvas.transform);
    }

    void CriarIndicador(Transform pai)
    {
        indicador = new GameObject("Indicador_Mouse_Esquerdo");
        indicador.transform.SetParent(pai, false);

        Image imagem = indicador.AddComponent<Image>();
        imagem.sprite = imagemMouseEsquerdo;
        imagem.preserveAspect = true;
        imagem.raycastTarget = false;

        RectTransform rect = indicador.GetComponent<RectTransform>();
        rect.sizeDelta = tamanhoIndicador;

        indicador.SetActive(false);
    }

    void CriarPainelDialogo(Transform pai)
    {
        painel = new GameObject("PainelDialogo");
        painel.transform.SetParent(pai, false);

        Image fundo = painel.AddComponent<Image>();
        fundo.color = new Color(0.08f, 0.08f, 0.08f, 0.92f);

        RectTransform rectPainel = painel.GetComponent<RectTransform>();
        rectPainel.sizeDelta = tamanhoCaixa;

        CriarTexto(
            "NomeNPC", nomeNPC, 22, FontStyles.Bold,
            new Vector2(0, 0.28f), new Vector2(0.9f, 0.9f),
            painel.transform, out textoNome
        );

        CriarTexto(
            "TextoFala", "", 18, FontStyles.Normal,
            new Vector2(0, -0.12f), new Vector2(0.9f, 0.55f),
            painel.transform, out textoFala
        );

        painel.SetActive(false);
    }

    void CriarTexto(
        string nome,
        string conteudo,
        float tamanho,
        FontStyles estilo,
        Vector2 ancoraMin,
        Vector2 ancoraMax,
        Transform pai,
        out TextMeshProUGUI texto)
    {
        GameObject obj = new GameObject(nome);
        obj.transform.SetParent(pai, false);

        texto = obj.AddComponent<TextMeshProUGUI>();
        texto.text = conteudo;
        texto.fontSize = tamanho;
        texto.fontStyle = estilo;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;
        texto.textWrappingMode = TextWrappingModes.Normal;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = ancoraMin;
        rect.anchorMax = ancoraMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
