using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EasyTransition;

public class MenuManager : MonoBehaviour
{
    [Header("Painéis do Menu")]
    [SerializeField] private GameObject painelConfiguracoes;
    [SerializeField] private GameObject painelCreditos;

    [Header("Botões do Menu")]
    [SerializeField] private GameObject botoesMenu;

    [Header("Transição")]
    [SerializeField] private Transition transition;
    [SerializeField] private string nomeCenaJogoUm = "JogoUm";
    [SerializeField] private float delay = 1f;

    private void Start()
    {
        // Começa com os painéis fechados
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);

        // Botões principais começam ativos
        if (botoesMenu != null)
            botoesMenu.SetActive(true);
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
        // Abre o painel de configurações
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);

        // Desativa os botões do menu principal
        if (botoesMenu != null)
            botoesMenu.SetActive(false);
    }

    public void FecharConfiguracoes()
    {
        // Fecha o painel de configurações
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        // Reativa os botões do menu principal
        if (botoesMenu != null)
            botoesMenu.SetActive(true);
    }

    // ---------- CRÉDITOS ----------

    public void AbrirCreditos()
    {
   
        if (painelCreditos != null)
            painelCreditos.SetActive(true);


        if (botoesMenu != null)
            botoesMenu.SetActive(false);
    }

    public void FecharCreditos()
    {
        if (painelCreditos != null)
            painelCreditos.SetActive(false);

       
        if (botoesMenu != null)
            botoesMenu.SetActive(true);
    }

    

    public void SairDoJogo()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}