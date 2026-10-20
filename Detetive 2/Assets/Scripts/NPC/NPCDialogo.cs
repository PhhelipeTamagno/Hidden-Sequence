using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCDialogo : MonoBehaviour
{
    [Header("Texto do NPC")]
    public TMP_Text textoDialogo;

    [TextArea(2, 5)]
    public string[] falas;

    [Header("Distância de interação")]
    public float distanciaMaxima = 3f;

    [Header("Posição do texto")]
    public Vector3 deslocamentoTexto = new Vector3(0f, 2f, 0f);

    [Header("Indicador do clique")]
    public Sprite imagemMouseEsquerdo;

    private int indiceFala;
    private bool conversando;
    private Camera cameraPrincipal;
    private GameObject indicadorMouse;

    private void Start()
    {
        cameraPrincipal = Camera.main;

        if (textoDialogo != null)
            textoDialogo.gameObject.SetActive(false);

        CriarIndicadorMouse();
    }

    private void Update()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        if (Mouse.current == null || cameraPrincipal == null)
            return;

        // Atualiza a posição do texto enquanto conversa.
        AtualizarPosicaoTexto();

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Se já estiver conversando, avança para a próxima fala.
        if (conversando)
        {
            ProximaFala();
            return;
        }

        // Detecta o NPC clicado.
        Ray raio = cameraPrincipal.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(raio, out RaycastHit hit))
        {
            NPCDialogo npc = hit.collider.GetComponentInParent<NPCDialogo>();

            if (npc == null)
                return;

            // Calcula a distância entre a câmera e o NPC.
            float distancia = Vector3.Distance(
                cameraPrincipal.transform.position,
                npc.transform.position
            );

            // Só inicia a conversa se estiver perto o suficiente.
            if (distancia <= npc.distanciaMaxima)
            {
                npc.IniciarDialogo();
            }
        }
    }

    private void IniciarDialogo()
    {
        if (textoDialogo == null || falas == null || falas.Length == 0)
            return;

        conversando = true;
        indiceFala = 0;

        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text = falas[indiceFala];

        if (indicadorMouse != null)
            indicadorMouse.SetActive(false);

        AtualizarPosicaoTexto();
    }

    private void ProximaFala()
    {
        indiceFala++;

        if (indiceFala >= falas.Length)
        {
            EncerrarDialogo();
            return;
        }

        textoDialogo.text = falas[indiceFala];
    }

    private void EncerrarDialogo()
    {
        conversando = false;

        if (textoDialogo != null)
        {
            textoDialogo.text = "";
            textoDialogo.gameObject.SetActive(false);
        }
    }

    private void AtualizarPosicaoTexto()
    {
        if (textoDialogo == null || !conversando)
            return;

        // O texto acompanha o NPC no mundo.
        textoDialogo.transform.position =
            transform.position + deslocamentoTexto;
    }

    private void OnMouseEnter()
    {
        if (!conversando && indicadorMouse != null)
        {
            float distancia = cameraPrincipal != null
                ? Vector3.Distance(
                    cameraPrincipal.transform.position,
                    transform.position
                )
                : Mathf.Infinity;

            if (distancia <= distanciaMaxima)
                indicadorMouse.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (indicadorMouse != null)
            indicadorMouse.SetActive(false);
    }

    private void CriarIndicadorMouse()
    {
        if (imagemMouseEsquerdo == null)
            return;

        GameObject canvasObj = new GameObject("CanvasIndicadorMouse");

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        indicadorMouse = new GameObject("IndicadorClique");
        indicadorMouse.transform.SetParent(canvasObj.transform, false);

        Image imagem = indicadorMouse.AddComponent<Image>();
        imagem.sprite = imagemMouseEsquerdo;
        imagem.raycastTarget = false;

        RectTransform rect = indicadorMouse.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(45f, 45f);
        rect.position = Input.mousePosition;

        indicadorMouse.SetActive(false);
    }
}