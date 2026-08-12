using UnityEngine;
using TMPro;

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

    [Header("Post-it de aviso")]
    public GameObject postIt;
    public TMP_Text textoPostIt;

    [TextArea(2, 4)]
    public string mensagemPostIt =
        "Ei fecha isso daí, você tem que trabalhar agora!";

    private RectTransform janelaAtual;
    private Canvas canvas;
    private Vector2 offset;
    private bool arrastando;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();

        if (imagemLixeiraVazia != null)
            imagemLixeiraVazia.SetActive(false);

        if (textoPostIt == null && postIt != null)
        {
            textoPostIt =
                postIt.GetComponentInChildren<TMP_Text>(true);
        }


        if (textoPostIt != null)
        {
            textoPostIt.text = mensagemPostIt;
        }
        else if (postIt != null)
        {
            Debug.LogError(
                "Não foi encontrado nenhum TMP_Text dentro do Post-it!"
            );
        }

        if (postIt != null)
            postIt.SetActive(false);
    }

    public void MostrarPostIt()
    {
        Debug.Log("MostrarPostIt chamado.");

        if (postIt == null)
        {
            Debug.LogError(
                "POST-IT NÃO FOI ATRIBUÍDO NO INSPECTOR!"
            );

            return;
        }

        if (textoPostIt == null)
        {
            textoPostIt =
                postIt.GetComponentInChildren<TMP_Text>(true);
        }

        if (textoPostIt != null)
        {
            textoPostIt.text = mensagemPostIt;

            Debug.Log(
                "Texto do Post-it: " + mensagemPostIt
            );
        }
        else
        {
            Debug.LogError(
                "O Post-it não possui nenhum TMP_Text!"
            );
        }

        postIt.SetActive(true);

        RectTransform rt =
            postIt.GetComponent<RectTransform>();

        if (rt != null)
            rt.SetAsLastSibling();

        Debug.Log("POST-IT ABERTO!");
    }

    public void EsconderPostIt()
    {
        if (postIt != null)
            postIt.SetActive(false);
    }

    public void AbrirEmail()
    {
        if (email != null)
            email.SetActive(true);
    }

    public void AbrirNavegador()
    {
        if (navegador == null)
            return;

        navegador.SetActive(true);

        if (postIt != null)
        {
            postIt.transform.SetParent(
                navegador.transform,
                false
            );

            postIt.transform.SetAsLastSibling();
        }

        MostrarPostIt();

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
        if (email != null)
            email.SetActive(false);

        if (navegador != null)
            navegador.SetActive(false);

        if (arquivos != null)
            arquivos.SetActive(false);

        if (lixeira != null)
            lixeira.SetActive(false);
    }
    public void ComecarArrastar(RectTransform janela)
    {
        if (janela == null)
            return;

        janelaAtual = janela;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            janela,
            Input.mousePosition,
            null,
            out offset
        );

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

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null,
            out pos))
        {
            Vector2 novaPos = pos - offset;

            if (limiteComputador != null)
            {
                float limiteX =
                    limiteComputador.rect.width / 2;

                float limiteY =
                    limiteComputador.rect.height / 2;

                float metadeJanelaX =
                    janelaAtual.rect.width / 2;

                float metadeJanelaY =
                    janelaAtual.rect.height / 2;

                novaPos.x = Mathf.Clamp(
                    novaPos.x,
                    -limiteX + metadeJanelaX,
                    limiteX - metadeJanelaX
                );

                novaPos.y = Mathf.Clamp(
                    novaPos.y,
                    -limiteY + metadeJanelaY,
                    limiteY - metadeJanelaY
                );
            }

            janelaAtual.localPosition = novaPos;
        }

        if (Input.GetMouseButtonUp(0))
            PararArrastar();
    }

    public void EsvaziarLixeira()
    {
        if (imagemLixeiraVazia != null)
            imagemLixeiraVazia.SetActive(true);
    }

    public void RestaurarLixeira()
    {
        if (imagemLixeiraVazia != null)
            imagemLixeiraVazia.SetActive(false);
    }
}