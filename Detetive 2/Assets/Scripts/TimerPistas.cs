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

    [Header("Texto")]
    public TMP_Text textoTimer;
    public TMP_Text mensagem;

    [Header("Easy Transition")]
    [SerializeField] private Transition transition;
    [SerializeField] private float delay = 1f;

    private bool reiniciando = false;

    private void Start()
    {
        mensagem.text = "";
    }

    void Update()
    {
        if (reiniciando)
            return;

        tempo -= Time.deltaTime;

        textoTimer.text = "Tempo: " + Mathf.Ceil(tempo);

        if (pistasEncontradas >= totalPistas)
        {
            mensagem.text = "Você conseguiu encontrar todas as pistas!";
            enabled = false;
            return;
        }

        if (tempo <= 0)
        {
            tempo = 0;

            textoTimer.text = "Tempo: 0";

            
            mensagem.text = "Você não conseguiu encontrar todas as pistas!";

            reiniciando = true;

            StartCoroutine(ReiniciarComFade());
        }
    }

    public void EncontrouPista()
    {
        pistasEncontradas++;
    }

    private IEnumerator ReiniciarComFade()
    {
        if (transition != null)
        {
            transition.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "TimerPistas: Transition não foi colocado no Inspector!"
            );
        }

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}