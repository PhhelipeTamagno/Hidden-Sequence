using UnityEngine;
using UnityEngine.UI;

public class TransicaoPorta : MonoBehaviour
{
    [Header("Configuração da Porta")]
    public bool vaiParaSauna;

    [Header("Fade")]
    public GameObject fade;

    [Header("Texto")]
    public GameObject textoIrParaSauna;
    public GameObject textoIrParaCasa;

    private bool jogadorDentro = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorDentro = true;

       
        if (vaiParaSauna)
        {
            if (fade != null)
                fade.SetActive(true);

            if (textoIrParaSauna != null)
                textoIrParaSauna.SetActive(true);

            if (textoIrParaCasa != null)
                textoIrParaCasa.SetActive(false);
        }

       
        else
        {
            if (fade != null)
                fade.SetActive(true);

            if (textoIrParaCasa != null)
                textoIrParaCasa.SetActive(true);

            if (textoIrParaSauna != null)
                textoIrParaSauna.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        jogadorDentro = false;

        if (textoIrParaSauna != null)
            textoIrParaSauna.SetActive(false);

        if (textoIrParaCasa != null)
            textoIrParaCasa.SetActive(false);

        if (fade != null)
            fade.SetActive(false);
    }
}