using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCResposta : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField campoTexto;
    public TMP_Text textoNPC;
    [Tooltip("Botão do Canvas que também envia a pergunta (opcional, além do Enter no campo de texto)")]
    public Button botaoEnviar;

    [Header("Player")]
    public PlayerMovement playerMovement; // referência ao script de movimento/câmera do player

    [Header("Popup Explicativo")]
    public RectTransform popupExplicativo;
    public CanvasGroup popupExplicativoCanvasGroup;
    public TMP_Text textoExplicacao;
    [TextArea]
    public string mensagemExplicacao =
        "Você terá apenas 3 perguntas para interrogar o suspeito. Escolha com cuidado!";
    public float duracaoAnimacaoPopup = 0.5f;
    public float distanciaQueda = 100f;
    public float tempoVisivelPopup = 3f;

    private Vector2 posicaoFinalPopupExplicativo;

    [Header("Respostas do NPC")]
    public string[] respostasNegacao = new string[]
    {
        "Não sei de nada. Eu não estava envolvido.",
        "Já disse, não tenho nada a ver com isso.",
        "Você está perdendo tempo comigo, eu não vi nada.",
        "Não vou responder isso. Não fiz nada de errado.",
        "Isso não tem nada a ver comigo, procure outra pessoa."
    };

    private int ultimaRespostaIndex = -1;

    [Header("Mensagem Final")]
    public TMP_Text textoMensagem;
    [TextArea]
    public string mensagemFinal =
        "Após interrogar o suspeito, você decide seguir para a cena do crime em busca de pistas.";
    public float velocidadeEscrita = 0.03f;

    private int perguntasFeitas = 0;
    public TMP_Text textoResposta;
    public TMP_Text textoDigitando;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1.5f;

    [Header("Cena")]
    public string nomeProximaCena = "CenaDoCrime";
    public float esperaAposTexto = 2f;

    void Start()
    {
        campoTexto.interactable = false;

        if (botaoEnviar != null)
        {
            botaoEnviar.interactable = false;
            botaoEnviar.onClick.AddListener(EnviarPergunta);
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        posicaoFinalPopupExplicativo = popupExplicativo.anchoredPosition;
        textoExplicacao.text = mensagemExplicacao;
        popupExplicativo.gameObject.SetActive(false);

        StartCoroutine(FluxoPopupExplicativo());
    }

    IEnumerator FluxoPopupExplicativo()
    {
        yield return StartCoroutine(AbrirPopupExplicativo());

        yield return new WaitForSeconds(tempoVisivelPopup);

        yield return StartCoroutine(FecharPopupExplicativoCorrotina());
    }

    IEnumerator AbrirPopupExplicativo()
    {
        popupExplicativo.gameObject.SetActive(true);

        Vector2 posicaoInicial = posicaoFinalPopupExplicativo + Vector2.up * distanciaQueda;
        popupExplicativo.anchoredPosition = posicaoInicial;

        if (popupExplicativoCanvasGroup != null)
            popupExplicativoCanvasGroup.alpha = 0f;

        float tempo = 0f;
        while (tempo < duracaoAnimacaoPopup)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoAnimacaoPopup);
            float tSuave = 1f - Mathf.Pow(1f - t, 3f); // ease out cubic

            popupExplicativo.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoFinalPopupExplicativo, tSuave);

            if (popupExplicativoCanvasGroup != null)
                popupExplicativoCanvasGroup.alpha = tSuave;

            yield return null;
        }

        popupExplicativo.anchoredPosition = posicaoFinalPopupExplicativo;

        if (popupExplicativoCanvasGroup != null)
            popupExplicativoCanvasGroup.alpha = 1f;
    }

    IEnumerator FecharPopupExplicativoCorrotina()
    {
        Vector2 posicaoInicial = popupExplicativo.anchoredPosition;
        Vector2 posicaoSaida = posicaoFinalPopupExplicativo + Vector2.up * distanciaQueda;

        float tempo = 0f;
        while (tempo < duracaoAnimacaoPopup)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoAnimacaoPopup);
            float tSuave = t * t; // ease in, sobe acelerando

            popupExplicativo.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoSaida, tSuave);

            if (popupExplicativoCanvasGroup != null)
                popupExplicativoCanvasGroup.alpha = 1f - tSuave;

            yield return null;
        }

        popupExplicativo.gameObject.SetActive(false);

        campoTexto.interactable = true;
        campoTexto.ActivateInputField();

        if (botaoEnviar != null)
            botaoEnviar.interactable = true;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    IEnumerator FinalInterrogatorio()
    {
        yield return StartCoroutine(FadeParaPreto());

        textoResposta.gameObject.SetActive(false);
        textoDigitando.gameObject.SetActive(false);
        campoTexto.gameObject.SetActive(false);

        if (botaoEnviar != null)
            botaoEnviar.gameObject.SetActive(false);

        textoMensagem.gameObject.SetActive(true);
        yield return StartCoroutine(EscreverTexto(textoMensagem, mensagemFinal));

        yield return new WaitForSeconds(esperaAposTexto);

        SceneManager.LoadScene(nomeProximaCena);
    }

    IEnumerator EscreverTexto(TMP_Text texto, string mensagem)
    {
        texto.text = "";
        foreach (char letra in mensagem)
        {
            texto.text += letra;
            yield return new WaitForSeconds(velocidadeEscrita);
        }
    }

    IEnumerator FadeParaPreto()
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(tempo / duracaoFade);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    public void EnviarPergunta()
    {
        string pergunta = campoTexto.text;
        if (string.IsNullOrWhiteSpace(pergunta))
            return;

        perguntasFeitas++;
        campoTexto.text = "";
        StartCoroutine(ResponderNPC());

        if (perguntasFeitas >= 3)
        {
            StartCoroutine(FinalInterrogatorio());
        }
    }

    IEnumerator ResponderNPC()
    {
        textoResposta.text = "";
        textoDigitando.text = ".";
        yield return new WaitForSeconds(0.3f);
        textoDigitando.text = "..";
        yield return new WaitForSeconds(0.3f);
        textoDigitando.text = "...";
        yield return new WaitForSeconds(1f);
        textoDigitando.text = "";

        textoResposta.text = ObterRespostaAleatoria();
    }

    string ObterRespostaAleatoria()
    {
        if (respostasNegacao.Length == 0)
            return "";

        int index;
        do
        {
            index = Random.Range(0, respostasNegacao.Length);
        }
        while (respostasNegacao.Length > 1 && index == ultimaRespostaIndex);

        ultimaRespostaIndex = index;
        return respostasNegacao[index];
    }
}