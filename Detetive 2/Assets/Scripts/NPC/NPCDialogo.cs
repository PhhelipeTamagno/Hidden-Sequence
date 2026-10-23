
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

    [Header("Animação do NPC")]
    public Animator animatorNPC;

    [Tooltip("Nome do estado da animação dentro do Animator.")]
    public string nomeAnimacao = "Interacao";

    [Tooltip("Tempo da animação em segundos. Use 0 para detectar o fim do clipe automaticamente.")]
    public float duracaoAnimacao = 0f;

    private int indiceFala;
    private bool conversando;
    private bool animacaoExecutando;

    private Camera cameraPrincipal;
    private GameObject indicadorMouse;

    private float tempoAnimacao;
    private float velocidadeOriginal;

    private void Start()
    {
        cameraPrincipal = Camera.main;

        if (textoDialogo != null)
            textoDialogo.gameObject.SetActive(false);

        if (animatorNPC == null)
            animatorNPC = GetComponentInChildren<Animator>();

        if (animatorNPC != null)
            velocidadeOriginal = animatorNPC.speed;

        CriarIndicadorMouse();
    }

    private void Update()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        // Controla o fim da animação.
        if (animacaoExecutando)
        {
            tempoAnimacao += Time.deltaTime;

            if (duracaoAnimacao > 0f &&
                tempoAnimacao >= duracaoAnimacao)
            {
                PararAnimacao();
            }
            else if (duracaoAnimacao <= 0f &&
                     animatorNPC != null)
            {
                AnimatorStateInfo estado =
                    animatorNPC.GetCurrentAnimatorStateInfo(0);

                if (!animatorNPC.IsInTransition(0) &&
                    estado.IsName(nomeAnimacao) &&
                    estado.normalizedTime >= 1f)
                {
                    PararAnimacao();
                }
            }
        }

        if (Mouse.current == null || cameraPrincipal == null)
            return;

        AtualizarPosicaoTexto();

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Clique durante o diálogo: avança a fala.
        if (conversando)
        {
            ProximaFala();
            return;
        }

        // Detecta o NPC clicado.
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
            {
                // Executa a animação ao clicar no NPC.
                npc.ExecutarAnimacao();

                // Inicia o diálogo.
                npc.IniciarDialogo();
            }
        }
    }

    private void ExecutarAnimacao()
    {
        if (animatorNPC == null)
        {
            Debug.LogWarning(
                "Animator não configurado no NPC: " + gameObject.name
            );
            return;
        }

        // Reinicia a animação desde o começo.
        animatorNPC.speed = velocidadeOriginal;
        animatorNPC.Play(nomeAnimacao, 0, 0f);
        animatorNPC.Update(0f);

        animacaoExecutando = true;
        tempoAnimacao = 0f;
    }

    private void PararAnimacao()
    {
        if (animatorNPC == null)
            return;

        animacaoExecutando = false;

        // Congela a animação no último quadro.
        animatorNPC.speed = 0f;
    }

    private void IniciarDialogo()
    {
        if (textoDialogo == null ||
            falas == null ||
            falas.Length == 0)
            return;

        conversando = true;
        indiceFala = 0;

        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text = falas[indiceFala];

        if (indicadorMouse != null)
            indicadorMouse.SetActive(false);

        AtualizarPosicaoTexto();
    }

    private void ProximaFala()
    {
        indiceFala++;

        if (indiceFala >= falas.Length)
        {
            EncerrarDialogo();
            return;
        }

        textoDialogo.text = falas[indiceFala];
    }

    private void EncerrarDialogo()
    {
        conversando = false;

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
            animatorNPC.speed = velocidadeOriginal;
    }
}
