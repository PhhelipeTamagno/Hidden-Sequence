using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class NpcFinal : MonoBehaviour
{
    // =========================================================
    // UI - PERGUNTAS
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
    // RESPOSTAS DE NEGAÇÃO
    // =========================================================

    [Header("Respostas de Negação")]
    [TextArea]
    public string[] respostasNegacao =
    {
        "Não sei de nada. Eu não estava envolvido.",
        "Já disse, não tenho nada a ver com isso.",
        "Você está perdendo tempo comigo.",
        "Não vou responder isso.",
        "Eu não fiz nada de errado.",
        "Não sei do que você está falando.",
        "Você não tem nenhuma prova contra mim.",
        "Eu estava em outro lugar naquele dia.",
        "Você está tentando me incriminar.",
        "Não tenho nada a ver com esse crime."
    };

    private int ultimaResposta = -1;

    // =========================================================
    // PAINEL DAS PROVAS
    // =========================================================

    [Header("Painel das Provas")]
    public GameObject painelProvas;

    // =========================================================
    // BOTÕES DAS PISTAS
    // =========================================================

    [Header("Botões das Pistas")]
    public Button botaoGarrafa;
    public Button botaoFaca;
    public Button botaoDigital;

    // =========================================================
    // IMAGENS DAS PISTAS
    // =========================================================

    [Header("Imagens das Pistas")]
    public Image imagemGarrafa;
    public Image imagemFaca;
    public Image imagemDigital;

    // =========================================================
    // SPRITES
    // =========================================================

    [Header("Sprites das Pistas")]
    public Sprite spriteGarrafa;
    public Sprite spriteFaca;
    public Sprite spriteDigital;

    [Header("Cor da Pista Depois de Usada")]
    public Color corPistaUsada = Color.gray;

    private bool garrafaApresentada = false;
    private bool facaApresentada = false;
    private bool digitalApresentada = false;

    // =========================================================
    // RESPOSTAS DAS PISTAS
    // =========================================================

    [Header("Resposta da Garrafa")]
    [TextArea]
    public string respostaGarrafa =
        "Essa garrafa não prova nada! Eu nunca vi isso antes.";

    [Header("Resposta da Faca")]
    [TextArea]
    public string respostaFaca =
        "Essa faca... eu não sei de onde você tirou isso.";

    [Header("Resposta das Digitais")]
    [TextArea]
    public string respostaDigital =
        "Essas digitais... não... isso não pode ser possível.";

    // =========================================================
    // CONFISSÃO - PARTE 1
    // =========================================================

    [Header("Confissão - Parte 1")]
    public TMP_Text textoConfissao1;

    [TextArea]
    public string mensagemConfissao1 =
        "Está bem... Eu confesso! Fui eu quem cometeu o crime. Eu o matei com uma estaca de gelo.";

    // =========================================================
    // CONFISSÃO - PARTE 2
    // =========================================================

    [Header("Confissão - Parte 2")]
    public TMP_Text textoConfissao2;

    [TextArea]
    public string mensagemConfissao2 =
        "Eu esculpi a estaca com uma faca e carregava na minha garrafa térmica.";

    // =========================================================
    // TEMPO DA CONFISSÃO
    // =========================================================

    [Header("Tempo da Confissão")]
    public float esperaAntesConfissao = 1f;

    public float tempoConfissao1 = 4f;

    public float tempoConfissao2 = 4f;

    // =========================================================
    // TEMPO DAS RESPOSTAS
    // =========================================================

    [Header("Tempo para Responder")]
    public float tempoPonto = 0.3f;

    public float tempoAntesResposta = 0.8f;

    // =========================================================
    // FADE
    // =========================================================

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;

    public float duracaoFade = 1.5f;

    // =========================================================
    // CENA
    // =========================================================

    [Header("Cena da Cutscene")]
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

    private bool respondendo = false;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        // Bloqueia perguntas inicialmente

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
        {
            botaoEnviar.interactable = false;

            botaoEnviar.onClick.RemoveAllListeners();

            botaoEnviar.onClick.AddListener(EnviarPergunta);
        }

        // Para o player inicialmente

        if (playerMovement != null)
            playerMovement.enabled = false;

        // =====================================================
        // BOTÃO GARRAFA
        // =====================================================

        if (botaoGarrafa != null)
        {
            botaoGarrafa.onClick.RemoveAllListeners();

            botaoGarrafa.onClick.AddListener(ApresentarGarrafa);
        }

        // =====================================================
        // BOTÃO FACA
        // =====================================================

        if (botaoFaca != null)
        {
            botaoFaca.onClick.RemoveAllListeners();

            botaoFaca.onClick.AddListener(ApresentarFaca);
        }

        // =====================================================
        // BOTÃO DIGITAL
        // =====================================================

        if (botaoDigital != null)
        {
            botaoDigital.onClick.RemoveAllListeners();

            botaoDigital.onClick.AddListener(ApresentarDigital);
        }

        // =====================================================
        // SPRITES
        // =====================================================

        if (imagemGarrafa != null)
        {
            imagemGarrafa.sprite = spriteGarrafa;

            imagemGarrafa.color = Color.white;
        }

        if (imagemFaca != null)
        {
            imagemFaca.sprite = spriteFaca;

            imagemFaca.color = Color.white;
        }

        if (imagemDigital != null)
        {
            imagemDigital.sprite = spriteDigital;

            imagemDigital.color = Color.white;
        }

        // =====================================================
        // CONFISSÃO
        // =====================================================

        if (textoConfissao1 != null)
        {
            textoConfissao1.gameObject.SetActive(false);
        }

        if (textoConfissao2 != null)
        {
            textoConfissao2.gameObject.SetActive(false);
        }

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
            {
                textoExplicacao.text = mensagemExplicacao;
            }

            StartCoroutine(FecharPopup());
        }
        else
        {
            LiberarInterrogatorio();
        }
    }

    // =========================================================
    // FECHAR POPUP
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
    // ENVIAR PERGUNTA
    // =========================================================

    public void EnviarPergunta()
    {
        if (interrogatorioFinalizado)
            return;

        if (respondendo)
            return;

        if (campoTexto == null)
            return;

        string pergunta = campoTexto.text;

        if (string.IsNullOrWhiteSpace(pergunta))
            return;

        campoTexto.text = "";

        StartCoroutine(ResponderPergunta());
    }

    // =========================================================
    // RESPONDER PERGUNTA
    // =========================================================

    IEnumerator ResponderPergunta()
    {
        respondendo = true;

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;

        // PONTO 1

        if (textoDigitando != null)
            textoDigitando.text = ".";

        yield return new WaitForSeconds(tempoPonto);

        // PONTO 2

        if (textoDigitando != null)
            textoDigitando.text = "..";

        yield return new WaitForSeconds(tempoPonto);

        // PONTO 3

        if (textoDigitando != null)
            textoDigitando.text = "...";

        yield return new WaitForSeconds(tempoAntesResposta);

        // Remove os pontos

        if (textoDigitando != null)
            textoDigitando.text = "";

        // Resposta

        if (textoResposta != null)
            textoResposta.text = ObterRespostaAleatoria();

        respondendo = false;

        // Libera novamente

        if (!interrogatorioFinalizado)
        {
            if (campoTexto != null)
            {
                campoTexto.interactable = true;

                campoTexto.ActivateInputField();
            }

            if (botaoEnviar != null)
                botaoEnviar.interactable = true;
        }
    }

    // =========================================================
    // RESPOSTA ALEATÓRIA
    // =========================================================

    string ObterRespostaAleatoria()
    {
        if (respostasNegacao == null ||
            respostasNegacao.Length == 0)
        {
            return "Não tenho nada para dizer.";
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
            interrogatorioFinalizado ||
            respondendo)
            return;

        garrafaApresentada = true;

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        if (imagemGarrafa != null)
            imagemGarrafa.color = corPistaUsada;

        StartCoroutine(
            ResponderPista(respostaGarrafa)
        );
    }

    // =========================================================
    // APRESENTAR FACA
    // =========================================================

    public void ApresentarFaca()
    {
        if (facaApresentada ||
            interrogatorioFinalizado ||
            respondendo)
            return;

        facaApresentada = true;

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        if (imagemFaca != null)
            imagemFaca.color = corPistaUsada;

        StartCoroutine(
            ResponderPista(respostaFaca)
        );
    }

    // =========================================================
    // APRESENTAR DIGITAL
    // =========================================================

    public void ApresentarDigital()
    {
        if (digitalApresentada ||
            interrogatorioFinalizado ||
            respondendo)
            return;

        digitalApresentada = true;

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        if (imagemDigital != null)
            imagemDigital.color = corPistaUsada;

        StartCoroutine(
            ResponderPista(respostaDigital)
        );
    }

    // =========================================================
    // RESPOSTA DA PISTA
    // =========================================================

    IEnumerator ResponderPista(string resposta)
    {
        respondendo = true;

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;

        // PONTO 1

        if (textoDigitando != null)
            textoDigitando.text = ".";

        yield return new WaitForSeconds(tempoPonto);

        // PONTO 2

        if (textoDigitando != null)
            textoDigitando.text = "..";

        yield return new WaitForSeconds(tempoPonto);

        // PONTO 3

        if (textoDigitando != null)
            textoDigitando.text = "...";

        yield return new WaitForSeconds(tempoAntesResposta);

        // Remove os pontos

        if (textoDigitando != null)
            textoDigitando.text = "";

        // Resposta da pista

        if (textoResposta != null)
            textoResposta.text = resposta;

        respondendo = false;

        // Libera perguntas novamente

        if (!interrogatorioFinalizado)
        {
            if (campoTexto != null)
            {
                campoTexto.interactable = true;

                campoTexto.ActivateInputField();
            }

            if (botaoEnviar != null)
                botaoEnviar.interactable = true;
        }

        // Verifica as pistas

        VerificarPistas();
    }

    // =========================================================
    // VERIFICAR AS 3 PISTAS
    // =========================================================

    void VerificarPistas()
    {
        if (interrogatorioFinalizado)
            return;

        if (!garrafaApresentada)
            return;

        if (!facaApresentada)
            return;

        if (!digitalApresentada)
            return;

        interrogatorioFinalizado = true;

        StartCoroutine(IniciarConfissao());
    }

    // =========================================================
    // CONFISSÃO
    // =========================================================

    IEnumerator IniciarConfissao()
    {
        // Espera depois da última pista

        yield return new WaitForSeconds(
            esperaAntesConfissao
        );

        // =====================================================
        // ESCONDE INTERROGATÓRIO
        // =====================================================

        if (textoResposta != null)
            textoResposta.gameObject.SetActive(false);

        if (textoDigitando != null)
            textoDigitando.gameObject.SetActive(false);

        if (campoTexto != null)
            campoTexto.gameObject.SetActive(false);

        if (botaoEnviar != null)
            botaoEnviar.gameObject.SetActive(false);

        // Para o player

        if (playerMovement != null)
            playerMovement.enabled = false;

        // =====================================================
        // CONFISSÃO - PARTE 1
        // =====================================================

        if (textoConfissao1 != null)
        {
            textoConfissao1.text = mensagemConfissao1;

            textoConfissao1.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(
            tempoConfissao1
        );

        // Esconde parte 1

        if (textoConfissao1 != null)
            textoConfissao1.gameObject.SetActive(false);

        // =====================================================
        // CONFISSÃO - PARTE 2
        // =====================================================

        if (textoConfissao2 != null)
        {
            textoConfissao2.text = mensagemConfissao2;

            textoConfissao2.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(
            tempoConfissao2
        );

        // Esconde parte 2

        if (textoConfissao2 != null)
            textoConfissao2.gameObject.SetActive(false);

        // =====================================================
        // FADE
        // =====================================================

        yield return StartCoroutine(
            FadeParaPreto()
        );

        // =====================================================
        // DESATIVA PAINEL DAS PROVAS
        // =====================================================

        if (painelProvas != null)
            painelProvas.SetActive(false);

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
    // FADE PARA PRETO
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