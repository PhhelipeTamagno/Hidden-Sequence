
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCDialogo : MonoBehaviour
{
    [Header("Texto do NPC")]
    public TMP_Text textoDialogo;

    [TextArea(2, 5)]
    public string falaInicial = "Olá, Detetive! Você terá apenas 2 minutos para achar todas as pistas.";

    [TextArea(2, 5)]
    public string falaProblema = "Mas tem um problema, cortaram a luz do bairro todo.";

    [TextArea(2, 5)]
    public string falaTerceira = "Você pode colocar sua terceira fala aqui.";

    [Header("Tempo entre as frases")]
    public float intervaloFalas = 2f;

    [Header("Tempo para sumir a terceira fala")]
    public float tempoParaSumir = 4f;

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

    private Camera cameraPrincipal;
    private GameObject indicadorMouse;

    // 0 = aguardando primeiro clique
    // 1 = exibindo as duas primeiras frases
    // 2 = aguardando clique para terceira frase
    // 3 = terceira frase exibida
    // 4 = diálogo encerrado

    private int etapaDialogo = 0;

    private Coroutine sequenciaCoroutine;

    private void Start()
    {
        cameraPrincipal = Camera.main;

        if (textoDialogo != null)
            textoDialogo.gameObject.SetActive(false);

        if (animatorNPC == null)
            animatorNPC = GetComponentInChildren<Animator>();

        CriarIndicadorMouse();

        TrocarAnimacao(animacaoComputador);
    }

    private void Update()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        if (cameraPrincipal == null || Mouse.current == null)
            return;

        AtualizarPosicaoTexto();

        if (!Mouse.current.leftButton.wasPressedThisFrame)
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

            if (distancia > npc.distanciaMaxima)
                return;

            npc.Interagir();
        }
    }

    private void Interagir()
    {
        // Primeiro clique: inicia as duas primeiras frases
        if (etapaDialogo == 0)
        {
            etapaDialogo = 1;

            if (indicadorMouse != null)
                indicadorMouse.SetActive(false);

            if (sequenciaCoroutine != null)
                StopCoroutine(sequenciaCoroutine);

            sequenciaCoroutine = StartCoroutine(SequenciaInicial());
        }

        // Só permite clicar novamente depois que as duas frases sumirem
        else if (etapaDialogo == 2)
        {
            MostrarTerceiraFala();
        }
    }

    private IEnumerator SequenciaInicial()
    {
        // PRIMEIRA FRASE
        MostrarTexto(falaInicial);
        TrocarAnimacao(animacaoFalando);

        yield return new WaitForSeconds(intervaloFalas);

        // SEGUNDA FRASE
        MostrarTexto(falaProblema);
        TrocarAnimacao(animacaoFalando);

        yield return new WaitForSeconds(intervaloFalas);

        // ESCONDE O TEXTO AUTOMATICAMENTE
        if (textoDialogo != null)
        {
            textoDialogo.text = "";
            textoDialogo.gameObject.SetActive(false);
        }

        TrocarAnimacao(animacaoComputador);

        // Agora o jogador pode clicar para ver a terceira frase
        etapaDialogo = 2;

        sequenciaCoroutine = null;
    }

    private void MostrarTerceiraFala()
    {
        etapaDialogo = 3;

        MostrarTexto(falaTerceira);
        TrocarAnimacao(animacaoFalando);

        StartCoroutine(EsconderTerceiraFala());
    }

    private IEnumerator EsconderTerceiraFala()
    {
        yield return new WaitForSeconds(tempoParaSumir);

        if (textoDialogo != null)
        {
            textoDialogo.text = "";
            textoDialogo.gameObject.SetActive(false);
        }

        TrocarAnimacao(animacaoComputador);

        etapaDialogo = 4;
    }

    private void MostrarTexto(string mensagem)
    {
        if (textoDialogo == null)
            return;

        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text = mensagem;

        AtualizarPosicaoTexto();
    }

    private void TrocarAnimacao(string nomeAnimacao)
    {
        if (animatorNPC == null || string.IsNullOrEmpty(nomeAnimacao))
            return;

        int hash = Animator.StringToHash(nomeAnimacao);

        if (animatorNPC.HasState(0, hash))
        {
            animatorNPC.speed = 1f;
            animatorNPC.CrossFade(hash, 0.15f);
        }
    }

    private void AtualizarPosicaoTexto()
    {
        if (textoDialogo == null || !textoDialogo.gameObject.activeSelf)
            return;

        textoDialogo.transform.position =
            transform.position + deslocamentoTexto;
    }

    private void OnMouseEnter()
    {
        if (etapaDialogo != 0 || indicadorMouse == null)
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
        if (sequenciaCoroutine != null)
            StopCoroutine(sequenciaCoroutine);

        if (animatorNPC != null)
            animatorNPC.speed = 1f;
    }
}
