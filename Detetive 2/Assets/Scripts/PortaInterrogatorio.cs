using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortaInterrogatorio : MonoBehaviour
{
    [Header("Cena")]
    public string nomeCenaInterrogatorio = "Interrogatorio";

    [Header("Jogador")]
    public Transform jogador;

    [Header("Interação")]
    public float distanciaInteracao = 3f;
    public GameObject painelPorta;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float duracaoFade = 1.5f;

    private bool podeInteragir = false;
    private bool trocandoCena = false;

    void Start()
    {
        if (painelPorta != null)
            painelPorta.SetActive(false);

        if (fadeCanvas != null)
        {
            // Começa invisível
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
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

        if (distancia <= distanciaInteracao)
        {
            if (!podeInteragir)
            {
                podeInteragir = true;

                if (painelPorta != null)
                    painelPorta.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(TrocarCena());
            }
        }
        else
        {
            if (podeInteragir)
            {
                podeInteragir = false;

                if (painelPorta != null)
                    painelPorta.SetActive(false);
            }
        }
    }

    IEnumerator TrocarCena()
    {
        trocandoCena = true;

        // Esconde o texto da porta
        if (painelPorta != null)
            painelPorta.SetActive(false);

        // FADE OUT
        if (fadeCanvas != null)
        {
            fadeCanvas.blocksRaycasts = true;

            float tempo = 0f;

            while (tempo < duracaoFade)
            {
                tempo += Time.deltaTime;

                float progresso = tempo / duracaoFade;

                fadeCanvas.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    progresso
                );

                yield return null;
            }

            // Garante que ficou completamente preto
            fadeCanvas.alpha = 1f;
        }

        // Espera um pequeno momento com a tela preta
        yield return new WaitForSeconds(0.2f);

        // Troca a cena
        SceneManager.LoadScene(nomeCenaInterrogatorio);
    }
}