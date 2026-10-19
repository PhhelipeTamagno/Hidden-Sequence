using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NpcFinal : MonoBehaviour
{
    [Header("Caderno e confronto")]
    public TMP_Dropdown dropdownAnotacoes;
    public Button botaoRebater;
    public TMP_Text textoFalaJogador;

    private bool confissaoIniciada = false;

    [Header("UI - Interrogatorio")]
    public TMP_InputField campoTexto;
    public TMP_Text textoResposta;
    public TMP_Text textoDigitando;
    public Button botaoEnviar;

    [Header("Player")]
    public PlayerMovement playerMovement;

    // =========================================================
    // NOVO - ANIMACAO DO NPC
    // =========================================================
    [Header("Animacao do NPC")]
    public Animator animatorNpc;

    [Tooltip("Trigger disparado quando o NPC responde uma pergunta normal (negacao)")]
    public string triggerNegacao = "Negar";

    [Tooltip("Trigger disparado quando o NPC responde a uma prova apresentada")]
    public string triggerProva = "Nervoso";

    [Tooltip("Trigger disparado na primeira mensagem de confissao")]
    public string triggerConfissao1 = "Confessar1";

    [Tooltip("Trigger disparado na segunda mensagem de confissao")]
    public string triggerConfissao2 = "Confessar2";

    [Header("Respostas de Negacao")]
    public string[] respostasNegacao =
    {
        "Nao sei de nada. Eu nao estava envolvido.",
        "Ja disse, nao tenho nada a ver com isso.",
        "Voce esta perdendo tempo comigo, eu nao vi nada.",
        "Nao vou responder isso. Nao fiz nada de errado.",
        "Isso nao tem nada a ver comigo, procure outra pessoa.",
        "Eu nao sei do que voce esta falando.",
        "Voce nao tem nenhuma prova contra mim.",
        "Eu estava em outro lugar naquele dia.",
        "Voce esta tentando me incriminar.",
        "Nao tenho nada a ver com esse crime."
    };

    [Header("Painel das Provas")]
    public GameObject painelProvas;

    [Header("Botoes das Provas")]
    public Button botaoGarrafa;
    public Button botaoFaca;
    public Button botaoDigital;

    [Header("Imagens das Provas")]
    public Image imagemGarrafa;
    public Image imagemFaca;
    public Image imagemDigital;

    [Header("Sprites das Provas")]
    public Sprite spriteGarrafa;
    public Sprite spriteFaca;
    public Sprite spriteDigital;

    [Header("Cor da Prova Usada")]
    public Color corPistaUsada = Color.gray;

    [Header("Tempo de carregamento das provas")]
    public float tempoProva = 10f;

    [Header("TMP Contador - Garrafa")]
    public TMP_Text contadorGarrafa;

    [Header("TMP Contador - Faca")]
    public TMP_Text contadorFaca;

    [Header("TMP Contador - Digital")]
    public TMP_Text contadorDigital;

    [Header("Respostas das Provas")]
    public string respostaGarrafa =
        "Essa garrafa nao prova nada! Eu nunca vi isso antes.";

    public string respostaFaca =
        "Essa faca... eu nao sei de onde voce tirou isso.";

    public string respostaDigital =
        "Essas digitais... nao... isso nao pode ser possivel.";

    [Header("Confissao")]
    public TMP_Text textoConfissao1;
    public TMP_Text textoConfissao2;

    [TextArea(2, 5)]
    public string mensagemConfissao1 =
        "Esta bem... Eu confesso! Fui eu quem cometeu o crime. Eu o matei com uma estaca de gelo.";

    [TextArea(2, 5)]
    public string mensagemConfissao2 =
        "Eu esculpi a estaca com uma faca e carregava na minha garrafa termica.";

    public float esperaAntesConfissao = 1f;
    public float tempoConfissao1 = 4f;
    public float tempoConfissao2 = 4f;

    [Header("Tempo das Respostas")]
    public float tempoPonto = 0.3f;
    public float tempoAntesResposta = 0.8f;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1.5f;

    [Header("Proxima Cena")]
    public string nomeProximaCena = "CutscenePrisao";

    [Header("Popup Explicativo")]
    public GameObject popupExplicativo;
    public TMP_Text textoExplicacao;
    public float tempoPopup = 3f;

    private bool respondendo = false;

    private bool garrafaApresentada = false;
    private bool facaApresentada = false;
    private bool digitalApresentada = false;

    private Coroutine contadorGarrafaCoroutine;
    private Coroutine contadorFacaCoroutine;
    private Coroutine contadorDigitalCoroutine;

    private void Start()
    {
        if (dropdownAnotacoes != null)
        {
            dropdownAnotacoes.ClearOptions();
            dropdownAnotacoes.AddOptions(DadosInterrogatorio.anotacoes);
        }

        if (botaoRebater != null)
        {
            botaoRebater.onClick.AddListener(RebaterComAnotacao);
        }

        // Botao enviar
        if (botaoEnviar != null)
        {
            botaoEnviar.onClick.AddListener(EnviarPergunta);
        }

        // Enter no campo de texto
        if (campoTexto != null)
        {
            campoTexto.onSubmit.AddListener(EnviarPerguntaPorEnter);
        }

        // Botoes das provas
        if (botaoGarrafa != null)
        {
            botaoGarrafa.onClick.RemoveAllListeners();
            botaoGarrafa.onClick.AddListener(ApresentarGarrafa);
        }

        if (botaoFaca != null)
        {
            botaoFaca.onClick.RemoveAllListeners();
            botaoFaca.onClick.AddListener(ApresentarFaca);
        }

        if (botaoDigital != null)
        {
            botaoDigital.onClick.RemoveAllListeners();
            botaoDigital.onClick.AddListener(ApresentarDigital);
        }

        // Sprites
        if (imagemGarrafa != null)
            imagemGarrafa.sprite = spriteGarrafa;

        if (imagemFaca != null)
            imagemFaca.sprite = spriteFaca;

        if (imagemDigital != null)
            imagemDigital.sprite = spriteDigital;

        // Confissao
        if (textoConfissao1 != null)
        {
            textoConfissao1.gameObject.SetActive(false);
            textoConfissao1.text = mensagemConfissao1;
        }

        if (textoConfissao2 != null)
        {
            textoConfissao2.gameObject.SetActive(false);
            textoConfissao2.text = mensagemConfissao2;
        }

        // Contadores
        if (contadorGarrafa != null)
        {
            contadorGarrafa.text = "";
            contadorGarrafa.gameObject.SetActive(false);
        }

        if (contadorFaca != null)
        {
            contadorFaca.text = "";
            contadorFaca.gameObject.SetActive(false);
        }

        if (contadorDigital != null)
        {
            contadorDigital.text = "";
            contadorDigital.gameObject.SetActive(false);
        }

        // Fade
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        // Popup
        if (popupExplicativo != null)
            popupExplicativo.SetActive(false);

        // NOVO - tenta pegar o Animator automaticamente se nao foi arrastado
        if (animatorNpc == null)
            animatorNpc = GetComponent<Animator>();

        StartCoroutine(MostrarPopup());
    }

    private void EnviarPerguntaPorEnter(string texto)
    {
        EnviarPergunta();
    }

    // =========================================================
    // NOVO - TOCAR ANIMACAO DO NPC
    // =========================================================

    void TocarAnimacao(string nomeTrigger)
    {
        if (animatorNpc == null)
            return;

        if (string.IsNullOrEmpty(nomeTrigger))
            return;

        // Reseta antes para garantir que a animacao reinicie
        // mesmo se o trigger anterior nao foi consumido
        animatorNpc.ResetTrigger(nomeTrigger);
        animatorNpc.SetTrigger(nomeTrigger);
    }

    IEnumerator MostrarPopup()
    {
        if (popupExplicativo != null)
        {
            popupExplicativo.SetActive(true);

            if (textoExplicacao != null)
            {
                textoExplicacao.text =
                    "Faca perguntas ao suspeito. Quando estiver pronto, apresente as provas.";
            }
        }

        yield return new WaitForSeconds(tempoPopup);

        if (popupExplicativo != null)
            popupExplicativo.SetActive(false);
    }

    // =========================================================
    // PERGUNTAS
    // =========================================================

    public void EnviarPergunta()
    {
        if (respondendo)
            return;

        if (campoTexto == null)
            return;

        if (string.IsNullOrWhiteSpace(campoTexto.text))
            return;

        StartCoroutine(ResponderPergunta());
    }

    IEnumerator ResponderPergunta()
    {
        respondendo = true;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (textoResposta != null)
            textoResposta.text = "";

        if (textoDigitando != null)
            textoDigitando.text = ".";

        yield return new WaitForSeconds(tempoPonto);

        if (textoDigitando != null)
            textoDigitando.text = "..";

        yield return new WaitForSeconds(tempoPonto);

        if (textoDigitando != null)
            textoDigitando.text = "...";

        yield return new WaitForSeconds(tempoAntesResposta);

        if (textoDigitando != null)
            textoDigitando.text = "";

        if (textoResposta != null &&
            respostasNegacao != null &&
            respostasNegacao.Length > 0)
        {
            int indice =
                UnityEngine.Random.Range(0, respostasNegacao.Length);

            textoResposta.text =
                respostasNegacao[indice];

            // NOVO - anima o NPC junto com a mensagem
            TocarAnimacao(triggerNegacao);
        }

        if (campoTexto != null)
        {
            campoTexto.text = "";
            campoTexto.interactable = true;
            campoTexto.ActivateInputField();
        }

        if (botaoEnviar != null)
            botaoEnviar.interactable = true;

        respondendo = false;
    }

    // =========================================================
    // GARRAFA
    // =========================================================

    public void ApresentarGarrafa()
    {
        if (respondendo)
            return;

        if (garrafaApresentada)
            return;

        // Marca imediatamente como apresentada
        garrafaApresentada = true;

        // Desativa o botao imediatamente
        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        // Para o contador
        if (contadorGarrafaCoroutine != null)
        {
            StopCoroutine(contadorGarrafaCoroutine);
            contadorGarrafaCoroutine = null;
        }

        // Esconde contador
        if (contadorGarrafa != null)
        {
            contadorGarrafa.text = "";
            contadorGarrafa.gameObject.SetActive(false);
        }

        // Muda cor da prova
        if (imagemGarrafa != null)
            imagemGarrafa.color = corPistaUsada;

        // Inicia contadores das outras provas
        IniciarContadoresDasOutrasProvas("garrafa");

        // Resposta
        StartCoroutine(ResponderPista(respostaGarrafa));
    }

    // =========================================================
    // FACA
    // =========================================================

    public void ApresentarFaca()
    {
        if (respondendo)
            return;

        if (facaApresentada)
            return;

        facaApresentada = true;

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        if (contadorFacaCoroutine != null)
        {
            StopCoroutine(contadorFacaCoroutine);
            contadorFacaCoroutine = null;
        }

        if (contadorFaca != null)
        {
            contadorFaca.text = "";
            contadorFaca.gameObject.SetActive(false);
        }

        if (imagemFaca != null)
            imagemFaca.color = corPistaUsada;

        IniciarContadoresDasOutrasProvas("faca");

        StartCoroutine(ResponderPista(respostaFaca));
    }

    // =========================================================
    // DIGITAL
    // =========================================================

    public void ApresentarDigital()
    {
        if (respondendo)
            return;

        if (digitalApresentada)
            return;

        digitalApresentada = true;

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        if (contadorDigitalCoroutine != null)
        {
            StopCoroutine(contadorDigitalCoroutine);
            contadorDigitalCoroutine = null;
        }

        if (contadorDigital != null)
        {
            contadorDigital.text = "";
            contadorDigital.gameObject.SetActive(false);
        }

        if (imagemDigital != null)
            imagemDigital.color = corPistaUsada;

        IniciarContadoresDasOutrasProvas("digital");

        StartCoroutine(ResponderPista(respostaDigital));
    }

    // =========================================================
    // CONTADORES DAS OUTRAS PROVAS
    // =========================================================

    void IniciarContadoresDasOutrasProvas(string provaUsada)
    {
        // Garrafa
        if (provaUsada != "garrafa" &&
            !garrafaApresentada)
        {
            if (contadorGarrafaCoroutine != null)
                StopCoroutine(contadorGarrafaCoroutine);

            contadorGarrafaCoroutine =
                StartCoroutine(ContadorGarrafa());
        }

        // Faca
        if (provaUsada != "faca" &&
            !facaApresentada)
        {
            if (contadorFacaCoroutine != null)
                StopCoroutine(contadorFacaCoroutine);

            contadorFacaCoroutine =
                StartCoroutine(ContadorFaca());
        }

        // Digital
        if (provaUsada != "digital" &&
            !digitalApresentada)
        {
            if (contadorDigitalCoroutine != null)
                StopCoroutine(contadorDigitalCoroutine);

            contadorDigitalCoroutine =
                StartCoroutine(ContadorDigital());
        }
    }

    // =========================================================
    // CONTADOR GARRAFA
    // =========================================================

    IEnumerator ContadorGarrafa()
    {
        if (contadorGarrafa != null)
        {
            contadorGarrafa.gameObject.SetActive(true);
            contadorGarrafa.text =
                Mathf.CeilToInt(tempoProva).ToString();
        }

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        float tempo = tempoProva;

        while (tempo > 0)
        {
            if (garrafaApresentada)
                yield break;

            if (contadorGarrafa != null)
            {
                contadorGarrafa.text =
                    Mathf.CeilToInt(tempo).ToString();
            }

            tempo -= Time.deltaTime;

            yield return null;
        }

        if (garrafaApresentada)
            yield break;

        if (contadorGarrafa != null)
        {
            contadorGarrafa.text = "0";
            contadorGarrafa.gameObject.SetActive(true);
        }

        // Libera o botao apenas uma vez
        if (botaoGarrafa != null)
            botaoGarrafa.interactable = true;

        contadorGarrafaCoroutine = null;
    }

    // =========================================================
    // CONTADOR FACA
    // =========================================================

    IEnumerator ContadorFaca()
    {
        if (contadorFaca != null)
        {
            contadorFaca.gameObject.SetActive(true);
            contadorFaca.text =
                Mathf.CeilToInt(tempoProva).ToString();
        }

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        float tempo = tempoProva;

        while (tempo > 0)
        {
            if (facaApresentada)
                yield break;

            if (contadorFaca != null)
            {
                contadorFaca.text =
                    Mathf.CeilToInt(tempo).ToString();
            }

            tempo -= Time.deltaTime;

            yield return null;
        }

        if (facaApresentada)
            yield break;

        if (contadorFaca != null)
        {
            contadorFaca.text = "0";
            contadorFaca.gameObject.SetActive(true);
        }

        if (botaoFaca != null)
            botaoFaca.interactable = true;

        contadorFacaCoroutine = null;
    }

    // =========================================================
    // CONTADOR DIGITAL
    // =========================================================

    IEnumerator ContadorDigital()
    {
        if (contadorDigital != null)
        {
            contadorDigital.gameObject.SetActive(true);
            contadorDigital.text =
                Mathf.CeilToInt(tempoProva).ToString();
        }

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        float tempo = tempoProva;

        while (tempo > 0)
        {
            if (digitalApresentada)
                yield break;

            if (contadorDigital != null)
            {
                contadorDigital.text =
                    Mathf.CeilToInt(tempo).ToString();
            }

            tempo -= Time.deltaTime;

            yield return null;
        }

        if (digitalApresentada)
            yield break;

        if (contadorDigital != null)
        {
            contadorDigital.text = "0";
            contadorDigital.gameObject.SetActive(true);
        }

        if (botaoDigital != null)
            botaoDigital.interactable = true;

        contadorDigitalCoroutine = null;
    }

    // =========================================================
    // RESPOSTA DA PROVA
    // =========================================================

    IEnumerator ResponderPista(string resposta)
    {
        respondendo = true;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (textoResposta != null)
            textoResposta.text = "";

        if (textoDigitando != null)
            textoDigitando.text = ".";

        yield return new WaitForSeconds(tempoPonto);

        if (textoDigitando != null)
            textoDigitando.text = "..";

        yield return new WaitForSeconds(tempoPonto);

        if (textoDigitando != null)
            textoDigitando.text = "...";

        yield return new WaitForSeconds(tempoAntesResposta);

        if (textoDigitando != null)
            textoDigitando.text = "";

        if (textoResposta != null)
            textoResposta.text = resposta;

        // NOVO - anima o NPC junto com a mensagem da prova
        TocarAnimacao(triggerProva);

        if (campoTexto != null)
        {
            campoTexto.text = "";
            campoTexto.interactable = true;
            campoTexto.ActivateInputField();
        }

        if (botaoEnviar != null)
            botaoEnviar.interactable = true;

        respondendo = false;

        VerificarPistas();
    }

    // =========================================================
    // VERIFICAR TODAS AS PROVAS
    // =========================================================


    void VerificarPistas()
    {
        if (garrafaApresentada &&
            facaApresentada &&
            digitalApresentada)
        {
            if (textoResposta != null)
            {
                textoResposta.text =
                    "Todas as provas foram apresentadas. Agora confronte o suspeito com as contradições do caderno.";
            }
        }
    }


    // =========================================================
    // CONFISSAO
    // =========================================================

    IEnumerator IniciarConfissao()
    {
        respondendo = true;

        if (contadorGarrafa != null)
            contadorGarrafa.gameObject.SetActive(false);

        if (contadorFaca != null)
            contadorFaca.gameObject.SetActive(false);

        if (contadorDigital != null)
            contadorDigital.gameObject.SetActive(false);

        if (campoTexto != null)
            campoTexto.gameObject.SetActive(false);

        if (botaoEnviar != null)
            botaoEnviar.gameObject.SetActive(false);

        if (textoResposta != null)
            textoResposta.gameObject.SetActive(false);

        if (textoDigitando != null)
            textoDigitando.gameObject.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return new WaitForSeconds(esperaAntesConfissao);

        if (textoConfissao1 != null)
        {
            textoConfissao1.gameObject.SetActive(true);
            textoConfissao1.text = mensagemConfissao1;
        }

        // NOVO - animacao da primeira mensagem de confissao
        TocarAnimacao(triggerConfissao1);

        yield return new WaitForSeconds(tempoConfissao1);

        if (textoConfissao1 != null)
            textoConfissao1.gameObject.SetActive(false);

        if (textoConfissao2 != null)
        {
            textoConfissao2.gameObject.SetActive(true);
            textoConfissao2.text = mensagemConfissao2;
        }

        // NOVO - animacao da segunda mensagem de confissao
        TocarAnimacao(triggerConfissao2);

        yield return new WaitForSeconds(tempoConfissao2);

        if (textoConfissao2 != null)
            textoConfissao2.gameObject.SetActive(false);

        yield return StartCoroutine(FadeParaPreto());

        if (painelProvas != null)
            painelProvas.SetActive(false);

        SceneManager.LoadScene(nomeProximaCena);
    }

    // =========================================================
    // FADE
    // =========================================================

    IEnumerator FadeParaPreto()
    {
        if (fadeCanvasGroup == null)
            yield break;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;

            fadeCanvasGroup.alpha =
                Mathf.Clamp01(tempo / duracaoFade);

            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    public void RebaterComAnotacao()
    {
        if (dropdownAnotacoes == null ||
            dropdownAnotacoes.options.Count == 0)
        {
            return;
        }

        string anotacao = dropdownAnotacoes.options[
            dropdownAnotacoes.value
        ].text;

        if (textoFalaJogador != null)
        {
            textoFalaJogador.text = "Você disse antes: " + anotacao;
        }

        if (anotacao.Contains("garrafa") && garrafaApresentada)
        {
            textoResposta.text =
                "Eu... talvez eu tenha visto essa garrafa antes.";
        }
        else if (anotacao.Contains("faca") && facaApresentada)
        {
            textoResposta.text =
                "Está bem, eu conhecia a faca. Mas isso não significa que fui eu!";
        }
        else if (anotacao.Contains("tocou") && digitalApresentada)
        {
            textoResposta.text =
                "Eu posso ter tocado nos objetos, mas não queria que ninguém se machucasse.";
        }
        else if (anotacao.Contains("22 horas") && garrafaApresentada)
        {
            textoResposta.text =
                "Eu não estava em casa o tempo todo. Só saí por alguns minutos!";
        }
        else
        {
            textoResposta.text =
                "Isso não prova nada! Você está tirando conclusões precipitadas.";
        }

        TocarAnimacao(triggerProva);


        VerificarPistas();

        if (garrafaApresentada &&
            facaApresentada &&
            digitalApresentada &&
            !confissaoIniciada)
        {
            confissaoIniciada = true;

            StartCoroutine(IniciarConfissao());
        }

    }

}