using TMPro;
using UnityEngine;
using System.Collections;

public class CenaDoCrimeIntro : MonoBehaviour
{
    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1.5f;

    [Header("Popup")]
    public RectTransform popup;          
    public CanvasGroup popupCanvasGroup; 
    public TMP_Text textoPopup;

    [TextArea]
    public string mensagemPopup = "Entre no carro.";

    [TextArea]
    [Tooltip("Mensagem exibida quando o player entra no carro (chamada pelo CarroInteragir)")]
    public string mensagemCenaDoCrime = "Vá até a cena do crime.";

    public float velocidadeEscrita = 0.03f;
    public float esperaAntesDoPopup = 0.3f;

    [Header("Animação do Popup")]
    public float duracaoAnimacaoPopup = 0.5f;
    public float distanciaQueda = 100f; 

    private Vector2 posicaoFinalPopup;
    private Coroutine popupRoutine;

    void Start()
    {
        
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;

        posicaoFinalPopup = popup.anchoredPosition;
        popup.gameObject.SetActive(false);

        StartCoroutine(IntroCena());
    }

    IEnumerator IntroCena()
    {
        yield return StartCoroutine(FadeParaTransparente());
        yield return new WaitForSeconds(esperaAntesDoPopup);
        MostrarPopup(mensagemPopup);
    }

  
    public void MostrarPopupCenaDoCrime()
    {
        MostrarPopup(mensagemCenaDoCrime);
    }

    
    public void MostrarPopup(string mensagem)
    {
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(MostrarPopupRotina(mensagem));
    }

    IEnumerator MostrarPopupRotina(string mensagem)
    {
       
        if (popup.gameObject.activeSelf)
            yield return StartCoroutine(EsconderPopup());

        yield return StartCoroutine(AnimarPopup());
        yield return StartCoroutine(EscreverTexto(textoPopup, mensagem));
    }

    IEnumerator EsconderPopup()
    {
        float tempo = 0f;
        float alphaInicial = popupCanvasGroup != null ? popupCanvasGroup.alpha : 1f;

        while (tempo < duracaoAnimacaoPopup)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoAnimacaoPopup);

            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = Mathf.Lerp(alphaInicial, 0f, t);

            yield return null;
        }

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0f;

        popup.gameObject.SetActive(false);
    }

    IEnumerator AnimarPopup()
    {
        popup.gameObject.SetActive(true);

        Vector2 posicaoInicial = posicaoFinalPopup + Vector2.up * distanciaQueda;
        popup.anchoredPosition = posicaoInicial;

        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0f;

        float tempo = 0f;
        while (tempo < duracaoAnimacaoPopup)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoAnimacaoPopup);
            float tSuave = 1f - Mathf.Pow(1f - t, 3f);

            popup.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoFinalPopup, tSuave);

            if (popupCanvasGroup != null)
                popupCanvasGroup.alpha = tSuave;

            yield return null;
        }

        popup.anchoredPosition = posicaoFinalPopup;
        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 1f;
    }

    IEnumerator FadeParaTransparente()
    {
        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(tempo / duracaoFade);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
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
}