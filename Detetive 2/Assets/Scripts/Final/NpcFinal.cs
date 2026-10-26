
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NpcFinal : MonoBehaviour
{
    [Header("Caderno e confronto")]
    public GameObject painelCaderno;
    public Button botaoOlharCaderno;
    public TMP_Text textoCaderno;
    public Button[] botoesAnotacoes;

    [Header("Fala do jogador")]
    public GameObject painelFalaJogador;
    public TMP_Text textoFalaJogador;
    public float tempoFalaJogador = 3f;
    public float velocidadeDigitacao = 0.025f;

    private bool cadernoAberto;
    private bool confissaoIniciada;
    private bool respondendo;

    private List<string> anotacoesDisponiveis = new List<string>();
    private Coroutine rotinaFalaJogador;

    [Header("UI - Interrogatorio")]
    public TMP_InputField campoTexto;
    public TMP_Text textoResposta;
    public TMP_Text textoDigitando;
    public Button botaoEnviar;

    [Header("Painel do suspeito")]
    public GameObject painelRespostaSuspeito;

    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Animacao do NPC")]
    public Animator animatorNpc;
    public string triggerNegacao = "Negar";
    public string triggerProva = "Nervoso";
    public string triggerConfissao1 = "Confessar1";
    public string triggerConfissao2 = "Confessar2";

    [Header("Respostas de negacao")]
    public string[] respostasNegacao =
    {
        "Nao sei de nada. Eu nao estava envolvido.",
        "Ja disse, nao tenho nada a ver com isso.",
        "Voce esta perdendo tempo comigo.",
        "Eu estava em outro lugar naquele dia.",
        "Voce esta tentando me incriminar."
    };

    [Header("Painel das provas")]
    public GameObject painelProvas;

    [Header("Botoes das provas")]
    public Button botaoGarrafa;
    public Button botaoFaca;
    public Button botaoDigital;

    [Header("Imagens das provas")]
    public Image imagemGarrafa;
    public Image imagemFaca;
    public Image imagemDigital;

    [Header("Sprites das provas")]
    public Sprite spriteGarrafa;
    public Sprite spriteFaca;
    public Sprite spriteDigital;

    [Header("Cor da prova usada")]
    public Color corPistaUsada = Color.gray;

    [Header("Tempo das provas")]
    public float tempoProva = 10f;

    [Header("Contadores")]
    public TMP_Text contadorGarrafa;
    public TMP_Text contadorFaca;
    public TMP_Text contadorDigital;

    [Header("Respostas das provas")]
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
        "Esta bem... Eu confesso! Fui eu quem cometeu o crime. " +
        "Eu matei a vitima com uma estaca de gelo, que eu escupi uma hora antes " +
        "Ela descobriu um segredo meu e ameacou acabar com a minha vida. " +
        "Eu fiquei com medo de perder tudo e decidi silencia-la.";

    [TextArea(2, 5)]
    public string mensagemConfissao2 =
        "Eu esculpi a estaca usando uma faca e a escondi na minha garrafa termica. " +
        "Achei que conseguiria apagar os rastros, mas deixei minhas digitais. " +
        "Quando voce apresentou as tres provas, percebi que nao tinha mais como negar.";

    public float esperaAntesConfissao = 1f;
    public float tempoConfissao1 = 7f;
    public float tempoConfissao2 = 7f;

    [Header("Tempo das respostas")]
    public float tempoPonto = 0.3f;
    public float tempoAntesResposta = 0.8f;
    public float tempoRespostaVisivel = 3f;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1.5f;

    [Header("Proxima cena")]
    public string nomeProximaCena = "CutscenePrisao";

    [Header("Popup explicativo")]
    public GameObject popupExplicativo;
    public TMP_Text textoExplicacao;
    public float tempoPopup = 3f;

    private bool garrafaApresentada;
    private bool facaApresentada;
    private bool digitalApresentada;

    private Coroutine contadorGarrafaCoroutine;
    private Coroutine contadorFacaCoroutine;
    private Coroutine contadorDigitalCoroutine;

    void Start()
    {
        if (animatorNpc == null)
            animatorNpc = GetComponent<Animator>();

        CarregarAnotacoes();

        if (painelCaderno != null)
            painelCaderno.SetActive(false);

        if (painelRespostaSuspeito != null)
            painelRespostaSuspeito.SetActive(false);

        if (painelFalaJogador != null)
            painelFalaJogador.SetActive(false);

        cadernoAberto = false;

        if (textoResposta != null)
            textoResposta.text = "";

        if (textoDigitando != null)
            textoDigitando.text = "";

        if (botaoOlharCaderno != null)
        {
            botaoOlharCaderno.onClick.RemoveListener(AlternarCaderno);
            botaoOlharCaderno.onClick.AddListener(AlternarCaderno);
        }

        if (botoesAnotacoes != null)
        {
            for (int i = 0; i < botoesAnotacoes.Length; i++)
            {
                int indice = i;

                if (botoesAnotacoes[i] == null)
                    continue;

                botoesAnotacoes[i].onClick.RemoveAllListeners();
                botoesAnotacoes[i].onClick.AddListener(
                    () => SelecionarAnotacao(indice)
                );
            }
        }

        if (botaoEnviar != null)
        {
            botaoEnviar.onClick.RemoveListener(EnviarPergunta);
            botaoEnviar.onClick.AddListener(EnviarPergunta);
        }

        if (campoTexto != null)
        {
            campoTexto.onSubmit.RemoveListener(EnviarPerguntaPorEnter);
            campoTexto.onSubmit.AddListener(EnviarPerguntaPorEnter);
            campoTexto.interactable = true;
        }

        ConfigurarBotao(botaoGarrafa, ApresentarGarrafa);
        ConfigurarBotao(botaoFaca, ApresentarFaca);
        ConfigurarBotao(botaoDigital, ApresentarDigital);

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

        EsconderContador(contadorGarrafa);
        EsconderContador(contadorFaca);
        EsconderContador(contadorDigital);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        if (popupExplicativo != null)
            popupExplicativo.SetActive(false);

        if (botaoEnviar != null)
            botaoEnviar.interactable = true;

        StartCoroutine(MostrarPopup());
    }

    void ConfigurarBotao(Button botao, UnityEngine.Events.UnityAction acao)
    {
        if (botao == null)
            return;

        botao.onClick.RemoveListener(acao);
        botao.onClick.AddListener(acao);
    }

    void CarregarAnotacoes()
    {
        anotacoesDisponiveis.Clear();

        if (DadosInterrogatorio.anotacoes != null)
        {
            foreach (string item in DadosInterrogatorio.anotacoes)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                string[] partes = Regex.Split(item.Trim(), @"(?:\r?\n\s*){2,}");

                foreach (string parte in partes)
                {
                    string anotacao = parte.Trim();

                    if (!string.IsNullOrWhiteSpace(anotacao))
                        anotacoesDisponiveis.Add(anotacao);
                }
            }
        }

        AtualizarCaderno();
    }

    void AtualizarCaderno()
    {
        if (textoCaderno != null)
        {
            textoCaderno.text = anotacoesDisponiveis.Count > 0
                ? string.Join("\n\n", anotacoesDisponiveis)
                : "Nenhuma anotacao encontrada no caderno.";
        }

        if (botoesAnotacoes == null)
            return;

        for (int i = 0; i < botoesAnotacoes.Length; i++)
        {
            Button botao = botoesAnotacoes[i];

            if (botao == null)
                continue;

            bool existe = i < anotacoesDisponiveis.Count;

            botao.gameObject.SetActive(existe);
            botao.interactable = existe;

            if (existe)
            {
                TMP_Text rotulo = botao.GetComponentInChildren<TMP_Text>(true);

                if (rotulo != null)
                    rotulo.text = anotacoesDisponiveis[i];
            }
        }
    }

    public void AlternarCaderno()
    {
        if (respondendo || confissaoIniciada || painelCaderno == null)
            return;

        cadernoAberto = !cadernoAberto;
        painelCaderno.SetActive(cadernoAberto);

        if (cadernoAberto)
            AtualizarCaderno();
    }

    void SelecionarAnotacao(int indice)
    {
        if (respondendo || confissaoIniciada)
            return;

        if (indice < 0 || indice >= anotacoesDisponiveis.Count)
            return;

        string anotacao = anotacoesDisponiveis[indice];

        if (string.IsNullOrWhiteSpace(anotacao))
            return;

        anotacoesDisponiveis.RemoveAt(indice);

        if (DadosInterrogatorio.anotacoes != null)
        {
            DadosInterrogatorio.anotacoes.Clear();

            foreach (string frase in anotacoesDisponiveis)
                DadosInterrogatorio.anotacoes.Add(frase);
        }

        AtualizarCaderno();

        if (painelCaderno != null)
            painelCaderno.SetActive(false);

        cadernoAberto = false;

        StartCoroutine(ConfrontarComAnotacao(anotacao));
    }

    IEnumerator ConfrontarComAnotacao(string anotacao)
    {
        respondendo = true;
        BloquearInterrogatorio();

        if (rotinaFalaJogador != null)
            StopCoroutine(rotinaFalaJogador);

        rotinaFalaJogador = StartCoroutine(
            MostrarFalaJogador("Voce disse antes: " + anotacao)
        );

        yield return StartCoroutine(MostrarDigitando());

        string resposta = ObterRespostaConfronto(anotacao);

        yield return StartCoroutine(
            MostrarRespostaSuspeito(resposta, triggerProva)
        );

        yield return new WaitForSeconds(tempoRespostaVisivel);

        EsconderRespostaSuspeito();

        respondendo = false;

        if (TodasAsProvasApresentadas() && !confissaoIniciada)
        {
            StartCoroutine(IniciarConfissao());
            yield break;
        }

        LiberarInterrogatorio();
    }

    IEnumerator MostrarFalaJogador(string mensagem)
    {
        if (textoFalaJogador == null)
            yield break;

        if (painelFalaJogador != null)
            painelFalaJogador.SetActive(true);

        textoFalaJogador.gameObject.SetActive(true);
        textoFalaJogador.text = "";

        foreach (char letra in mensagem)
        {
            textoFalaJogador.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
        }

        yield return new WaitForSeconds(tempoFalaJogador);

        textoFalaJogador.text = "";

        if (painelFalaJogador != null)
            painelFalaJogador.SetActive(false);

        rotinaFalaJogador = null;
    }

    void EnviarPerguntaPorEnter(string texto)
    {
        EnviarPergunta();
    }

    public void EnviarPergunta()
    {
        if (respondendo || confissaoIniciada || campoTexto == null)
            return;

        if (string.IsNullOrWhiteSpace(campoTexto.text))
            return;

        string pergunta = campoTexto.text.Trim();
        campoTexto.text = "";

        StartCoroutine(ResponderPergunta(pergunta));
    }

    IEnumerator ResponderPergunta(string pergunta)
    {
        respondendo = true;
        BloquearInterrogatorio();

        yield return StartCoroutine(MostrarDigitando());

        string resposta = ObterRespostaNormal(pergunta);

        yield return StartCoroutine(
            MostrarRespostaSuspeito(resposta, triggerNegacao)
        );

        yield return new WaitForSeconds(tempoRespostaVisivel);

        EsconderRespostaSuspeito();

        respondendo = false;
        LiberarInterrogatorio();
    }

    void BloquearInterrogatorio()
    {
        if (campoTexto != null)
            campoTexto.interactable = false;

        if (botaoEnviar != null)
            botaoEnviar.interactable = false;
    }

    void LiberarInterrogatorio()
    {
        if (confissaoIniciada)
            return;

        if (campoTexto != null)
        {
            campoTexto.interactable = true;
            campoTexto.ActivateInputField();
        }

        if (botaoEnviar != null)
            botaoEnviar.interactable = true;
    }

    IEnumerator MostrarDigitando()
    {
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
    }

    IEnumerator MostrarRespostaSuspeito(string mensagem, string trigger)
    {
        if (painelRespostaSuspeito != null)
            painelRespostaSuspeito.SetActive(true);

        if (textoResposta != null)
        {
            textoResposta.gameObject.SetActive(true);
            textoResposta.text = mensagem;
        }

        TocarAnimacao(trigger);

        yield return null;
    }

    void EsconderRespostaSuspeito()
    {
        if (textoResposta != null)
            textoResposta.text = "";

        if (painelRespostaSuspeito != null)
            painelRespostaSuspeito.SetActive(false);
        else if (textoResposta != null)
            textoResposta.gameObject.SetActive(false);
    }

    string ObterRespostaNormal(string pergunta)
    {
        string texto = Normalizar(pergunta);

        if (ContemAlgum(texto, "sozinho", "companhia", "com quem", "alguem com voce", "estava acompanhado", "estava com alguem", "quem estava com", "tinha alguem"))
            return "Eu estava com um conhecido naquela noite. Ele pode confirmar que nos encontramos.";

        if (ContemAlgum(texto, "o que fazia", "que fazia", "fazendo em casa", "fazendo naquele dia", "estava fazendo", "fazendo naquela noite", "estava ocupado", "fazendo no dia", "fazia perto"))
            return "Eu estava ocupado consertando algumas coisas em casa, mas precisei sair para resolver um problema.";

        if (ContemAlgum(texto, "conhecia", "conhece", "conhecer", "quem era aquele", "quem era a vitima", "relacao com", "amigo dele", "amigo dela", "conhecia aquele cara", "conhecia aquela pessoa", "conhecia a vitima", "como conheceu"))
            return "Eu trabalhava com a vitima. Nos conheciamos bem, embora nossa relacao tenha ficado complicada ultimamente.";

        if (ContemAlgum(texto, "onde estava", "onde voce estava", "estava em casa", "qual lugar", "estava aonde", "seu paradeiro", "foi para onde", "estava na casa"))
            return "Eu estava caminhando perto da praca naquela noite. Precisava sair um pouco de casa.";

        if (ContemAlgum(texto, "que horas", "horario", "22 horas", "dez da noite", "meia noite", "a que horas", "qual hora", "quando aconteceu", "horario do crime", "hora do crime", "na hora do crime", "perto do crime", "naquela hora", "naquele horario"))
            return "Por volta das 22 horas, eu estava perto do local do crime. Passei por la, mas nao parei.";

        if (ContemAlgum(texto, "saiu de casa", "sair de casa", "saiu naquela noite", "saiu naquele dia", "foi para fora", "deixou a casa", "saiu durante a noite"))
            return "Sai de casa naquela noite para encontrar uma pessoa. Nao achei que isso fosse importante.";

        if (ContemAlgum(texto, "discutiu", "discussao", "brigou", "briga", "desentendimento", "ameacou", "ameaca", "tinha problemas com"))
            return "Eu discuti seriamente com a vitima alguns dias antes. Ela estava me pressionando por causa de um segredo.";

        if (ContemAlgum(texto, "ouviu barulho", "escutou barulho", "grito", "barulho estranho", "ouviu alguma coisa", "escutou alguma coisa"))
            return "Eu ouvi um grito naquela noite. Achei melhor nao me envolver e continuei andando.";

        if (ContemAlgum(texto, "por que", "porque", "motivo", "razao", "raiva", "vinganca", "vingar"))
            return "A vitima sabia de algo que poderia acabar com a minha vida. Mas isso nao significa que eu teria coragem de fazer alguma coisa.";

        if (ContemAlgum(texto, "garrafa", "bebida", "frasco"))
            return "Essa garrafa era minha, mas eu a perdi naquela semana. Qualquer pessoa poderia ter pegado.";

        if (ContemAlgum(texto, "faca", "arma branca", "lamina", "objeto cortante"))
            return "Eu tinha uma faca parecida com essa. Usei para cortar algumas coisas, mas nao sei como ela foi parar nessa investigacao.";

        if (ContemAlgum(texto, "digital", "digitais", "impressao", "toquei", "tocou", "encostou", "encostei"))
            return "Eu toquei naquele objeto alguns dias antes. A vitima me mostrou algo relacionado a ele.";

        if (ContemAlgum(texto, "matou", "assassinou", "cometeu o crime", "foi voce", "culpado", "participou", "envolvido", "fez isso", "machucou"))
            return "Nao fui eu! Posso ter escondido algumas coisas, mas nunca faria algo tao grave.";

        if (ContemAlgum(texto, "o que aconteceu", "me conte", "conte tudo", "o que sabe", "sabe sobre o caso", "explique o que"))
            return "Eu encontrei a vitima naquela noite e tivemos uma conversa dificil. Depois fui embora.";

        if (ContemAlgum(texto, "vitima", "morto", "morta", "morreu", "corpo", "encontraram"))
            return "Eu vi a vitima pouco antes de tudo acontecer. Ela parecia nervosa e queria falar comigo.";

        return "Nao sei responder isso. Ja falei mais do que deveria sobre aquela noite.";
    }

    string ObterRespostaConfronto(string anotacao)
    {
        string texto = Normalizar(anotacao);

        if (ContemAlgum(texto, "sozinho", "companhia", "com quem", "alguem com voce", "estava acompanhado", "estava com alguem", "quem estava com", "tinha alguem"))
            return "Eu nao disse que estava acompanhado! Voce deve ter entendido errado. Eu estava sozinho.";

        if (ContemAlgum(texto, "o que fazia", "que fazia", "fazendo em casa", "fazendo naquele dia", "estava fazendo", "fazendo naquela noite", "estava ocupado", "fazendo no dia"))
            return "Eu nao estava fazendo isso! Voce esta misturando as coisas. Eu fiquei em casa.";

        if (ContemAlgum(texto, "conhecia", "conhece", "conhecer", "quem era a vitima", "relacao com", "amigo dele", "amigo dela", "como conheceu", "trabalhava com"))
            return "Eu mal conhecia aquela pessoa! Trabalhar no mesmo lugar nao significa que fossemos proximos.";

        if (ContemAlgum(texto, "onde estava", "onde voce estava", "estava em casa", "qual lugar", "estava aonde", "paradeiro", "foi para onde", "estava na casa", "caminhando", "praca"))
            return "Eu estava em casa naquela noite! Nao sei de onde voce tirou essa historia de que eu estava caminhando.";

        if (ContemAlgum(texto, "que horas", "horario", "22 horas", "dez da noite", "meia noite", "quando aconteceu", "horario do crime", "hora do crime", "naquela hora"))
            return "Nao lembro de ter dito isso. Tenho certeza de que ja estava em casa naquele horario.";

        if (ContemAlgum(texto, "saiu de casa", "sair de casa", "saiu naquela noite", "saiu naquele dia", "foi para fora", "deixou a casa", "saiu durante a noite"))
            return "Eu nao sai de casa naquela noite. Devo ter me expressado mal quando conversamos.";

        if (ContemAlgum(texto, "discutiu", "discussao", "brigou", "briga", "desentendimento", "ameacou", "ameaca", "problemas com"))
            return "Aquilo nao foi uma discussao de verdade. Tivemos apenas uma conversa e nada mais.";

        if (ContemAlgum(texto, "ouviu barulho", "escutou barulho", "grito", "barulho estranho", "ouviu alguma coisa"))
            return "Eu nao ouvi grito nenhum. Se alguem gritou, eu nao estava por perto.";

        if (ContemAlgum(texto, "por que", "porque", "motivo", "razao", "raiva", "vinganca", "vingar"))
            return "Eu nunca tive motivo para machucar aquela pessoa. Voce esta tentando transformar uma discussao em alguma coisa que ela nao foi.";

        if (ContemAlgum(texto, "garrafa", "bebida", "frasco"))
            return "Essa garrafa nao e minha! Nunca tive uma garrafa igual a essa.";

        if (ContemAlgum(texto, "faca", "arma branca", "lamina", "objeto cortante"))
            return "Eu nunca tive essa faca. Nao sei por que ela teria alguma ligacao comigo.";

        if (ContemAlgum(texto, "digital", "digitais", "impressao", "toquei", "tocou", "encostou", "encostei"))
            return "Essas digitais nao provam que eu cometi o crime. Posso ter tocado nesse objeto em outra ocasiao.";

        if (ContemAlgum(texto, "matou", "assassinou", "cometeu o crime", "foi voce", "culpado", "participou", "envolvido"))
            return "Nao fui eu! Voce esta usando minhas palavras contra mim, mas nao tem a historia completa.";

        return "Essa anotacao esta fora de contexto. Voce esta tentando me incriminar com palavras que nao provam nada.";
    }

    string Normalizar(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return "";

        string original = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder resultado = new StringBuilder();

        foreach (char letra in original)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letra) != UnicodeCategory.NonSpacingMark)
                resultado.Append(letra);
        }

        return resultado.ToString().Normalize(NormalizationForm.FormC);
    }

    bool ContemAlgum(string texto, params string[] palavras)
    {
        foreach (string palavra in palavras)
        {
            if (texto.Contains(Normalizar(palavra)))
                return true;
        }

        return false;
    }

    public void ApresentarGarrafa()
    {
        if (respondendo || confissaoIniciada || garrafaApresentada)
            return;

        garrafaApresentada = true;

        if (botaoGarrafa != null)
            botaoGarrafa.interactable = false;

        PararContador(ref contadorGarrafaCoroutine, contadorGarrafa);

        if (imagemGarrafa != null)
            imagemGarrafa.color = corPistaUsada;

        IniciarContadoresDasOutrasProvas("garrafa");
        StartCoroutine(ResponderPista(respostaGarrafa));
    }

    public void ApresentarFaca()
    {
        if (respondendo || confissaoIniciada || facaApresentada)
            return;

        facaApresentada = true;

        if (botaoFaca != null)
            botaoFaca.interactable = false;

        PararContador(ref contadorFacaCoroutine, contadorFaca);

        if (imagemFaca != null)
            imagemFaca.color = corPistaUsada;

        IniciarContadoresDasOutrasProvas("faca");
        StartCoroutine(ResponderPista(respostaFaca));
    }

    public void ApresentarDigital()
    {
        if (respondendo || confissaoIniciada || digitalApresentada)
            return;

        digitalApresentada = true;

        if (botaoDigital != null)
            botaoDigital.interactable = false;

        PararContador(ref contadorDigitalCoroutine, contadorDigital);

        if (imagemDigital != null)
            imagemDigital.color = corPistaUsada;

        IniciarContadoresDasOutrasProvas("digital");
        StartCoroutine(ResponderPista(respostaDigital));
    }

    void IniciarContadoresDasOutrasProvas(string provaUsada)
    {
        if (provaUsada != "garrafa" && !garrafaApresentada)
        {
            PararContador(ref contadorGarrafaCoroutine, contadorGarrafa);
            contadorGarrafaCoroutine = StartCoroutine(ContadorGarrafa());
        }

        if (provaUsada != "faca" && !facaApresentada)
        {
            PararContador(ref contadorFacaCoroutine, contadorFaca);
            contadorFacaCoroutine = StartCoroutine(ContadorFaca());
        }

        if (provaUsada != "digital" && !digitalApresentada)
        {
            PararContador(ref contadorDigitalCoroutine, contadorDigital);
            contadorDigitalCoroutine = StartCoroutine(ContadorDigital());
        }
    }

    void PararContador(ref Coroutine rotina, TMP_Text contador)
    {
        if (rotina != null)
        {
            StopCoroutine(rotina);
            rotina = null;
        }

        EsconderContador(contador);
    }

    void EsconderContador(TMP_Text contador)
    {
        if (contador == null)
            return;

        contador.text = "";
        contador.gameObject.SetActive(false);
    }

    IEnumerator ContadorGarrafa()
    {
        yield return StartCoroutine(
            ExecutarContador(contadorGarrafa, botaoGarrafa, () => garrafaApresentada)
        );

        contadorGarrafaCoroutine = null;
    }

    IEnumerator ContadorFaca()
    {
        yield return StartCoroutine(
            ExecutarContador(contadorFaca, botaoFaca, () => facaApresentada)
        );

        contadorFacaCoroutine = null;
    }

    IEnumerator ContadorDigital()
    {
        yield return StartCoroutine(
            ExecutarContador(contadorDigital, botaoDigital, () => digitalApresentada)
        );

        contadorDigitalCoroutine = null;
    }

    IEnumerator ExecutarContador(TMP_Text contador, Button botao, Func<bool> jaApresentada)
    {
        if (contador != null)
        {
            contador.gameObject.SetActive(true);
            contador.text = Mathf.CeilToInt(tempoProva).ToString();
        }

        if (botao != null)
            botao.interactable = false;

        float tempo = tempoProva;

        while (tempo > 0f)
        {
            if (jaApresentada())
            {
                EsconderContador(contador);
                yield break;
            }

            if (contador != null)
                contador.text = Mathf.CeilToInt(tempo).ToString();

            tempo -= Time.deltaTime;
            yield return null;
        }

        EsconderContador(contador);

        if (!jaApresentada() && !confissaoIniciada && botao != null)
            botao.interactable = true;
    }

    IEnumerator ResponderPista(string resposta)
    {
        respondendo = true;
        BloquearInterrogatorio();

        yield return StartCoroutine(MostrarDigitando());

        yield return StartCoroutine(
            MostrarRespostaSuspeito(resposta, triggerProva)
        );

        yield return new WaitForSeconds(tempoRespostaVisivel);

        EsconderRespostaSuspeito();

        if (TodasAsProvasApresentadas() && !confissaoIniciada)
        {
            confissaoIniciada = true;
            StartCoroutine(IniciarConfissao());
            yield break;
        }

        respondendo = false;
        LiberarInterrogatorio();
    }

    bool TodasAsProvasApresentadas()
    {
        return garrafaApresentada && facaApresentada && digitalApresentada;
    }

    IEnumerator IniciarConfissao()
    {
        confissaoIniciada = true;
        respondendo = true;

        PararContador(ref contadorGarrafaCoroutine, contadorGarrafa);
        PararContador(ref contadorFacaCoroutine, contadorFaca);
        PararContador(ref contadorDigitalCoroutine, contadorDigital);

        if (campoTexto != null)
            campoTexto.gameObject.SetActive(false);

        if (botaoEnviar != null)
            botaoEnviar.gameObject.SetActive(false);

        if (botaoOlharCaderno != null)
            botaoOlharCaderno.interactable = false;

        if (painelRespostaSuspeito != null)
            painelRespostaSuspeito.SetActive(false);

        if (textoResposta != null)
            textoResposta.text = "";

        if (textoDigitando != null)
            textoDigitando.gameObject.SetActive(false);

        if (painelCaderno != null)
            painelCaderno.SetActive(false);

        if (painelFalaJogador != null)
            painelFalaJogador.SetActive(false);

        if (painelProvas != null)
        {
            if (botaoGarrafa != null)
                botaoGarrafa.interactable = false;

            if (botaoFaca != null)
                botaoFaca.interactable = false;

            if (botaoDigital != null)
                botaoDigital.interactable = false;
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return new WaitForSeconds(esperaAntesConfissao);

        if (textoConfissao1 != null)
        {
            textoConfissao1.gameObject.SetActive(true);
            textoConfissao1.text = mensagemConfissao1;
        }

        TocarAnimacao(triggerConfissao1);

        yield return new WaitForSeconds(tempoConfissao1);

        if (textoConfissao1 != null)
            textoConfissao1.gameObject.SetActive(false);

        if (textoConfissao2 != null)
        {
            textoConfissao2.gameObject.SetActive(true);
            textoConfissao2.text = mensagemConfissao2;
        }

        TocarAnimacao(triggerConfissao2);

        yield return new WaitForSeconds(tempoConfissao2);

        if (textoConfissao2 != null)
            textoConfissao2.gameObject.SetActive(false);

        yield return StartCoroutine(FadeParaPreto());

        if (painelProvas != null)
            painelProvas.SetActive(false);

        SceneManager.LoadScene(nomeProximaCena);
    }

    void TocarAnimacao(string nomeTrigger)
    {
        if (animatorNpc == null || string.IsNullOrEmpty(nomeTrigger))
            return;

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
                    "Faca perguntas ao suspeito. Apresente as tres provas para descobrir a verdade.";
            }
        }

        yield return new WaitForSeconds(tempoPopup);

        if (popupExplicativo != null)
            popupExplicativo.SetActive(false);
    }

    IEnumerator FadeParaPreto()
    {
        if (fadeCanvasGroup == null)
            yield break;

        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = false;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(tempo / duracaoFade);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }
}
