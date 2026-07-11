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
        public Image imagemSlot;      // Image que mostra o ícone do item
        public GameObject destaque;   // Borda/GameObject que aparece quando o slot está selecionado

        public bool EstaVazio => string.IsNullOrEmpty(nomeItem);
    }

    [Header("Slots")]
    public HotbarSlot[] slots = new HotbarSlot[3];

    [Header("Configuração")]
    public int slotSelecionado = 0;

    [Header("Animação de Entrada")]
    public RectTransform hotbarRect;   // RectTransform do container da hotbar (painel com os 3 slots)
    public CanvasGroup hotbarCanvasGroup; // opcional, pra fade junto com o movimento
    public float duracaoAnimacao = 0.5f;
    public float distanciaSubida = 150f; // quanto abaixo da posição final ela começa
    public bool animarNoInicio = true;

    private Vector2 posicaoFinalHotbar;

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelecionarSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelecionarSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelecionarSlot(2);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SelecionarSlot((slotSelecionado + 1) % slots.Length);
        }
        else if (scroll < 0f)
        {
            SelecionarSlot((slotSelecionado - 1 + slots.Length) % slots.Length);
        }
    }

    public void SelecionarSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        slotSelecionado = index;
        AtualizarVisual();
    }

    void AtualizarVisual()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].destaque != null)
                slots[i].destaque.SetActive(i == slotSelecionado);
        }
    }

    // Chame esse método publicamente pra mostrar a hotbar (ex: início de fase, após coletar 1º item, etc)
    public void MostrarHotbarExterno()
    {
        StartCoroutine(MostrarHotbar());
    }

    // Chame esse método pra esconder a hotbar (ex: durante cutscenes, menus, etc)
    public void EsconderHotbarExterno()
    {
        StartCoroutine(EsconderHotbar());
    }

    IEnumerator MostrarHotbar()
    {
        hotbarRect.gameObject.SetActive(true);

        Vector2 posicaoInicial = posicaoFinalHotbar + Vector2.down * distanciaSubida;
        hotbarRect.anchoredPosition = posicaoInicial;

        if (hotbarCanvasGroup != null)
            hotbarCanvasGroup.alpha = 0f;

        float tempo = 0f;
        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoAnimacao);
            float tSuave = 1f - Mathf.Pow(1f - t, 3f); // ease out cubic

            hotbarRect.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoFinalHotbar, tSuave);

            if (hotbarCanvasGroup != null)
                hotbarCanvasGroup.alpha = tSuave;

            yield return null;
        }

        hotbarRect.anchoredPosition = posicaoFinalHotbar;

        if (hotbarCanvasGroup != null)
            hotbarCanvasGroup.alpha = 1f;
    }

    IEnumerator EsconderHotbar()
    {
        Vector2 posicaoInicial = hotbarRect.anchoredPosition;
        Vector2 posicaoSaida = posicaoFinalHotbar + Vector2.down * distanciaSubida;

        float tempo = 0f;
        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;
            float t = Mathf.Clamp01(tempo / duracaoAnimacao);
            float tSuave = t * t; // ease in, desce acelerando

            hotbarRect.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoSaida, tSuave);

            if (hotbarCanvasGroup != null)
                hotbarCanvasGroup.alpha = 1f - tSuave;

            yield return null;
        }

        hotbarRect.gameObject.SetActive(false);
    }

    // Tenta adicionar o item no primeiro slot vazio. Retorna true se conseguiu.
    public bool AdicionarItem(string nome, Sprite icone)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].EstaVazio)
            {
                DefinirItem(i, nome, icone);
                return true;
            }
        }

        return false;
    }

    void DefinirItem(int index, string nome, Sprite icone)
    {
        slots[index].nomeItem = nome;
        slots[index].icone = icone;

        if (slots[index].imagemSlot != null)
        {
            slots[index].imagemSlot.sprite = icone;
            slots[index].imagemSlot.enabled = icone != null;
        }
    }

    public void LimparSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        DefinirItem(index, "", null);
    }

    public string ObterItemSelecionado()
    {
        return slots[slotSelecionado].nomeItem;
    }

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