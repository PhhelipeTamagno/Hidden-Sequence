using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hotbar : MonoBehaviour
{
    [System.Serializable]
    public class HotbarSlot
    {
        public string nomeItem = "";
        public Sprite icone;
        public Image imagemSlot;
        public GameObject destaque;

        public bool EstaVazio => string.IsNullOrEmpty(nomeItem);
    }

    [Header("Slots")]
    public HotbarSlot[] slots = new HotbarSlot[3];

    [Header("Item Inicial")]
    public string nomeLanterna = "LanternaUV";
    public Sprite iconeLanterna;

    [Header("Configuração")]
    public int slotSelecionado = 0;

    [Header("Animação de Entrada")]
    public RectTransform hotbarRect;
    public CanvasGroup hotbarCanvasGroup;
    public float duracaoAnimacao = 0.5f;
    public float distanciaSubida = 150f;
    public bool animarNoInicio = true;

    private Vector2 posicaoFinalHotbar;

    void Start()
    {
        // ==========================
        // COLOCA A LANTERNA NO SLOT 1
        // ==========================

        if (slots.Length > 0)
        {
            DefinirItem(0, nomeLanterna, iconeLanterna);
        }

        // ==========================
        // DEIXA OS OUTROS SLOTS VAZIOS
        // ==========================

        if (slots.Length > 1)
            LimparSlot(1);

        if (slots.Length > 2)
            LimparSlot(2);

        // ==========================
        // ANIMAÇÃO DA HOTBAR
        // ==========================

        if (hotbarRect != null)
        {
            posicaoFinalHotbar = hotbarRect.anchoredPosition;

            if (animarNoInicio)
            {
                hotbarRect.gameObject.SetActive(false);
                StartCoroutine(MostrarHotbar());
            }
        }

        AtualizarVisual();
    }

    void Update()
    {
        // ==========================
        // TECLAS 1, 2 E 3
        // ==========================

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelecionarSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelecionarSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelecionarSlot(2);
        }

        // ==========================
        // SCROLL DO MOUSE
        // ==========================

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            SelecionarSlot(
                (slotSelecionado + 1) % slots.Length
            );
        }
        else if (scroll < 0f)
        {
            SelecionarSlot(
                (slotSelecionado - 1 + slots.Length) % slots.Length
            );
        }
    }

    // ==========================
    // SELECIONAR SLOT
    // ==========================

    public void SelecionarSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        slotSelecionado = index;

        AtualizarVisual();
    }

    // ==========================
    // ATUALIZAR DESTAQUE
    // ==========================

    void AtualizarVisual()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].destaque != null)
            {
                slots[i].destaque.SetActive(
                    i == slotSelecionado
                );
            }
        }
    }

    // ==========================
    // MOSTRAR HOTBAR
    // ==========================

    public void MostrarHotbarExterno()
    {
        StartCoroutine(MostrarHotbar());
    }

    IEnumerator MostrarHotbar()
    {
        if (hotbarRect == null)
            yield break;

        hotbarRect.gameObject.SetActive(true);

        Vector2 posicaoInicial =
            posicaoFinalHotbar +
            Vector2.down * distanciaSubida;

        hotbarRect.anchoredPosition = posicaoInicial;

        if (hotbarCanvasGroup != null)
            hotbarCanvasGroup.alpha = 0f;

        float tempo = 0f;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(
                tempo / duracaoAnimacao
            );

            float tSuave =
                1f - Mathf.Pow(1f - t, 3f);

            hotbarRect.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicial,
                    posicaoFinalHotbar,
                    tSuave
                );

            if (hotbarCanvasGroup != null)
            {
                hotbarCanvasGroup.alpha = tSuave;
            }

            yield return null;
        }

        hotbarRect.anchoredPosition =
            posicaoFinalHotbar;

        if (hotbarCanvasGroup != null)
            hotbarCanvasGroup.alpha = 1f;
    }

    // ==========================
    // ESCONDER HOTBAR
    // ==========================

    public void EsconderHotbarExterno()
    {
        StartCoroutine(EsconderHotbar());
    }

    IEnumerator EsconderHotbar()
    {
        if (hotbarRect == null)
            yield break;

        Vector2 posicaoInicial =
            hotbarRect.anchoredPosition;

        Vector2 posicaoSaida =
            posicaoFinalHotbar +
            Vector2.down * distanciaSubida;

        float tempo = 0f;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(
                tempo / duracaoAnimacao
            );

            float tSuave = t * t;

            hotbarRect.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicial,
                    posicaoSaida,
                    tSuave
                );

            if (hotbarCanvasGroup != null)
            {
                hotbarCanvasGroup.alpha =
                    1f - tSuave;
            }

            yield return null;
        }

        hotbarRect.gameObject.SetActive(false);
    }

    // ==========================
    // ADICIONAR ITEM
    // ==========================

    public bool AdicionarItem(
        string nome,
        Sprite icone)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].EstaVazio)
            {
                DefinirItem(
                    i,
                    nome,
                    icone
                );

                return true;
            }
        }

        return false;
    }

    // ==========================
    // DEFINIR ITEM
    // ==========================

    void DefinirItem(
        int index,
        string nome,
        Sprite icone)
    {
        if (index < 0 ||
            index >= slots.Length)
            return;

        slots[index].nomeItem = nome;
        slots[index].icone = icone;

        if (slots[index].imagemSlot != null)
        {
            slots[index].imagemSlot.sprite = icone;

            slots[index].imagemSlot.enabled =
                icone != null;
        }
    }

    // ==========================
    // LIMPAR SLOT
    // ==========================

    public void LimparSlot(int index)
    {
        if (index < 0 ||
            index >= slots.Length)
            return;

        DefinirItem(
            index,
            "",
            null
        );
    }

    // ==========================
    // PEGAR ITEM SELECIONADO
    // ==========================

    public string ObterItemSelecionado()
    {
        if (slotSelecionado < 0 ||
            slotSelecionado >= slots.Length)
            return "";

        return slots[slotSelecionado].nomeItem;
    }

    // ==========================
    // VERIFICAR SE ESTÁ CHEIA
    // ==========================

    public bool HotbarCheia()
    {
        foreach (var slot in slots)
        {
            if (slot.EstaVazio)
                return false;
        }

        return true;
    }
}