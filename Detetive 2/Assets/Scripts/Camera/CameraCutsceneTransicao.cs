using System.Collections;
using UnityEngine;

public class CameraCutsceneTransicao : MonoBehaviour
{
    [Header("Transicao")]
    [Tooltip("Arraste o TransitionTemplate que esta na Hierarchy")]
    public GameObject transitionTemplate;

    [Tooltip("Tempo em segundos ate ativar a transicao")]
    public float tempoAteTransicao = 8.65f;

    [Tooltip("Desativa o TransitionTemplate ao iniciar a cena")]
    public bool desativarNoInicio = true;

    private void Start()
    {
        if (transitionTemplate == null)
        {
            Debug.LogWarning("CameraCutsceneTransicao: arraste o TransitionTemplate no Inspector.");
            return;
        }

        // Garante que o TransitionTemplate comece desativado
        if (desativarNoInicio)
            transitionTemplate.SetActive(false);

        StartCoroutine(AtivarTransicaoAposTempo());
    }

    IEnumerator AtivarTransicaoAposTempo()
    {
        yield return new WaitForSeconds(tempoAteTransicao);

        transitionTemplate.SetActive(true);
    }
}