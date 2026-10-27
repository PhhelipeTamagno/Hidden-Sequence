
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCDialogo : MonoBehaviour
{
    [Header("Texto do NPC")]
    public TMP_Text textoDialogo;

    [TextArea(2, 5)]
    public string[] falas;

    [Header("Distância de interação")]
    public float distanciaMaxima = 3f;

    [Header("Posição do texto")]
    public Vector3 deslocamentoTexto = new Vector3(0f, 2f, 0f);

    [Header("Indicador do clique")]
    public Sprite imagemMouseEsquerdo;

    [Header("Animações do NPC")]
    public Animator animatorNPC;
    public string animacaoComputador = "MexendoPC";
    public string animacaoFalando = "Falando";

    [Header("Tempo da animação de fala")]
    [Tooltip("0 = detectar automaticamente o fim da animação.")]
    public float duracaoFalaAnimacao = 0f;

    [Header("Tempo do diálogo")]
    public float tempoParaSumir = 4f;

    private int indiceFala;
    private bool conversando;
    private bool animacaoFalandoAtiva;

    private Camera cameraPrincipal;
    private GameObject indicadorMouse;

    private float tempoAnimacao;
    private float temporizadorDialogo;

    private void Start()
    {
        cameraPrincipal = Camera.main;

        if (textoDialogo != null)
            textoDialogo.gameObject.SetActive(false);

        if (animatorNPC == null)
            animatorNPC = GetComponentInChildren<Animator>();

        CriarIndicadorMouse();

       
        if (animatorNPC != null)
            animatorNPC.Play(animacaoComputador, 0, 0f);
    }

    private void Update()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

       
        if (conversando)
        {
            temporizadorDialogo += Time.deltaTime;

            if (temporizadorDialogo >= tempoParaSumir)
                EncerrarDialogo();
        }

        AtualizarAnimacaoFalando();

        if (Mouse.current == null || cameraPrincipal == null)
            return;

        AtualizarPosicaoTexto();

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

       
        if (conversando)
            return;

        Ray raio = cameraPrincipal.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(raio, out RaycastHit hit))
        {
            NPCDialogo npc =
                hit.collider.GetComponentInParent<NPCDialogo>();

            if (npc == null)
                return;

            float distancia = Vector3.Distance(
                cameraPrincipal.transform.position,
                npc.transform.position
            );

            if (distancia <= npc.distanciaMaxima)
                npc.IniciarDialogo();
        }
    }

    private void IniciarDialogo()
    {
        if (textoDialogo == null ||
            falas == null ||
            falas.Length == 0)
            return;

        conversando = true;
        indiceFala = 0;
        temporizadorDialogo = 0f;

        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text = falas[indiceFala];

        if (indicadorMouse != null)
            indicadorMouse.SetActive(false);

        
        if (animatorNPC != null)
        {
            animatorNPC.speed = 1f;
            animatorNPC.Play(animacaoFalando, 0, 0f);

            animacaoFalandoAtiva = true;
            tempoAnimacao = 0f;
        }

        AtualizarPosicaoTexto();
    }

    private void AtualizarAnimacaoFalando()
    {
        if (!animacaoFalandoAtiva || animatorNPC == null)
            return;

        tempoAnimacao += Time.deltaTime;

       
        if (duracaoFalaAnimacao > 0f)
        {
            if (tempoAnimacao >= duracaoFalaAnimacao)
                VoltarAoComputador();

            return;
        }

        
        AnimatorStateInfo estado =
            animatorNPC.GetCurrentAnimatorStateInfo(0);

        if (!animatorNPC.IsInTransition(0) &&
            estado.IsName(animacaoFalando) &&
            estado.normalizedTime >= 1f)
        {
            VoltarAoComputador();
        }
    }

    private void VoltarAoComputador()
    {
        animacaoFalandoAtiva = false;

        if (animatorNPC != null)
        {
            animatorNPC.speed = 1f;
            animatorNPC.Play(animacaoComputador, 0, 0f);
        }
    }

    private void EncerrarDialogo()
    {
        conversando = false;
        temporizadorDialogo = 0f;

        if (textoDialogo != null)
        {
            textoDialogo.text = "";
            textoDialogo.gameObject.SetActive(false);
        }
    }

    private void AtualizarPosicaoTexto()
    {
        if (textoDialogo == null || !conversando)
            return;

        textoDialogo.transform.position =
            transform.position + deslocamentoTexto;
    }

    private void OnMouseEnter()
    {
        if (conversando || indicadorMouse == null)
            return;

        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        if (cameraPrincipal == null)
            return;

        float distancia = Vector3.Distance(
            cameraPrincipal.transform.position,
            transform.position
        );

        if (distancia <= distanciaMaxima)
            indicadorMouse.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (indicadorMouse != null)
            indicadorMouse.SetActive(false);
    }

    private void CriarIndicadorMouse()
    {
        if (imagemMouseEsquerdo == null)
            return;

        GameObject canvasObj =
            new GameObject("CanvasIndicadorMouse");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        indicadorMouse = new GameObject("IndicadorClique");
        indicadorMouse.transform.SetParent(
            canvasObj.transform, false
        );

        Image imagem = indicadorMouse.AddComponent<Image>();
        imagem.sprite = imagemMouseEsquerdo;
        imagem.raycastTarget = false;

        RectTransform rect =
            indicadorMouse.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(45f, 45f);

        indicadorMouse.SetActive(false);
    }

    private void OnDisable()
    {
        if (animatorNPC != null)
            animatorNPC.speed = 1f;
    }
}
