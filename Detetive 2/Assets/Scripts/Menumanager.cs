using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paineis do Menu")]
    [SerializeField] private GameObject painelConfiguracoes;
    [SerializeField] private GameObject painelCreditos;

    [Header("Configuracoes de Cena")]
    [Tooltip("Nome exato da cena do jogo um, igual está no Build Settings")]
    [SerializeField] private string nomeCenaJogoUm = "JogoUm";

    private void Start()
    {
        // Garante que os paineis comecem fechados ao abrir o menu
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        if (painelCreditos != null)
            painelCreditos.SetActive(false);
    }

    // ---------- JOGO ----------

    // Conecte esse metodo no OnClick do botao "Jogar"
    public void AbrirJogoUm()
    {
        SceneManager.LoadScene(nomeCenaJogoUm);
    }

    // ---------- CONFIGURACOES ----------

    // Conecte no OnClick do botao "Configuracoes"
    public void AbrirConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);
    }

    // Conecte no OnClick do botao "Fechar" dentro do painel de config
    public void FecharConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);
    }

    // ---------- CREDITOS ----------

    // Conecte no OnClick do botao "Creditos"
    public void AbrirCreditos()
    {
        if (painelCreditos != null)
            painelCreditos.SetActive(true);
    }

    // Conecte no OnClick do botao "Fechar" dentro do painel de creditos
    public void FecharCreditos()
    {
        if (painelCreditos != null)
            painelCreditos.SetActive(false);
    }

    // ---------- EXTRA ----------

    // Opcional: conecte num botao "Sair"
    public void SairDoJogo()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}