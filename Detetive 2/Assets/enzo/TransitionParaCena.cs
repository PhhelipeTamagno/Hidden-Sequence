
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class TransitionParaCena : MonoBehaviour
{
    [Header("Cena de destino")]
    public string nomeDaCena;

    [Header("Controle")]
    public float atrasoAposTelaPreta = 0f;

    private bool iniciado;

    public void Iniciar()
    {
        if (iniciado)
            return;

        iniciado = true;
        StartCoroutine(AguardarTransicao());
    }

    private IEnumerator AguardarTransicao()
    {
        Transition transition = FindObjectOfType<Transition>();

        if (transition == null)
        {
            Debug.LogError("Nenhuma Transition encontrada na cena.");
            yield break;
        }

        while (transition != null)
        {
            yield return null;
            transition = FindObjectOfType<Transition>();
        }

        if (atrasoAposTelaPreta > 0)
            yield return new WaitForSeconds(atrasoAposTelaPreta);

        if (!string.IsNullOrEmpty(nomeDaCena))
            SceneManager.LoadScene(nomeDaCena);
        else
            Debug.LogError("Nome da cena não foi definido.");
    }
}