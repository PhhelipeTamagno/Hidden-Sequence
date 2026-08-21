using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EasyTransition;

public class MenuManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject painelConfiguracoes;
    [SerializeField] private GameObject painelCreditos;

    [Header("Transição")]
    [SerializeField] private Transition transition;
    [SerializeField] private string nomeCenaJogoUm = "JogoUm";
    [SerializeField] private float delay = 1f;

    private void Start()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);
    }

    // ---------- JOGO ----------

    public void AbrirJogoUm()
    {
        StartCoroutine(TransicaoECarregarCena());
    }

    private IEnumerator TransicaoECarregarCena()
    {
        if (transition != null)
            transition.gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(nomeCenaJogoUm);
    }

    // ---------- CONFIGURAÇÕES ----------

    public void AbrirConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);
    }

    // ---------- CRÉDITOS ----------

    public void AbrirCreditos()
    {
        if (painelCreditos != null)
            painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        if (painelCreditos != null)
            painelCreditos.SetActive(false);
    }

    // ---------- SAIR ----------

    public void SairDoJogo()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}