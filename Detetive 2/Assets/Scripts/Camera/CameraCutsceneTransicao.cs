
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCutsceneTransicao : MonoBehaviour
{
    [Header("Transicao")]
    public GameObject transitionTemplate;
    public float tempoAteTransicao = 8.65f;
    public bool desativarNoInicio = true;

    [Header("Troca de cena")]
    public string nomeDaCena;
    public float tempoDaTransicao = 1f;

    private bool iniciado;

    private void Start()
    {
        if (transitionTemplate == null)
        {
            Debug.LogWarning("Arraste o TransitionTemplate no Inspector.");
            return;
        }

        if (desativarNoInicio)
            transitionTemplate.SetActive(false);

        StartCoroutine(ExecutarSequencia());
    }

    private IEnumerator ExecutarSequencia()
    {
        yield return new WaitForSeconds(tempoAteTransicao);

        if (iniciado)
            yield break;

        iniciado = true;

        transitionTemplate.SetActive(true);

        yield return new WaitForSeconds(tempoDaTransicao);

        if (string.IsNullOrEmpty(nomeDaCena))
        {
            Debug.LogError("Defina o nome da cena no Inspector.");
            yield break;
        }

        SceneManager.LoadScene(nomeDaCena);
    }
}