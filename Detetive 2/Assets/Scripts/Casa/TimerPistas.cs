
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using EasyTransition;
using System.Collections;

public class TimerPistas : MonoBehaviour
{
    [Header("Tempo")]
    public float tempo = 60f;

    [Header("Pistas")]
    public int totalPistas = 5;
    public int pistasEncontradas = 0;

    [Header("Textos")]
    public TMP_Text textoTimer;
    public TMP_Text mensagem;

    [Header("Elementos que desaparecem quando o tempo acaba")]
    public GameObject[] elementosParaEsconder;

    [Header("Painel de fala")]
    public GameObject painelFala;
    public TMP_Text textoPersonagem;

    [Header("Mensagem quando o tempo acaba")]
    [TextArea(2, 5)]
    public string mensagemTempoAcabado =
        "Não consegui achar dentro do tempo, tiveram que soltar o suspeito por falta de prova.";

    [Header("Velocidade da digitação")]
    public float velocidadeDigitacao = 0.04f;

    [Header("Tempo antes da transição")]
    public float esperaDepoisDaFala = 2f;

    [Header("Easy Transition")]
    [SerializeField] private Transition transition;
    [SerializeField] private float delay = 1f;

    private bool reiniciando = false;

    private void Start()
    {
        if (mensagem != null)
            mensagem.text = "";

        if (painelFala != null)
            painelFala.SetActive(false);

        if (textoPersonagem != null)
            textoPersonagem.text = "";

        AtualizarTextoTimer();
    }

    private void Update()
    {
        if (reiniciando)
            return;

        if (pistasEncontradas >= totalPistas)
        {
            if (mensagem != null)
                mensagem.text = "Você conseguiu encontrar todas as pistas!";

            enabled = false;
            return;
        }

        tempo -= Time.deltaTime;

        AtualizarTextoTimer();

        if (tempo <= 0)
        {
            tempo = 0;
            AtualizarTextoTimer();

            reiniciando = true;

            StartCoroutine(MostrarFalaEReiniciar());
        }
    }

    private void AtualizarTextoTimer()
    {
        if (textoTimer != null)
            textoTimer.text = "Tempo: " + Mathf.Ceil(tempo);
    }

    public void EncontrouPista()
    {
        if (reiniciando || pistasEncontradas >= totalPistas)
            return;

        pistasEncontradas++;

        if (pistasEncontradas >= totalPistas)
        {
            if (mensagem != null)
                mensagem.text = "Você conseguiu encontrar todas as pistas!";

            enabled = false;
        }
    }

    private IEnumerator MostrarFalaEReiniciar()
    {
        if (elementosParaEsconder != null)
        {
            foreach (GameObject elemento in elementosParaEsconder)
            {
                if (elemento != null)
                    elemento.SetActive(false);
            }
        }

        if (mensagem != null)
            mensagem.gameObject.SetActive(false);

        if (painelFala != null)
            painelFala.SetActive(true);

        if (textoPersonagem != null)
        {
            textoPersonagem.text = "";

            foreach (char letra in mensagemTempoAcabado)
            {
                textoPersonagem.text += letra;

                yield return new WaitForSeconds(velocidadeDigitacao);
            }
        }
        else
        {
            Debug.LogWarning(
                "TimerPistas: Texto Personagem não configurado!"
            );
        }
        yield return new WaitForSeconds(esperaDepoisDaFala);

        if (transition != null)
        {
            transition.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "TimerPistas: Transition não configurada!"
            );
        }

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
