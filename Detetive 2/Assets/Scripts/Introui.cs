
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
    public float tempoParaInvestigar = 10f;

    private Vector2 posicaoFinalPopup;
    private Vector2 posicaoInicialPopup;

    private Vector2 posicaoFinalJogador;
    private Vector2 posicaoInicialJogador;

    private Coroutine sequenciaAtual;
    private Coroutine mensagemJogadorAtual;
    private Coroutine contadorInvestigacao;

    private bool investigacaoAtivada = false;
    private bool dentroDoEmail = false;

    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        // ============================================================
        // MENSAGEM PRINCIPAL
        // ============================================================

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

        // ============================================================
        // MENSAGEM DO JOGADOR
        // ============================================================

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

            // Começa desativado
            mensagemJogador.gameObject.SetActive(false);
        }

        if (textoMensagemJogador != null)
            textoMensagemJogador.text = "";

        // ============================================================
        // FADE
        // ============================================================

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        StartCoroutine(SequenciaIntro());
    }

    // ============================================================
    // FADE INICIAL
    // ============================================================

    IEnumerator SequenciaIntro()
    {
        if (fadeCanvasGroup != null)
        {
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duracaoFade;

                fadeCanvasGroup.alpha =
                    Mathf.Lerp(1f, 0f, t);

                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
        }
    }

    // ============================================================
    // ENTROU NO EMAIL
    // ============================================================

    public void EntrouNoEmail()
    {
        Debug.Log("INTRO UI: Entrou no email.");

        dentroDoEmail = true;

        // Se já aconteceu, não faz novamente
        if (investigacaoAtivada)
        {
            Debug.Log(
                "INTRO UI: A mensagem já foi trocada."
            );

            return;
        }

        // Cancela contador anterior caso exista
        if (contadorInvestigacao != null)
        {
            StopCoroutine(contadorInvestigacao);
            contadorInvestigacao = null;
        }

        // Começa os 10 segundos
        contadorInvestigacao =
            StartCoroutine(
                ContarTempoInvestigacao()
            );

        Debug.Log(
            "INTRO UI: Contador de 10 segundos iniciado."
        );
    }

    // ============================================================
    // SAIU DO EMAIL
    // ============================================================

    public void SaiuDoEmail()
    {
        Debug.Log("INTRO UI: Saiu do email.");

        dentroDoEmail = false;

        // Se ainda não completou os 10 segundos,
        // cancela o contador
        if (!investigacaoAtivada)
        {
            CancelarContadorInvestigacao();
        }
    }

    // ============================================================
    // MOSTRAR MENSAGEM PRINCIPAL
    // ============================================================

    public void MostrarMensagem(string texto)
    {
        if (sequenciaAtual != null)
            StopCoroutine(sequenciaAtual);

        sequenciaAtual =
            StartCoroutine(
                MostrarPopupComTexto(texto)
            );
    }

    // ============================================================
    // ANIMAÇÃO DA MENSAGEM PRINCIPAL
    // ============================================================

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

        // ============================================================
        // SLIDE DO POPUP
        // ============================================================

        if (mensagemPC != null)
        {
            CanvasGroup cgMsg =
                mensagemPC.GetComponent<CanvasGroup>();

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime /
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

        // ============================================================
        // MÁQUINA DE ESCREVER
        // ============================================================

        if (textoMensagem != null)
        {
            for (int i = 0; i < texto.Length; i++)
            {
                textoMensagem.text += texto[i];

                yield return new WaitForSeconds(
                    velocidadeEscrita
                );
            }
        }

        yield return new WaitForSeconds(6f);

        EsconderMensagem();
    }

    // ============================================================
    // CONTADOR DOS 10 SEGUNDOS
    // ============================================================

    IEnumerator ContarTempoInvestigacao()
    {
        float tempo = 0f;

        while (tempo < tempoParaInvestigar)
        {
            // Se sair do email antes dos 10 segundos,
            // cancela a contagem
            if (!dentroDoEmail)
            {
                Debug.Log(
                    "INTRO UI: Contador cancelado porque saiu do email."
                );

                contadorInvestigacao = null;

                yield break;
            }

            tempo += Time.deltaTime;

            Debug.Log(
                "Tempo no email: " +
                tempo.ToString("F1") +
                " / " +
                tempoParaInvestigar
            );

            yield return null;
        }

        // Confirma que ainda está no email
        if (!dentroDoEmail)
        {
            contadorInvestigacao = null;
            yield break;
        }

        // ============================================================
        // ATIVA A INVESTIGAÇÃO
        // ============================================================

        investigacaoAtivada = true;
        contadorInvestigacao = null;

        Debug.Log(
            "INTRO UI: 10 SEGUNDOS COMPLETOS!"
        );

        // ============================================================
        // TROCA A MENSAGEM DO QUADRO
        // ============================================================

        MostrarMensagem(
            "Vá investigar o suspeito."
        );

        // ============================================================
        // ATIVA A FALA DO JOGADOR
        // ============================================================

        MostrarMensagemJogador(
            "Vou lá interrogar esse Remi e ver o que ele tem para dizer."
        );
    }

    // ============================================================
    // MENSAGEM DO JOGADOR
    // ============================================================

    public void MostrarMensagemJogador(string texto)
    {
        if (mensagemJogadorAtual != null)
            StopCoroutine(mensagemJogadorAtual);

        mensagemJogadorAtual =
            StartCoroutine(
                AnimarMensagemJogador(texto)
            );
    }

    // ============================================================
    // ANIMAÇÃO DO JOGADOR
    // ============================================================

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

        // ============================================================
        // ATIVA O PAINEL
        // ============================================================

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

        // ============================================================
        // RESET DA ANIMAÇÃO
        // ============================================================

        cg.alpha = 0f;

        mensagemJogador.anchoredPosition =
            posicaoInicialJogador;

        textoMensagemJogador.text = "";

        // ============================================================
        // ENTRADA
        // ============================================================

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime /
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

        // ============================================================
        // MÁQUINA DE ESCREVER
        // ============================================================

        for (int i = 0; i < texto.Length; i++)
        {
            textoMensagemJogador.text += texto[i];

            yield return new WaitForSeconds(
                velocidadeEscrita
            );
        }

        // ============================================================
        // ESPERA
        // ============================================================

        yield return new WaitForSeconds(5f);

        // ============================================================
        // ESCONDE
        // ============================================================

        EsconderMensagemJogador();
    }

    // ============================================================
    // ESCONDER MENSAGEM PRINCIPAL
    // ============================================================

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
            t += Time.deltaTime / 0.4f;

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

    // ============================================================
    // ESCONDER MENSAGEM DO JOGADOR
    // ============================================================

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
            t += Time.deltaTime / 0.4f;

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

    // ============================================================
    // CANCELAR CONTADOR
    // ============================================================

    public void CancelarContadorInvestigacao()
    {
        if (contadorInvestigacao != null)
        {
            StopCoroutine(contadorInvestigacao);

            contadorInvestigacao = null;
        }
    }

    // ============================================================
    // RESETAR INVESTIGAÇÃO
    // ============================================================

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

        // ============================================================
        // RESET DO JOGADOR
        // ============================================================

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

        // ============================================================
        // RESET DA MENSAGEM PRINCIPAL
        // ============================================================

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

