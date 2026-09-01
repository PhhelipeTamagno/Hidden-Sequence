using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NpcFinal : MonoBehaviour
{
    // =========================================================
    // UI DO INTERROGATÓRIO
    // =========================================================

    [Header("UI - Perguntas")]
    public TMP_InputField campoTexto;
    public TMP_Text textoResposta;
    public TMP_Text textoDigitando;
    public Button botaoEnviar;

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    public PlayerMovement playerMovement;

    // =========================================================
    // RESPOSTAS DO NPC
    // =========================================================

    [Header("Respostas do NPC")]
    public string[] respostasNegacao =
    {
        "Não sei de nada. Eu não estava envolvido.",
        "Já disse, não tenho nada a ver com isso.",
        "Você está perdendo tempo comigo.",
        "Não vou responder isso.",
        "Eu não fiz nada de errado.",
        "Não sei do que você está falando.",
        "Você não tem nenhuma prova contra mim."
    };

    private int ultimaResposta = -1;

    // =========================================================
    // PISTAS
    // =========================================================

    [Header("PISTAS")]

    public Button botaoGarrafa;
    public Button botaoFaca;
    public Button botaoDigital;

    [Header("Imagens das Pistas")]

    public Image imagemGarrafa;
    public Image imagemFaca;
    public Image imagemDigital;

    [Header("Sprites das Pistas")]

    public Sprite spriteGarrafa;
    public Sprite spriteFaca;
    public Sprite spriteDigital;

    [Header("Cores")]

    public Color corNormal = Color.white;
    public Color corApresentada = Color.gray;

    // Controle das pistas

    private bool garrafaApresentada = false;
    private bool facaApresentada = false;
    private bool digitalApresentada = false;

    // =========================================================
    // MENSAGENS DAS PISTAS
    // =========================================================

    [Header("Reações às Pistas")]

    [TextArea]
    public string respostaGarrafa =
        "Essa garrafa não prova nada! Eu nunca vi isso antes.";

    [TextArea]
    public string respostaFaca =
        "Essa faca... eu não sei de onde você tirou isso.";

    [TextArea]
    public string respostaDigital =
        "Essas digitais... não... isso não pode ser possível.";

    // =========================================================
    // CONFISSÃO
    // =========================================================

    [Header("Confissão")]

    public TMP_Text textoConfissao;

    [TextArea]
    public string mensagemConfissao =
        "Está bem... Eu confesso! Fui eu quem cometeu o crime.";

    public float velocidadeEscrita = 0.03f;

    public float esperaAntesConfissao = 1f;
    public float esperaDepoisConfissao = 3f;

    // =========================================================
    // FADE
    // =========================================================

    [Header("Fade")]

    public CanvasGroup fadeCanvasGroup;

    public float duracaoFade = 1.5f;

    // =========================================================
    // CENA FINAL
    // =========================================================

    [Header("Próxima Cena")]

    public string nomeProximaCena = "CutscenePrisao";

    // =========================================================
    // POPUP
    // =========================================================

    [Header("Popup Inicial")]

    public GameObject popupExplicativo;

    public TMP_Text textoExplicacao;

    [TextArea]
    public string mensagemExplicacao =
        "Faça quantas perguntas quiser. Apresente as três pistas para fazer o suspeito confessar.";

    public float tempoPopup = 3f;

    // =========================================================
    // CONTROLE
    // =========================================================

    private bool interrogatorioFinalizado = false;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        // Campo de perguntas

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
        {
            botaoEnviar.interactable = false;

            botaoEnviar.onClick.AddListener(EnviarPergunta);
        }

        // Player parado inicialmente

        if (playerMovement != null)
            playerMovement.enabled = false;

        // =====================================================
        // PISTAS
        // =====================================================

        if (botaoGarrafa != null)
            botaoGarrafa.onClick.AddListener(ApresentarGarrafa);

        if (botaoFaca != null)
            botaoFaca.onClick.AddListener(ApresentarFaca);

        if (botaoDigital != null)
            botaoDigital.onClick.AddListener(ApresentarDigital);

        // =====================================================
        // SPRITES
        // =====================================================

        if (imagemGarrafa != null)
        {
            imagemGarrafa.sprite = spriteGarrafa;
            imagemGarrafa.color = corNormal;
        }

        if (imagemFaca != null)
        {
            imagemFaca.sprite = spriteFaca;
            imagemFaca.color = corNormal;
        }

        if (imagemDigital != null)
        {
            imagemDigital.sprite = spriteDigital;
            imagemDigital.color = corNormal;
        }

        // =====================================================
        // CONFISSÃO
        // =====================================================

        if (textoConfissao != null)
            textoConfissao.gameObject.SetActive(false);

        // =====================================================
        // FADE
        // =====================================================

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        // =====================================================
        // POPUP
        // =====================================================

        if (popupExplicativo != null)
        {
            popupExplicativo.SetActive(true);

            if (textoExplicacao != null)
                textoExplicacao.text = mensagemExplicacao;

            StartCoroutine(FecharPopup());
        }
        else
        {
            LiberarInterrogatorio();
        }
    }

    // =========================================================
    // POPUP
    // =========================================================

    IEnumerator FecharPopup()
    {
        yield return new WaitForSeconds(tempoPopup);

        if (popupExplicativo != null)
            popupExplicativo.SetActive(false);

        LiberarInterrogatorio();
    }

    // =========================================================
    // LIBERAR INTERROGATÓRIO
    // =========================================================

    void LiberarInterrogatorio()
    {
        if (campoTexto != null)
        {
            campoTexto.interactable = true;
            campoTexto.ActivateInputField();
        }

        if (botaoEnviar != null)
            botaoEnviar.interactable = true;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    // =========================================================
    // PERGUNTAS ILIMITADAS
    // =========================================================

    public void EnviarPergunta()
    {
        if (interrogatorioFinalizado)
            return;

        if (campoTexto == null)
            return;

        string pergunta = campoTexto.text;

        if (string.IsNullOrWhiteSpace(pergunta))
            return;

        campoTexto.text = "";

        StartCoroutine(ResponderNPC());
    }

    // =========================================================
    // RESPOSTA
    // =========================================================

    IEnumerator ResponderNPC()
    {
        if (textoResposta != null)
            textoResposta.text = "";

        if (textoDigitando != null)
            textoDigitando.text = ".";

        yield return new WaitForSeconds(0.3f);

        if (textoDigitando != null)
            textoDigitando.text = "..";

        yield return new WaitForSeconds(0.3f);

        if (textoDigitando != null)
            textoDigitando.text = "...";

        yield return new WaitForSeconds(1f);

        if (textoDigitando != null)
            textoDigitando.text = "";

        if (textoResposta != null)
            textoResposta.text = ObterRespostaAleatoria();
    }

    // =========================================================
    // RESPOSTA ALEATÓRIA
    // =========================================================

    string ObterRespostaAleatoria()
    {
        if (respostasNegacao == null ||
            respostasNegacao.Length == 0)
        {
            return "";
        }

        int index;

        do
        {
            index = Random.Range(
                0,
                respostasNegacao.Length
            );

        } while (
            respostasNegacao.Length > 1 &&
            index == ultimaResposta
        );

        ultimaResposta = index;

        return respostasNegacao[index];
    }

    // =========================================================
    // APRESENTAR GARRAFA
    // =========================================================

    public void ApresentarGarrafa()
    {
        if (garrafaApresentada ||
            interrogatorioFinalizado)
            return;

        garrafaApresentada = true;

        if (textoResposta != null)
            textoResposta.text = respostaGarrafa;

        MarcarPista(imagemGarrafa);

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        VerificarPistas();
    }

    // =========================================================
    // APRESENTAR FACA
    // =========================================================

    public void ApresentarFaca()
    {
        if (facaApresentada ||
            interrogatorioFinalizado)
            return;

        facaApresentada = true;

        if (textoResposta != null)
            textoResposta.text = respostaFaca;

        MarcarPista(imagemFaca);

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        VerificarPistas();
    }

    // =========================================================
    // APRESENTAR DIGITAL
    // =========================================================

    public void ApresentarDigital()
    {
        if (digitalApresentada ||
            interrogatorioFinalizado)
            return;

        digitalApresentada = true;

        if (textoResposta != null)
            textoResposta.text = respostaDigital;

        MarcarPista(imagemDigital);

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        VerificarPistas();
    }

    // =========================================================
    // MARCAR PISTA
    // =========================================================

    void MarcarPista(Image imagem)
    {
        if (imagem != null)
            imagem.color = corApresentada;
    }

    // =========================================================
    // VERIFICAR AS 3 PISTAS
    // =========================================================

    void VerificarPistas()
    {
        if (!garrafaApresentada)
            return;

        if (!facaApresentada)
            return;

        if (!digitalApresentada)
            return;

        // As três pistas foram apresentadas

        interrogatorioFinalizado = true;

        StartCoroutine(Confissao());
    }

    // =========================================================
    // CONFISSÃO
    // =========================================================

    IEnumerator Confissao()
    {
        // Para perguntas

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;

        // Para as pistas

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        // Para o jogador

        if (playerMovement != null)
            playerMovement.enabled = false;

        // Espera

        yield return new WaitForSeconds(
            esperaAntesConfissao
        );

        // Mostra confissão

        if (textoConfissao != null)
        {
            textoConfissao.gameObject.SetActive(true);

            yield return StartCoroutine(
                EscreverTexto(
                    textoConfissao,
                    mensagemConfissao
                )
            );
        }

        // Deixa a confissão na tela

        yield return new WaitForSeconds(
            esperaDepoisConfissao
        );

        // Fade

        yield return StartCoroutine(
            FadeParaPreto()
        );

        // =====================================================
        // TROCA DE CENA
        // =====================================================

        if (!string.IsNullOrEmpty(nomeProximaCena))
        {
            SceneManager.LoadScene(
                nomeProximaCena
            );
        }
    }

    // =========================================================
    // ESCREVER TEXTO
    // =========================================================

    IEnumerator EscreverTexto(
        TMP_Text texto,
        string mensagem)
    {
        texto.text = "";

        foreach (char letra in mensagem)
        {
            texto.text += letra;

            yield return new WaitForSeconds(
                velocidadeEscrita
            );
        }
    }

    // =========================================================
    // FADE
    // =========================================================

    IEnumerator FadeParaPreto()
    {
        if (fadeCanvasGroup == null)
            yield break;

        fadeCanvasGroup.blocksRaycasts = true;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;

            fadeCanvasGroup.alpha =
                Mathf.Clamp01(
                    tempo / duracaoFade
                );

            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}