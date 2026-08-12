using UnityEngine;
using System.Collections;

public class ComputadorInteract : MonoBehaviour
{
    [Header("Computador")]
    public GameObject telaPC;

    [Header("Mira do jogador")]
    public GameObject mira;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float tempoFade = 0.5f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somEntrar;
    public AudioClip somSair;

    [Header("Mensagem")]
    public IntroUI introUI;
    public string mensagemAoAbrir = "Olhe o seu email";

    [Header("Email")]
    public GameObject telaEmail;

    [Header("Abas do computador")]
    public AbasComputador abasComputador;

    [System.Serializable]
    public class IconeTela
    {
        public GameObject icone;
        public GameObject tela;
    }

    public IconeTela[] telasPC;
    public GameObject[] semprevisiveis;

    private bool dentroDoComputador;
    private bool emailAberto;
    private bool emailEstavaAtivo;

    // =========================================================
    // VERIFICAÇÃO DO EMAIL
    // =========================================================

    void Update()
    {
        if (!dentroDoComputador)
            return;

        if (telaEmail == null)
            return;

        bool emailAtivoAgora =
            telaEmail.activeSelf;

        if (
            emailAtivoAgora &&
            !emailEstavaAtivo
        )
        {
            Debug.Log(
                "DETECTADO: Email foi aberto."
            );

            emailAberto = true;
            emailEstavaAtivo = true;

            if (introUI != null)
            {
                Debug.Log(
                    "Iniciando contador de 6 segundos."
                );

                introUI.EntrouNoEmail();
            }
            else
            {
                Debug.LogError(
                    "IntroUI não foi conectado!"
                );
            }
        }

        if (
            !emailAtivoAgora &&
            emailEstavaAtivo
        )
        {
            Debug.Log(
                "DETECTADO: Email foi fechado."
            );

            emailAberto = false;
            emailEstavaAtivo = false;

            if (introUI != null)
                introUI.SaiuDoEmail();
        }
    }

    // =========================================================
    // ENTRAR NO COMPUTADOR
    // =========================================================

    public void UsarComputador(PlayerMovement player)
    {
        if (player == null)
            return;

        player.usandoComputador = true;

        // Jogador fica parado enquanto usa o computador.
        player.velocidade = 0f;

        StartCoroutine(
            Abrir(player)
        );
    }

    // =========================================================
    // SAIR DO COMPUTADOR
    // =========================================================

    public void SairComputador(PlayerMovement player)
    {
        if (player == null)
            return;

        StartCoroutine(
            Fechar(player)
        );
    }

    // =========================================================
    // ABRIR COMPUTADOR
    // =========================================================

    IEnumerator Abrir(PlayerMovement player)
    {
        if (
            audioSource != null &&
            somEntrar != null
        )
        {
            audioSource.PlayOneShot(
                somEntrar
            );
        }

        yield return Fade(0, 1);

        if (telaPC != null)
            telaPC.SetActive(true);

        foreach (var t in telasPC)
        {
            if (t.tela != null)
                t.tela.SetActive(false);
        }

        foreach (var o in semprevisiveis)
        {
            if (o != null)
                o.SetActive(true);
        }

        if (mira != null)
            mira.SetActive(false);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        dentroDoComputador = true;

        emailAberto = false;
        emailEstavaAtivo = false;

        if (introUI != null)
        {
            introUI.MostrarMensagem(
                mensagemAoAbrir
            );
        }

        yield return Fade(1, 0);
    }

    // =========================================================
    // ABRIR EMAIL
    // =========================================================

    public void AbrirEmail()
    {
        if (!dentroDoComputador)
            return;

        if (telaEmail == null)
            return;

        telaEmail.SetActive(true);

        emailAberto = true;
        emailEstavaAtivo = true;

        Debug.Log(
            "AbrirEmail() chamado."
        );

        if (introUI != null)
        {
            introUI.EntrouNoEmail();
        }
        else
        {
            Debug.LogError(
                "IntroUI não está conectado."
            );
        }
    }

    // =========================================================
    // FECHAR EMAIL
    // =========================================================

    public void FecharEmail()
    {
        if (telaEmail == null)
            return;

        telaEmail.SetActive(false);

        emailAberto = false;
        emailEstavaAtivo = false;

        Debug.Log(
            "FecharEmail() chamado."
        );

        if (introUI != null)
        {
            introUI.SaiuDoEmail();
        }
    }

    // =========================================================
    // FECHAR COMPUTADOR
    // =========================================================

    IEnumerator Fechar(PlayerMovement player)
    {
        if (
            audioSource != null &&
            somSair != null
        )
        {
            audioSource.PlayOneShot(
                somSair
            );
        }

        if (emailAberto)
        {
            emailAberto = false;
            emailEstavaAtivo = false;

            if (introUI != null)
                introUI.SaiuDoEmail();
        }

        yield return Fade(0, 1);

        if (telaPC != null)
            telaPC.SetActive(false);

        if (mira != null)
            mira.SetActive(true);

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        dentroDoComputador = false;

        player.usandoComputador = false;

        // =====================================================
        // VELOCIDADE APÓS SAIR DO COMPUTADOR
        // =====================================================

        if (
            introUI != null &&
            introUI.InvestigacaoAtivada
        )
        {
            // Pega o valor configurado no Inspector
            // do PlayerMovement.
            player.velocidade =
                player.velocidadeAposInvestigacao;

            Debug.Log(
                "INVESTIGAÇÃO ATIVADA: velocidade do jogador = " +
                player.velocidadeAposInvestigacao
            );
        }
        else
        {
            // Continua parado.
            player.velocidade = 0f;

            Debug.Log(
                "Investigação ainda não ativada. Velocidade continua 0."
            );
        }

        yield return Fade(1, 0);
    }

    // =========================================================
    // FADE
    // =========================================================

    IEnumerator Fade(float a, float b)
    {
        float t = 0f;

        while (t < tempoFade)
        {
            t += Time.deltaTime;

            if (fadeCanvas != null)
            {
                fadeCanvas.alpha =
                    Mathf.Lerp(
                        a,
                        b,
                        t / tempoFade
                    );
            }

            yield return null;
        }

        if (fadeCanvas != null)
            fadeCanvas.alpha = b;
    }
}