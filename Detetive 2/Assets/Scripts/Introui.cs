using UnityEngine;
using System.Collections;
using TMPro;

public class IntroUI : MonoBehaviour
{
    [Header("Fade de entrada")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1.5f;

    [Header("Mensagem popup")]
    public RectTransform mensagemPC;
    public float duracaoAnimacaoPopup = 0.6f;

    [Header("Texto (efeito máquina de escrever)")]
    public TMP_Text textoMensagem;
    public float velocidadeEscrita = 0.04f;

    [Header("Mensagem do jogador")]
    public RectTransform mensagemJogador;
    public TMP_Text textoMensagemJogador;
    public float duracaoAnimacaoJogador = 0.6f;

    [Header("Configuração da investigação")]
    public float tempoParaInvestigar = 6f;

    private Vector2 posicaoFinalPopup;
    private Vector2 posicaoInicialPopup;

    private Vector2 posicaoFinalJogador;
    private Vector2 posicaoInicialJogador;

    private Coroutine sequenciaAtual;
    private Coroutine mensagemJogadorAtual;
    private Coroutine contadorInvestigacao;

    private bool investigacaoAtivada = false;
    private bool dentroDoEmail = false;

    // Permite que o ComputadorInteract saiba
    // se a mensagem de investigação já apareceu.
    public bool InvestigacaoAtivada
    {
        get { return investigacaoAtivada; }
    }

    void Start()
    {
        if (mensagemPC != null)
        {
            posicaoFinalPopup =
                mensagemPC.anchoredPosition;

            posicaoInicialPopup =
                posicaoFinalPopup +
                new Vector2(0f, 250f);

            mensagemPC.anchoredPosition =
                posicaoInicialPopup;

            CanvasGroup cgMsg =
                mensagemPC.GetComponent<CanvasGroup>();

            if (cgMsg != null)
                cgMsg.alpha = 0f;
        }

        if (textoMensagem != null)
            textoMensagem.text = "";

        if (mensagemJogador != null)
        {
            posicaoFinalJogador =
                mensagemJogador.anchoredPosition;

            posicaoInicialJogador =
                posicaoFinalJogador +
                new Vector2(0f, -150f);

            mensagemJogador.anchoredPosition =
                posicaoInicialJogador;

            CanvasGroup cgJogador =
                mensagemJogador.GetComponent<CanvasGroup>();

            if (cgJogador != null)
                cgJogador.alpha = 0f;

            mensagemJogador.gameObject.SetActive(false);
        }

        if (textoMensagemJogador != null)
            textoMensagemJogador.text = "";

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        StartCoroutine(
            SequenciaIntro()
        );
    }

    IEnumerator SequenciaIntro()
    {
        if (fadeCanvasGroup != null)
        {
            float t = 0f;

            while (t < 1f)
            {
                t +=
                    Time.deltaTime /
                    duracaoFade;

                fadeCanvasGroup.alpha =
                    Mathf.Lerp(
                        1f,
                        0f,
                        t
                    );

                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
        }
    }

    // =========================================================
    // EMAIL
    // =========================================================

    public void EntrouNoEmail()
    {
        Debug.Log(
            "INTRO UI: Entrou no email."
        );

        dentroDoEmail = true;

        if (investigacaoAtivada)
        {
            Debug.Log(
                "INTRO UI: A investigação já foi ativada."
            );

            return;
        }

        if (contadorInvestigacao != null)
        {
            StopCoroutine(
                contadorInvestigacao
            );

            contadorInvestigacao = null;
        }

        contadorInvestigacao =
            StartCoroutine(
                ContarTempoInvestigacao()
            );

        Debug.Log(
            "INTRO UI: Contador de 6 segundos iniciado."
        );
    }

    public void SaiuDoEmail()
    {
        Debug.Log(
            "INTRO UI: Saiu do email."
        );

        dentroDoEmail = false;

        if (!investigacaoAtivada)
        {
            CancelarContadorInvestigacao();
        }
    }

    // =========================================================
    // MENSAGEM PRINCIPAL
    // =========================================================

    public void MostrarMensagem(string texto)
    {
        if (sequenciaAtual != null)
            StopCoroutine(
                sequenciaAtual
            );

        sequenciaAtual =
            StartCoroutine(
                MostrarPopupComTexto(texto)
            );
    }

    IEnumerator MostrarPopupComTexto(string texto)
    {
        if (mensagemPC != null)
        {
            mensagemPC.gameObject.SetActive(true);

            mensagemPC.anchoredPosition =
                posicaoInicialPopup;

            CanvasGroup cgReset =
                mensagemPC.GetComponent<CanvasGroup>();

            if (cgReset != null)
                cgReset.alpha = 0f;
        }

        if (textoMensagem != null)
            textoMensagem.text = "";

        if (mensagemPC != null)
        {
            CanvasGroup cgMsg =
                mensagemPC.GetComponent<CanvasGroup>();

            float t = 0f;

            while (t < 1f)
            {
                t +=
                    Time.deltaTime /
                    duracaoAnimacaoPopup;

                float tSuave =
                    1f -
                    Mathf.Pow(
                        1f - t,
                        3f
                    );

                mensagemPC.anchoredPosition =
                    Vector2.Lerp(
                        posicaoInicialPopup,
                        posicaoFinalPopup,
                        tSuave
                    );

                if (cgMsg != null)
                {
                    cgMsg.alpha =
                        Mathf.Lerp(
                            0f,
                            1f,
                            t
                        );
                }

                yield return null;
            }

            mensagemPC.anchoredPosition =
                posicaoFinalPopup;

            if (cgMsg != null)
                cgMsg.alpha = 1f;
        }

        if (textoMensagem != null)
        {
            for (
                int i = 0;
                i < texto.Length;
                i++
            )
            {
                textoMensagem.text +=
                    texto[i];

                yield return new WaitForSeconds(
                    velocidadeEscrita
                );
            }
        }

        yield return new WaitForSeconds(6f);

        EsconderMensagem();
    }

    // =========================================================
    // CONTADOR DA INVESTIGAÇÃO
    // =========================================================

    IEnumerator ContarTempoInvestigacao()
    {
        float tempo = 0f;

        while (
            tempo <
            tempoParaInvestigar
        )
        {
            if (!dentroDoEmail)
            {
                Debug.Log(
                    "INTRO UI: Contador cancelado porque saiu do email."
                );

                contadorInvestigacao = null;

                yield break;
            }

            tempo +=
                Time.deltaTime;

            Debug.Log(
                "Tempo no email: " +
                tempo.ToString("F1") +
                " / " +
                tempoParaInvestigar
            );

            yield return null;
        }

        if (!dentroDoEmail)
        {
            contadorInvestigacao = null;

            yield break;
        }

        investigacaoAtivada = true;

        contadorInvestigacao = null;

        Debug.Log(
            "INTRO UI: 6 SEGUNDOS COMPLETOS!"
        );

        MostrarMensagem(
            "Vá investigar o suspeito."
        );

        MostrarMensagemJogador(
            "Vou lá fazer um rápido interrogatório com esse cara, antes de ir procurar e coletar as provas."
        );
    }

    // =========================================================
    // MENSAGEM DO JOGADOR
    // =========================================================

    public void MostrarMensagemJogador(string texto)
    {
        if (mensagemJogadorAtual != null)
            StopCoroutine(
                mensagemJogadorAtual
            );

        mensagemJogadorAtual =
            StartCoroutine(
                AnimarMensagemJogador(texto)
            );
    }

    IEnumerator AnimarMensagemJogador(string texto)
    {
        if (
            mensagemJogador == null ||
            textoMensagemJogador == null
        )
        {
            Debug.LogError(
                "Mensagem do jogador não foi configurada no Inspector."
            );

            yield break;
        }

        mensagemJogador.gameObject.SetActive(true);

        CanvasGroup cg =
            mensagemJogador.GetComponent<CanvasGroup>();

        if (cg == null)
        {
            Debug.LogError(
                "O painel Mensagem Jogador precisa ter um CanvasGroup."
            );

            yield break;
        }

        cg.alpha = 0f;

        mensagemJogador.anchoredPosition =
            posicaoInicialJogador;

        textoMensagemJogador.text = "";

        float t = 0f;

        while (t < 1f)
        {
            t +=
                Time.deltaTime /
                duracaoAnimacaoJogador;

            float tSuave =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            mensagemJogador.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicialJogador,
                    posicaoFinalJogador,
                    tSuave
                );

            cg.alpha =
                Mathf.Lerp(
                    0f,
                    1f,
                    t
                );

            yield return null;
        }

        mensagemJogador.anchoredPosition =
            posicaoFinalJogador;

        cg.alpha = 1f;

        for (
            int i = 0;
            i < texto.Length;
            i++
        )
        {
            textoMensagemJogador.text +=
                texto[i];

            yield return new WaitForSeconds(
                velocidadeEscrita
            );
        }

        yield return new WaitForSeconds(5f);

        EsconderMensagemJogador();
    }

    // =========================================================
    // ESCONDER MENSAGEM
    // =========================================================

    public void EsconderMensagem()
    {
        if (mensagemPC != null)
        {
            StartCoroutine(
                FadeOutMensagem()
            );
        }
    }

    IEnumerator FadeOutMensagem()
    {
        CanvasGroup cg =
            mensagemPC.GetComponent<CanvasGroup>();

        if (cg == null)
            yield break;

        float t = 0f;

        while (t < 1f)
        {
            t +=
                Time.deltaTime /
                0.4f;

            cg.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    t
                );

            yield return null;
        }

        cg.alpha = 0f;

        mensagemPC.gameObject.SetActive(false);
    }

    // =========================================================
    // ESCONDER MENSAGEM DO JOGADOR
    // =========================================================

    public void EsconderMensagemJogador()
    {
        if (mensagemJogador != null)
        {
            StartCoroutine(
                FadeOutMensagemJogador()
            );
        }
    }

    IEnumerator FadeOutMensagemJogador()
    {
        CanvasGroup cg =
            mensagemJogador.GetComponent<CanvasGroup>();

        if (cg == null)
            yield break;

        float t = 0f;

        while (t < 1f)
        {
            t +=
                Time.deltaTime /
                0.4f;

            cg.alpha =
                Mathf.Lerp(
                    1f,
                    0f,
                    t
                );

            yield return null;
        }

        cg.alpha = 0f;

        mensagemJogador.gameObject.SetActive(false);
    }

    // =========================================================
    // CANCELAR CONTADOR
    // =========================================================

    public void CancelarContadorInvestigacao()
    {
        if (contadorInvestigacao != null)
        {
            StopCoroutine(
                contadorInvestigacao
            );

            contadorInvestigacao = null;
        }
    }

    // =========================================================
    // RESETAR INVESTIGAÇÃO
    // =========================================================

    public void ResetarInvestigacao()
    {
        CancelarContadorInvestigacao();

        if (mensagemJogadorAtual != null)
        {
            StopCoroutine(
                mensagemJogadorAtual
            );

            mensagemJogadorAtual = null;
        }

        investigacaoAtivada = false;
        dentroDoEmail = false;

        if (mensagemJogador != null)
        {
            CanvasGroup cg =
                mensagemJogador.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f;

            mensagemJogador.anchoredPosition =
                posicaoInicialJogador;

            mensagemJogador.gameObject.SetActive(false);
        }

        if (textoMensagemJogador != null)
            textoMensagemJogador.text = "";

        if (mensagemPC != null)
        {
            CanvasGroup cg =
                mensagemPC.GetComponent<CanvasGroup>();

            if (cg != null)
                cg.alpha = 0f;

            mensagemPC.anchoredPosition =
                posicaoInicialPopup;

            mensagemPC.gameObject.SetActive(false);
        }

        if (textoMensagem != null)
            textoMensagem.text = "";
    }
}