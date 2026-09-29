using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreditosJogo : MonoBehaviour
{
    [Header("Texto dos creditos")]
    public TMP_Text textoCreditos;
    public float velocidadeTexto = 50f;
    public float posicaoInicialY = -600f;
    public float posicaoFinalY = 1000f;

    [Header("Imagens")]
    public Image imagem1;
    public Image imagem2;
    public float tempoImagem1 = 3f;
    public float tempoImagem2 = 3f;
    public float tempoFade = 1f;

    [Header("Inicio")]
    public float atrasoInicial = 0f;

    private RectTransform textoRect;
    private RectTransform imagem1Rect;
    private RectTransform imagem2Rect;

    private void Start()
    {
        if (textoCreditos != null)
        {
            textoRect = textoCreditos.GetComponent<RectTransform>();

            Vector2 pos = textoRect.anchoredPosition;
            pos.y = posicaoInicialY;
            textoRect.anchoredPosition = pos;
        }

        PrepararImagem(imagem1);
        PrepararImagem(imagem2);

        StartCoroutine(IniciarCreditos());
    }

    private IEnumerator IniciarCreditos()
    {
        yield return new WaitForSeconds(atrasoInicial);

        StartCoroutine(AnimarImagem(imagem1, tempoImagem1));
        StartCoroutine(AnimarImagem(imagem2, tempoImagem2));

        while (textoRect != null && textoRect.anchoredPosition.y < posicaoFinalY)
        {
            Vector2 pos = textoRect.anchoredPosition;
            pos.y += velocidadeTexto * Time.deltaTime;
            textoRect.anchoredPosition = pos;

            yield return null;
        }
    }

    private IEnumerator AnimarImagem(Image imagem, float tempoEspera)
    {
        if (imagem == null)
            yield break;

        yield return new WaitForSeconds(tempoEspera);

        yield return FadeImagem(imagem, 0f, 1f, tempoFade);

        yield return new WaitForSeconds(tempoEspera);

        yield return FadeImagem(imagem, 1f, 0f, tempoFade);
    }

    private IEnumerator FadeImagem(Image imagem, float inicio, float fim, float duracao)
    {
        if (duracao <= 0)
        {
            SetAlpha(imagem, fim);
            yield break;
        }

        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float valor = Mathf.Lerp(inicio, fim, tempo / duracao);
            SetAlpha(imagem, valor);

            yield return null;
        }

        SetAlpha(imagem, fim);
    }

    private void PrepararImagem(Image imagem)
    {
        if (imagem == null)
            return;

        SetAlpha(imagem, 0f);
    }

    private void SetAlpha(Image imagem, float alpha)
    {
        Color cor = imagem.color;
        cor.a = alpha;
        imagem.color = cor;
    }
}