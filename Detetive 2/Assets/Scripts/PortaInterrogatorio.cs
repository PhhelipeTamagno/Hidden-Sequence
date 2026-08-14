using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortaInterrogatorio : MonoBehaviour
{
    [Header("Cena")]
    public string nomeCenaDestino;

    [Header("Jogador")]
    public Transform jogador;

    [Header("Interação")]
    public float distanciaInteracao = 3f;
    public GameObject painelInteracao;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float duracaoFade = 1.5f;

    private bool podeInteragir = false;
    private bool trocandoCena = false;

    void Start()
    {
        // Esconde o painel no início
        if (painelInteracao != null)
            painelInteracao.SetActive(false);

        // Fade começa transparente
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
            fadeCanvas.interactable = false;
        }
    }

    void Update()
    {
        if (jogador == null || trocandoCena)
            return;

        float distancia = Vector3.Distance(
            jogador.position,
            transform.position
        );

        // Jogador está perto
        if (distancia <= distanciaInteracao)
        {
            if (!podeInteragir)
            {
                podeInteragir = true;

                if (painelInteracao != null)
                    painelInteracao.SetActive(true);
            }

            // Pressionou E
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!string.IsNullOrEmpty(nomeCenaDestino))
                {
                    StartCoroutine(TrocarCena());
                }
                else
                {
                    Debug.LogWarning(
                        "TransicaoCena: Nenhuma cena de destino foi definida!"
                    );
                }
            }
        }
        else
        {
            if (podeInteragir)
            {
                podeInteragir = false;

                if (painelInteracao != null)
                    painelInteracao.SetActive(false);
            }
        }
    }

    IEnumerator TrocarCena()
    {
        trocandoCena = true;

        // Esconde o painel
        if (painelInteracao != null)
            painelInteracao.SetActive(false);

        // FADE OUT
        if (fadeCanvas != null)
        {
            fadeCanvas.blocksRaycasts = true;

            float tempo = 0f;

            while (tempo < duracaoFade)
            {
                tempo += Time.deltaTime;

                float progresso = Mathf.Clamp01(
                    tempo / duracaoFade
                );

                fadeCanvas.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    progresso
                );

                yield return null;
            }

            fadeCanvas.alpha = 1f;
        }

        // Pequena espera com a tela preta
        yield return new WaitForSeconds(0.2f);

        // Troca de cena
        SceneManager.LoadScene(nomeCenaDestino);
    }
}