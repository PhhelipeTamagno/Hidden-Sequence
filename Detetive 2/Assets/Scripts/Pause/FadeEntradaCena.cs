using UnityEngine;
using System.Collections;

public class FadeEntradaCena : MonoBehaviour
{
    [Header("Fade de entrada")]
    public CanvasGroup fadeCanvas;

    public float duracaoFade = 1f;

    void Start()
    {
        if (fadeCanvas == null)
            return;
        fadeCanvas.alpha = 1f;
        fadeCanvas.blocksRaycasts = true;

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;

            fadeCanvas.alpha = Mathf.Lerp(
                1f,
                0f,
                tempo / duracaoFade
            );

            yield return null;
        }

        fadeCanvas.alpha = 0f;
        fadeCanvas.blocksRaycasts = false;
    }
}