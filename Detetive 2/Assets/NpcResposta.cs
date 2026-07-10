using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCResposta : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField campoTexto;
    public TMP_Text textoNPC;

    [Header("Mensagem Final")]
    public TMP_Text textoMensagem;
    [TextArea]
    public string mensagemFinal =
        "Após interrogar o suspeito, você decide seguir para a cena do crime em busca de pistas.";
    public float velocidadeEscrita = 0.03f;

    private int perguntasFeitas = 0;
    public TMP_Text textoResposta;
    public TMP_Text textoDigitando;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1.5f;

    [Header("Cena")]
    public string nomeProximaCena = "CenaDoCrime";
    public float esperaAposTexto = 2f;

    IEnumerator FinalInterrogatorio()
    {
        // Fade para preto primeiro
        yield return StartCoroutine(FadeParaPreto());

        // Depois do fade, esconde a UI de pergunta
        textoResposta.gameObject.SetActive(false);
        textoDigitando.gameObject.SetActive(false);
        campoTexto.gameObject.SetActive(false);

        // Ativa e escreve o texto final aos poucos
        textoMensagem.gameObject.SetActive(true);
        yield return StartCoroutine(EscreverTexto(textoMensagem, mensagemFinal));

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
        fadeCanvasGroup.blocksRaycasts = true;

        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(tempo / duracaoFade);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }

    public void EnviarPergunta()
    {
        string pergunta = campoTexto.text;
        if (string.IsNullOrWhiteSpace(pergunta))
            return;

        perguntasFeitas++;
        campoTexto.text = "";
        StartCoroutine(ResponderNPC());

        if (perguntasFeitas >= 3)
        {
            StartCoroutine(FinalInterrogatorio());
        }
    }

    IEnumerator ResponderNPC()
    {
        textoResposta.text = "";
        textoDigitando.text = ".";
        yield return new WaitForSeconds(0.3f);
        textoDigitando.text = "..";
        yield return new WaitForSeconds(0.3f);
        textoDigitando.text = "...";
        yield return new WaitForSeconds(1f);
        textoDigitando.text = "";
        textoResposta.text = "Não sei de nada. Eu não estava envolvido.";
    }
}