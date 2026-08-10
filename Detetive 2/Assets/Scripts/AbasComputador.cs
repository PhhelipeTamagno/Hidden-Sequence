using UnityEngine;
using UnityEngine.UI;

public class AbasComputador : MonoBehaviour
{
    [Header("Janelas")]
    public GameObject email;
    public GameObject navegador;
    public GameObject arquivos;
    public GameObject lixeira;

    [Header("Área do Computador")]
    public RectTransform limiteComputador;

    [Header("Lixeira")]
    public GameObject imagemLixeiraVazia;

    private RectTransform janelaAtual;
    private Canvas canvas;
    private Vector2 offset;
    private bool arrastando;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();

        // Garante que a imagem da lixeira vazia comece escondida
        if (imagemLixeiraVazia != null)
            imagemLixeiraVazia.SetActive(false);
    }

    #region Abrir

    public void AbrirEmail()
    {
        if (email != null)
            email.SetActive(true);
    }

    public void AbrirNavegador()
    {
        if (navegador != null)
            navegador.SetActive(true);
    }

    public void AbrirArquivos()
    {
        if (arquivos != null)
            arquivos.SetActive(true);
    }

    public void AbrirLixeira()
    {
        if (lixeira != null)
            lixeira.SetActive(true);
    }

    #endregion

    #region Fechar

    public void FecharEmail()
    {
        if (email != null)
            email.SetActive(false);
    }

    public void FecharNavegador()
    {
        if (navegador != null)
            navegador.SetActive(false);
    }

    public void FecharArquivos()
    {
        if (arquivos != null)
            arquivos.SetActive(false);
    }

    public void FecharLixeira()
    {
        if (lixeira != null)
            lixeira.SetActive(false);
    }

    public void FecharTodas()
    {
        if (email != null) email.SetActive(false);
        if (navegador != null) navegador.SetActive(false);
        if (arquivos != null) arquivos.SetActive(false);
        if (lixeira != null) lixeira.SetActive(false);
    }

    #endregion

    //=========================================
    // ARRASTAR JANELAS
    //=========================================

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
            Vector2 novaPos = pos - offset;

            // Limita a janela à área do computador
            if (limiteComputador != null)
            {
                float limiteX = limiteComputador.rect.width / 2;
                float limiteY = limiteComputador.rect.height / 2;

                float metadeJanelaX = janelaAtual.rect.width / 2;
                float metadeJanelaY = janelaAtual.rect.height / 2;

                novaPos.x = Mathf.Clamp(
                    novaPos.x,
                    -limiteX + metadeJanelaX,
                    limiteX - metadeJanelaX);

                novaPos.y = Mathf.Clamp(
                    novaPos.y,
                    -limiteY + metadeJanelaY,
                    limiteY - metadeJanelaY);
            }

            janelaAtual.localPosition = novaPos;
        }

        if (Input.GetMouseButtonUp(0))
            PararArrastar();
    }

    //=========================================
    // LIXEIRA
    //=========================================

    // Chame este método no botão "Esvaziar Lixeira"
    public void EsvaziarLixeira()
    {
        if (imagemLixeiraVazia != null)
            imagemLixeiraVazia.SetActive(true);
    }

    // Caso queira esconder novamente
    public void RestaurarLixeira()
    {
        if (imagemLixeiraVazia != null)
            imagemLixeiraVazia.SetActive(false);
    }
}