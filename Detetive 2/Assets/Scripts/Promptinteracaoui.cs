using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class PromptInteracaoUI : MonoBehaviour
{
    public float duracaoAnimacao = 0.4f;
    public float distanciaFora = 300f; // o quanto ele começa deslocado pra fora da tela (direita)

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector2 posicaoFinal;
    private Vector2 posicaoFora;
    private Coroutine animacaoAtual;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        posicaoFinal = rect.anchoredPosition;
        posicaoFora = posicaoFinal + new Vector2(distanciaFora, 0f);

        // começa escondido, fora da tela à direita
        rect.anchoredPosition = posicaoFora;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);

        if (animacaoAtual != null)
            StopCoroutine(animacaoAtual);

        animacaoAtual = StartCoroutine(Animar(posicaoFora, posicaoFinal, 0f, 1f));
    }

    public void Esconder()
    {
        if (!gameObject.activeSelf) return;

        if (animacaoAtual != null)
            StopCoroutine(animacaoAtual);

        animacaoAtual = StartCoroutine(Animar(rect.anchoredPosition, posicaoFora, canvasGroup.alpha, 0f, true));
    }

    IEnumerator Animar(Vector2 de, Vector2 para, float alphaDe, float alphaPara, bool desativarNoFim = false)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracaoAnimacao;
            float tSuave = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f); // ease-out cúbico

            rect.anchoredPosition = Vector2.Lerp(de, para, tSuave);
            canvasGroup.alpha = Mathf.Lerp(alphaDe, alphaPara, tSuave);

            yield return null;
        }

        rect.anchoredPosition = para;
        canvasGroup.alpha = alphaPara;

        if (desativarNoFim)
            gameObject.SetActive(false);
    }
}