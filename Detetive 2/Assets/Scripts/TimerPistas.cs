using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimerPistas : MonoBehaviour
{
    public float tempo = 60f;

    public int totalPistas = 5;
    public int pistasEncontradas = 0;

    public TMP_Text textoTimer;
    public TMP_Text mensagem;

    void Update()
    {
        tempo -= Time.deltaTime;

        textoTimer.text = "Tempo: " + Mathf.Ceil(tempo);

        if (pistasEncontradas >= totalPistas)
        {
            mensagem.text = "Você conseguiu encontrar todas as pistas!";
            enabled = false;
        }

        if (tempo <= 0 && pistasEncontradas < totalPistas)
        {
            mensagem.text = "Você não conseguiu encontrar todas as pistas!";

            Invoke("ReiniciarCena", 2f);

            enabled = false;
        }
    }

    public void EncontrouPista()
    {
        pistasEncontradas++;
    }

    void ReiniciarCena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}