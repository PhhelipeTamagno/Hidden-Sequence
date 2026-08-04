using UnityEngine;
using System.Collections;
using TMPro;

public class IntroUI : MonoBehaviour
{
    [Header("Fade de entrada")]
    public CanvasGroup fadeCanvasGroup; // arraste o CanvasGroup da FadeImage
    public float duracaoFade = 1.5f;

    [Header("Mensagem popup")]
    public RectTransform mensagemPC;    // arraste o RectTransform do painel/texto da mensagem
    public float duracaoAnimacaoPopup = 0.6f;

    [Header("Texto (efeito máquina de escrever)")]
    public TMP_Text textoMensagem;              // arraste o componente TMP do texto
    public float velocidadeEscrita = 0.04f;     // segundos entre cada letra

    private Vector2 posicaoFinalPopup;
    private Vector2 posicaoInicialPopup;
    private Coroutine sequenciaAtual;
    private bool primeiraExibicao = true; // só anima na primeira vez que a mensagem aparece

    void Start()
    {
        if (mensagemPC != null)
        {
            posicaoFinalPopup = mensagemPC.anchoredPosition;
            posicaoInicialPopup = posicaoFinalPopup + new Vector2(0f, 250f); // começa 150px acima
            mensagemPC.anchoredPosition = posicaoInicialPopup;

            CanvasGroup cgMsg = mensagemPC.GetComponent<CanvasGroup>();
            if (cgMsg != null)
                cgMsg.alpha = 0f;
        }

        if (textoMensagem != null)
            textoMensagem.text = "";

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        StartCoroutine(SequenciaIntro());
    }

    IEnumerator SequenciaIntro()
    {
        // 1. Fade de preto pra transparente
        if (fadeCanvasGroup != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duracaoFade;
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
        }
    }

    // Chame esse método de qualquer script (ex: ComputadorInteract) pra
    // mostrar o popup de novo com um texto diferente.
    public void MostrarMensagem(string texto)
    {
        if (sequenciaAtual != null)
            StopCoroutine(sequenciaAtual);

        if (primeiraExibicao)
        {
            // primeira vez: roda a animação de slide + máquina de escrever
            primeiraExibicao = false;
            sequenciaAtual = StartCoroutine(MostrarPopupComTexto(texto));
        }
        else
        {
            // próximas vezes: mostra direto, sem resetar/reanimar
            MostrarInstantaneo(texto);
        }
    }

    // Mostra o popup já na posição final, com o texto completo, sem animação.
    void MostrarInstantaneo(string texto)
    {
        if (mensagemPC != null)
        {
            mensagemPC.anchoredPosition = posicaoFinalPopup;
            CanvasGroup cgMsg = mensagemPC.GetComponent<CanvasGroup>();
            if (cgMsg != null)
                cgMsg.alpha = 1f;
        }

        if (textoMensagem != null)
            textoMensagem.text = texto;
    }

    IEnumerator MostrarPopupComTexto(string texto)
    {
        // reseta a posição/alpha caso o popup já tenha sumido antes
        if (mensagemPC != null)
        {
            mensagemPC.anchoredPosition = posicaoInicialPopup;
            CanvasGroup cgReset = mensagemPC.GetComponent<CanvasGroup>();
            if (cgReset != null)
                cgReset.alpha = 0f;
        }

        if (textoMensagem != null)
            textoMensagem.text = "";

        // 1. Popup desliza de cima pra baixo
        if (mensagemPC != null)
        {
            CanvasGroup cgMsg = mensagemPC.GetComponent<CanvasGroup>();
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duracaoAnimacaoPopup;
                float tSuave = 1f - Mathf.Pow(1f - t, 3f); // ease-out cúbico
                mensagemPC.anchoredPosition = Vector2.Lerp(posicaoInicialPopup, posicaoFinalPopup, tSuave);

                if (cgMsg != null)
                    cgMsg.alpha = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
            mensagemPC.anchoredPosition = posicaoFinalPopup;
            if (cgMsg != null)
                cgMsg.alpha = 1f;
        }

        // 2. Efeito de máquina de escrever
        if (textoMensagem != null)
        {
            for (int i = 0; i < texto.Length; i++)
            {
                textoMensagem.text += texto[i];
                yield return new WaitForSeconds(velocidadeEscrita);
            }
        }
    }

    // chame esse método quando quiser esconder o popup manualmente
    public void EsconderMensagem()
    {
        if (mensagemPC != null)
            StartCoroutine(FadeOutMensagem());
    }

    IEnumerator FadeOutMensagem()
    {
        CanvasGroup cgMsg = mensagemPC.GetComponent<CanvasGroup>();
        if (cgMsg == null) yield break;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            cgMsg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }
        cgMsg.alpha = 0f;
    }
}