
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Globalization;
using System.Text;

public class NPCResposta : MonoBehaviour
{
    [Header("Caderno")]
    public TMP_Text textoCaderno;

    [Header("Pensamento do jogador")]
    public TMP_Text textoPensamento;

    [Header("Animação do pensamento")]
    public CanvasGroup painelPensamento;
    public float velocidadeDigitacaoPensamento = 0.04f;
    public float tempoPensamentoVisivel = 4f;
    public float duracaoDesaparecerPensamento = 0.5f;

    private Coroutine rotinaPensamento;
    private string perguntaAtual;

    [Header("UI")]
    public TMP_InputField campoTexto;
    public TMP_Text textoNPC;
    public Button botaoEnviar;

    [Header("Player")]
    public PlayerMovement playerMovement;

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
    public string[] respostasNegacao =
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
    private bool respondendo = false;
    private bool interrogatorioFinalizado = false;

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
        DadosInterrogatorio.Limpar();

        if (textoCaderno != null)
            textoCaderno.text = "";

 
        if (painelPensamento != null)
        {
            painelPensamento.alpha = 1f;
            painelPensamento.gameObject.SetActive(true);
        }

        if (textoPensamento != null)
        {
            string pensamento =
                "Vou anotar o que ele falar. Vai que ele diz algo totalmente contrário depois.";

            rotinaPensamento = StartCoroutine(
                MostrarPensamento(pensamento)
            );
        }

        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
        {
            botaoEnviar.interactable = false;
            botaoEnviar.onClick.AddListener(EnviarPergunta);
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (popupExplicativo != null)
        {
            posicaoFinalPopupExplicativo =
                popupExplicativo.anchoredPosition;

            if (textoExplicacao != null)
                textoExplicacao.text = mensagemExplicacao;

            popupExplicativo.gameObject.SetActive(false);

            StartCoroutine(FluxoPopupExplicativo());
        }
        else
        {
            LiberarInterrogatorio();
        }
    }

    IEnumerator MostrarPensamento(string mensagem)
    {
        textoPensamento.text = "";

        foreach (char letra in mensagem)
        {
            textoPensamento.text += letra;

            yield return new WaitForSeconds(
                velocidadeDigitacaoPensamento
            );
        }

        yield return new WaitForSeconds(
            tempoPensamentoVisivel
        );

        if (painelPensamento != null)
        {
            float tempo = 0f;
            float alphaInicial = painelPensamento.alpha;

            while (tempo < duracaoDesaparecerPensamento)
            {
                tempo += Time.deltaTime;

                float progresso = duracaoDesaparecerPensamento > 0f
                    ? Mathf.Clamp01(tempo / duracaoDesaparecerPensamento)
                    : 1f;

                painelPensamento.alpha = Mathf.Lerp(
                    alphaInicial,
                    0f,
                    progresso
                );

                yield return null;
            }

            painelPensamento.alpha = 0f;
            painelPensamento.gameObject.SetActive(false);
        }
        else
        {
            textoPensamento.gameObject.SetActive(false);
        }
    }

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

    IEnumerator FluxoPopupExplicativo()
    {
        yield return StartCoroutine(AbrirPopupExplicativo());

        yield return new WaitForSeconds(tempoVisivelPopup);

        yield return StartCoroutine(FecharPopupExplicativoCorrotina());
    }

    IEnumerator AbrirPopupExplicativo()
    {
        popupExplicativo.gameObject.SetActive(true);

        Vector2 posicaoInicial =
            posicaoFinalPopupExplicativo +
            Vector2.up * distanciaQueda;

        popupExplicativo.anchoredPosition = posicaoInicial;

        if (popupExplicativoCanvasGroup != null)
            popupExplicativoCanvasGroup.alpha = 0f;

        float tempo = 0f;

        while (tempo < duracaoAnimacaoPopup)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(
                tempo / duracaoAnimacaoPopup
            );

            float suave = 1f - Mathf.Pow(1f - t, 3f);

            popupExplicativo.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicial,
                    posicaoFinalPopupExplicativo,
                    suave
                );

            if (popupExplicativoCanvasGroup != null)
                popupExplicativoCanvasGroup.alpha = suave;

            yield return null;
        }

        popupExplicativo.anchoredPosition =
            posicaoFinalPopupExplicativo;

        if (popupExplicativoCanvasGroup != null)
            popupExplicativoCanvasGroup.alpha = 1f;
    }

    IEnumerator FecharPopupExplicativoCorrotina()
    {
        Vector2 inicial = popupExplicativo.anchoredPosition;

        Vector2 final =
            posicaoFinalPopupExplicativo +
            Vector2.up * distanciaQueda;

        float tempo = 0f;

        while (tempo < duracaoAnimacaoPopup)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(
                tempo / duracaoAnimacaoPopup
            );

            popupExplicativo.anchoredPosition =
                Vector2.Lerp(inicial, final, t * t);

            if (popupExplicativoCanvasGroup != null)
                popupExplicativoCanvasGroup.alpha = 1f - t * t;

            yield return null;
        }

        popupExplicativo.gameObject.SetActive(false);

        LiberarInterrogatorio();
    }

    public void EnviarPergunta()
    {
        if (respondendo || interrogatorioFinalizado)
            return;

        if (campoTexto == null ||
            string.IsNullOrWhiteSpace(campoTexto.text))
            return;

        perguntaAtual = Normalizar(campoTexto.text);

        perguntasFeitas++;
        campoTexto.text = "";

        respondendo = true;
        campoTexto.interactable = false;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;

        StartCoroutine(ResponderNPC());
    }

    IEnumerator ResponderNPC()
    {
        if (textoDigitando != null)
            textoDigitando.text = "Suspeito está pensando...";

        yield return new WaitForSeconds(1.2f);

        if (textoDigitando != null)
            textoDigitando.text = "";

        string resposta = ObterResposta();

        if (textoResposta != null)
            textoResposta.text = resposta;

        DadosInterrogatorio.Adicionar(resposta);

        AtualizarCaderno();

        respondendo = false;

        if (perguntasFeitas >= 3)
        {
            interrogatorioFinalizado = true;

            yield return new WaitForSeconds(2f);

            StartCoroutine(FinalInterrogatorio());
        }
        else
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

    string Normalizar(string texto)
    {
        string original = texto.ToLowerInvariant().Normalize(
            NormalizationForm.FormD
        );

        StringBuilder resultado = new StringBuilder();

        foreach (char letra in original)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letra)
                != UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(letra);
            }
        }

        return resultado.ToString()
            .Normalize(NormalizationForm.FormC);
    }

    bool ContemAlgum(string texto, params string[] palavras)
    {
        foreach (string palavra in palavras)
        {
            if (texto.Contains(palavra))
                return true;
        }

        return false;
    }

    string ObterResposta()
    {
        // Companhia
        if (ContemAlgum(perguntaAtual,
            "sozinho", "companhia", "com quem",
            "alguem com voce", "estava acompanhado",
            "estava com alguem", "quem estava com",
            "tinha alguem"))
        {
            return "Eu estava sozinho em casa naquela noite. Não tinha ninguem comigo.";
        }

        // Atividades
        if (ContemAlgum(perguntaAtual,
            "o que fazia", "que fazia", "fazendo em casa",
            "fazendo naquele dia", "estava fazendo",
            "fazendo naquela noite", "estava ocupado",
            "fazendo no dia", "fazia perto"))
        {
            return "Eu estava em casa tentando descansar. Nao fiz nada de especial naquela noite.";
        }

        // Relação com a vítima
        if (ContemAlgum(perguntaAtual,
            "conhecia", "conhece", "conhecer",
            "quem era aquele", "quem era a vitima",
            "relacao com", "amigo dele", "amigo dela",
            "conhecia aquele cara", "conhecia aquela pessoa",
            "conhecia a vitima", "como conheceu"))
        {
            return "Eu ja tinha visto essa pessoa algumas vezes, mas não eramos proximos.";
        }

        // Localização
        if (ContemAlgum(perguntaAtual,
            "onde estava", "onde voce estava",
            "estava em casa", "qual lugar",
            "estava aonde", "seu paradeiro",
            "foi para onde", "estava na casa"))
        {
            return "Eu estava em casa naquela noite. Nao tenho motivo para inventar outro lugar.";
        }

        // Horário do crime
        if (ContemAlgum(perguntaAtual,
            "que horas", "horario", "22 horas",
            "dez da noite", "meia noite",
            "a que horas", "qual hora",
            "quando aconteceu", "horario do crime",
            "hora do crime", "na hora do crime",
            "perto do crime", "naquela hora",
            "naquele horario"))
        {
            return "Por volta das 22 horas, eu estava em casa.So isso que consigo lembrar.";
        }

        // Saída de casa
        if (ContemAlgum(perguntaAtual,
            "saiu de casa", "sair de casa",
            "saiu naquela noite", "saiu naquele dia",
            "foi para fora", "deixou a casa",
            "saiu durante a noite"))
        {
            return "Nao sai de casa naquela noite. Fiquei la praticamente o tempo todo.";
        }

        // Discussões
        if (ContemAlgum(perguntaAtual,
            "discutiu", "discussao", "brigou",
            "briga", "desentendimento", "ameacou",
            "ameaca", "tinha problemas com"))
        {
            return "Eu tive alguns desentendimentos, mas nada que justificasse um crime.";
        }

        // Barulhos
        if (ContemAlgum(perguntaAtual,
            "ouviu barulho", "escutou barulho",
            "grito", "barulho estranho",
            "ouviu alguma coisa", "escutou alguma coisa"))
        {
            return "Nao me lembro de ter ouvido nada estranho naquela noite.";
        }

        // Motivo
        if (ContemAlgum(perguntaAtual,
            "por que", "porque", "motivo", "razao",
            "raiva", "vinganca", "vingar"))
        {
            return "Eu não tinha motivo para machucar ninguem. Não ganharia nada com isso.";
        }

        // Garrafa
        if (ContemAlgum(perguntaAtual,
            "garrafa", "bebida", "frasco"))
        {
            return "Eu nao reconheço essa garrafa. Não sei por que ela estaria ligada a mim.";
        }

        // Faca
        if (ContemAlgum(perguntaAtual,
            "faca", "arma branca", "lamina", "objeto cortante"))
        {
            return "Nao conheço essa faca. Não tenho nada a esconder sobre isso.";
        }

        // Impressões digitais
        if (ContemAlgum(perguntaAtual,
            "digital", "digitais", "impressao",
            "toquei", "tocou", "encostou", "encostei"))
        {
            return "Eu nao me lembro de ter tocado em nada relacionado a esse caso.";
        }

        // Acusação de crime
        if (ContemAlgum(perguntaAtual,
            "matou", "assassinou", "cometeu o crime",
            "foi voce", "culpado", "participou",
            "envolvido", "fez isso", "machucou"))
        {
            return "Nao fui eu! Você esta tentando me culpar sem ter certeza.";
        }

        // Perguntas gerais
        if (ContemAlgum(perguntaAtual,
            "o que aconteceu", "me conte", "conte tudo",
            "o que sabe", "sabe sobre o caso",
            "explique o que"))
        {
            return "Eu nao sei exatamente o que aconteceu. So sei que estao me acusando de algo que nao fiz.";
        }

        // Perguntas sobre a vítima
        if (ContemAlgum(perguntaAtual,
            "vitima", "morto", "morta", "morreu",
            "corpo", "encontraram"))
        {
            return "Eu soube do caso, mas nao sei dizer o que aconteceu com essa pessoa.";
        }

        return "Nao sei responder isso. Ja falei tudo o que sei sobre aquela noite.";
    }

    void AtualizarCaderno()
    {
        if (textoCaderno == null)
            return;

        textoCaderno.text = string.Join(
            "\n\n",
            DadosInterrogatorio.anotacoes
        );
    }

    IEnumerator FinalInterrogatorio()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return StartCoroutine(FadeParaPreto());

        if (textoResposta != null)
            textoResposta.gameObject.SetActive(false);

        if (textoDigitando != null)
            textoDigitando.gameObject.SetActive(false);

        if (campoTexto != null)
            campoTexto.gameObject.SetActive(false);

        if (botaoEnviar != null)
            botaoEnviar.gameObject.SetActive(false);

        if (textoMensagem != null)
        {
            textoMensagem.gameObject.SetActive(true);

            yield return StartCoroutine(
                EscreverTexto(textoMensagem, mensagemFinal)
            );
        }

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
        if (fadeCanvasGroup == null)
            yield break;

        fadeCanvasGroup.blocksRaycasts = true;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;

            fadeCanvasGroup.alpha = Mathf.Clamp01(
                tempo / duracaoFade
            );

            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}
