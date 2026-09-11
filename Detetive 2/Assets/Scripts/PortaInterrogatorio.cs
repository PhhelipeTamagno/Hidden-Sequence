using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EasyTransition;

public class PortaInterrogatorio : MonoBehaviour
{
    [Header("Cena")]
    [SerializeField] private string nomeCenaDestino = "Interrogatorio";

    [Header("Jogador")]
    [SerializeField] private Transform jogador;
    [SerializeField] private float distanciaInteracao = 3f;

    [Header("Fade + Texto")]
    [SerializeField] private GameObject fadeInteracao;

    [Header("Easy Transition")]
    [SerializeField] private Transition transition;
    [SerializeField] private float delay = 1f;

    private bool podeInteragir = false;
    private bool trocandoCena = false;

    private void Start()
    {
        if (fadeInteracao != null)
        {
            fadeInteracao.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "PortaInterrogatorio: FADE INTERACAO não foi colocado no Inspector!"
            );
        }
    }

    private void Update()
    {
        if (jogador == null)
        {
            Debug.LogWarning(
                "PortaInterrogatorio: JOGADOR não foi colocado no Inspector!"
            );

            return;
        }

        if (trocandoCena)
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

                Debug.Log("JOGADOR ENTROU NA ÁREA DA PORTA");

                AtivarFade();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E FOI APERTADO");

                IrParaInterrogatorio();
            }
        }

        else
        {
            if (podeInteragir)
            {
                podeInteragir = false;

                Debug.Log("JOGADOR SAIU DA ÁREA DA PORTA");

                DesativarFade();
            }
        }
    }

    private void AtivarFade()
    {
        if (fadeInteracao == null)
            return;

        fadeInteracao.SetActive(true);

        Debug.Log("FADE + TEXTO ATIVADO");
    }

    private void DesativarFade()
    {
        if (fadeInteracao == null)
            return;

        fadeInteracao.SetActive(false);

        Debug.Log("FADE + TEXTO DESATIVADO");
    }

    public void IrParaInterrogatorio()
    {
        if (trocandoCena)
            return;

        if (string.IsNullOrEmpty(nomeCenaDestino))
        {
            Debug.LogWarning(
                "PortaInterrogatorio: Nome da cena não foi definido!"
            );

            return;
        }

        trocandoCena = true;

        DesativarFade();

        StartCoroutine(TransicaoECarregarCena());
    }

    private IEnumerator TransicaoECarregarCena()
    {
        if (transition != null)
        {
            transition.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "PortaInterrogatorio: Transition não foi colocado no Inspector!"
            );
        }

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(nomeCenaDestino);
    }
}