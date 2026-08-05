using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ComputadorInteract : MonoBehaviour
{
    [Header("Computador")]
    public GameObject telaPC;

    [Header("Mira do jogador")]
    public GameObject mira; // Arraste a imagem da mira aqui

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float tempoFade = 0.5f;

    [Header("Mensagem")]
    public IntroUI introUI;
    public string mensagemAoAbrir = "Olhe o seu email";

    [System.Serializable]
    public class IconeTela
    {
        public GameObject icone;
        public GameObject tela;
    }

    public IconeTela[] telasPC;
    public GameObject[] semprevisiveis;

    bool dentroDoComputador;

    public void UsarComputador(PlayerMovement player)
    {
        player.usandoComputador = true;
        StartCoroutine(Abrir(player));
    }

    public void SairComputador(PlayerMovement player)
    {
        StartCoroutine(Fechar(player));
    }

    IEnumerator Abrir(PlayerMovement player)
    {
        yield return Fade(0, 1);

        telaPC.SetActive(true);

        foreach (var t in telasPC)
        {
            if (t.tela)
                t.tela.SetActive(false);
        }

        foreach (var o in semprevisiveis)
        {
            if (o)
                o.SetActive(true);
        }

        // Esconde a mira
        if (mira != null)
            mira.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        dentroDoComputador = true;

        if (introUI)
            introUI.MostrarMensagem(mensagemAoAbrir);

        yield return Fade(1, 0);
    }

    IEnumerator Fechar(PlayerMovement player)
    {
        yield return Fade(0, 1);

        telaPC.SetActive(false);

        // Mostra a mira novamente
        if (mira != null)
            mira.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dentroDoComputador = false;
        player.usandoComputador = false;

        yield return Fade(1, 0);
    }

    IEnumerator Fade(float a, float b)
    {
        float t = 0;

        while (t < tempoFade)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(a, b, t / tempoFade);
            yield return null;
        }

        fadeCanvas.alpha = b;
    }
}