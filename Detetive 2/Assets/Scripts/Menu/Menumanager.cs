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
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);

     
        if (botoesMenu != null)
            botoesMenu.SetActive(true);
    }

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

    public void AbrirConfiguracoes()
    {
       
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);

  
        if (botoesMenu != null)
            botoesMenu.SetActive(false);
    }

    public void FecharConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        if (botoesMenu != null)
            botoesMenu.SetActive(true);
    }

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