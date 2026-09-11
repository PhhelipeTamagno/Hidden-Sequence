using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class NpcFinal : MonoBehaviour
{
    [Header("UI - Interrogatorio")]
    public TMP_InputField campoTexto;
    public TMP_Text textoResposta;
    public TMP_Text textoDigitando;
    public Button botaoEnviar;

    [Header("Player")]
    public PlayerMovement playerMovement;

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

        if (botaoEnviar != null)
        {
            botaoEnviar.onClick.AddListener(EnviarPergunta);

            EventTrigger trigger =
                botaoEnviar.gameObject.GetComponent<EventTrigger>();

            if (trigger == null)
                trigger = botaoEnviar.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entradaPointerDown =
                new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerDown
                };

            entradaPointerDown.callback.AddListener(
                (data) => { EnviarPergunta(); }
            );

            trigger.triggers.Add(entradaPointerDown);
        }

        if (botaoGarrafa != null)
            botaoGarrafa.onClick.AddListener(ApresentarGarrafa);

        if (botaoFaca != null)
            botaoFaca.onClick.AddListener(ApresentarFaca);

        if (botaoDigital != null)
            botaoDigital.onClick.AddListener(ApresentarDigital);

        if (imagemGarrafa != null)
            imagemGarrafa.sprite = spriteGarrafa;

        if (imagemFaca != null)
            imagemFaca.sprite = spriteFaca;

        if (imagemDigital != null)
            imagemDigital.sprite = spriteDigital;

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

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (popupExplicativo != null)
            popupExplicativo.SetActive(false);

        StartCoroutine(MostrarPopup());
    }

    private void Update()
    {
        if (campoTexto != null &&
            campoTexto.isFocused &&
            (Input.GetKeyDown(KeyCode.Return) ||
             Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            EnviarPergunta();
        }
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
            respostasNegacao.Length > 0)
        {
            int indice =
                Random.Range(0, respostasNegacao.Length);

            textoResposta.text =
                respostasNegacao[indice];
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

    public void ApresentarGarrafa()
    {
        if (respondendo)
            return;

        if (garrafaApresentada)
            return;

        garrafaApresentada = true;

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        if (imagemGarrafa != null)
            imagemGarrafa.color = corPistaUsada;

        if (contadorGarrafaCoroutine != null)
        {
            StopCoroutine(contadorGarrafaCoroutine);
            contadorGarrafaCoroutine = null;
        }

        if (contadorGarrafa != null)
        {
            contadorGarrafa.text = "";
            contadorGarrafa.gameObject.SetActive(false);
        }

        IniciarContadoresDasOutrasProvas("garrafa");

        StartCoroutine(ResponderPista(respostaGarrafa));
    }

    public void ApresentarFaca()
    {
        if (respondendo)
            return;

        if (facaApresentada)
            return;

        facaApresentada = true;

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        if (imagemFaca != null)
            imagemFaca.color = corPistaUsada;

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

        IniciarContadoresDasOutrasProvas("faca");

        StartCoroutine(ResponderPista(respostaFaca));
    }

    public void ApresentarDigital()
    {
        if (respondendo)
            return;

        if (digitalApresentada)
            return;

        digitalApresentada = true;

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        if (imagemDigital != null)
            imagemDigital.color = corPistaUsada;

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

        IniciarContadoresDasOutrasProvas("digital");

        StartCoroutine(ResponderPista(respostaDigital));
    }

    void IniciarContadoresDasOutrasProvas(string provaUsada)
    {

        if (provaUsada != "garrafa" &&
            !garrafaApresentada)
        {
            if (contadorGarrafaCoroutine != null)
                StopCoroutine(contadorGarrafaCoroutine);

            contadorGarrafaCoroutine =
                StartCoroutine(ContadorGarrafa());
        }

        if (provaUsada != "faca" &&
            !facaApresentada)
        {
            if (contadorFacaCoroutine != null)
                StopCoroutine(contadorFacaCoroutine);

            contadorFacaCoroutine =
                StartCoroutine(ContadorFaca());
        }

        if (provaUsada != "digital" &&
            !digitalApresentada)
        {
            if (contadorDigitalCoroutine != null)
                StopCoroutine(contadorDigitalCoroutine);

            contadorDigitalCoroutine =
                StartCoroutine(ContadorDigital());
        }
    }

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

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = true;
    }

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
    }

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
    }

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

        if (textoResposta != null)
            textoResposta.text = resposta;

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
    void VerificarPistas()
    {
        if (garrafaApresentada &&
            facaApresentada &&
            digitalApresentada)
        {
            if (contadorGarrafaCoroutine != null)
                StopCoroutine(contadorGarrafaCoroutine);

            if (contadorFacaCoroutine != null)
                StopCoroutine(contadorFacaCoroutine);

            if (contadorDigitalCoroutine != null)
                StopCoroutine(contadorDigitalCoroutine);

            StartCoroutine(IniciarConfissao());
        }
    }

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

        yield return new WaitForSeconds(tempoConfissao1);

        if (textoConfissao1 != null)
            textoConfissao1.gameObject.SetActive(false);

        if (textoConfissao2 != null)
        {
            textoConfissao2.gameObject.SetActive(true);
            textoConfissao2.text = mensagemConfissao2;
        }

        yield return new WaitForSeconds(tempoConfissao2);

        if (textoConfissao2 != null)
            textoConfissao2.gameObject.SetActive(false);

        yield return StartCoroutine(FadeParaPreto());

        if (painelProvas != null)
            painelProvas.SetActive(false);

        SceneManager.LoadScene(nomeProximaCena);
    }

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
}