using UnityEngine;
using UnityEngine.EventSystems;

public class AbasComputador : MonoBehaviour
{
    [Header("Janelas")]
    public GameObject email;
    public GameObject navegador;
    public GameObject arquivos;
    public GameObject lixeira;

    private RectTransform janelaAtual;
    private Canvas canvas;
    private Vector2 offset;
    private bool arrastando;

    void Start()
    {
      

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();
    }

    #region Abrir

    public void AbrirEmail()
    {
        email.SetActive(true);
        email.SetActive(true);
    }

    public void AbrirNavegador()
    {
        navegador.SetActive(true);
        navegador.SetActive(true);
    }

    public void AbrirArquivos()
    {
        arquivos.SetActive(true);
        arquivos.SetActive(true);
    }

    public void AbrirLixeira()
    {
        lixeira.SetActive(true);
        lixeira.SetActive(true);
    }

    #endregion

    #region Fechar

    public void FecharEmail()
    {
        email.SetActive(false);
    }

    public void FecharNavegador()
    {
        navegador.SetActive(false);
    }

    public void FecharArquivos()
    {
        arquivos.SetActive(false);
    }

    public void FecharLixeira()
    {
        lixeira.SetActive(false);
    }

    public void FecharTodas()
    {
        if (email) email.SetActive(false);
        if (navegador) navegador.SetActive(false);
        if (arquivos) arquivos.SetActive(false);
        if (lixeira) lixeira.SetActive(false);
    }

    #endregion

    // ==========================
    // ARRASTAR JANELAS
    // ==========================

    public void ComecarArrastar(RectTransform janela)
    {
        janelaAtual = janela;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            janela,
            Input.mousePosition,
            null,
            out offset);

        janela.SetAsLastSibling();

        arrastando = true;
    }

    public void PararArrastar()
    {
        arrastando = false;
    }

    void Update()
    {
        if (!arrastando || janelaAtual == null)
            return;

        Vector2 pos;

        RectTransform canvasRect = canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out pos))
        {
            janelaAtual.localPosition = pos - offset;
        }

        if (Input.GetMouseButtonUp(0))
            PararArrastar();
    }
}