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
    public HotbarSlot[] slots = new HotbarSlot[4];

    [Header("Item Inicial - Lanterna")]
    public string nomeLanterna = "LanternaUV";
    public Sprite iconeLanterna;

    [Header("Lanterna (Modelo 3D)")]
    public GameObject lanternaNaMao;

    [Header("Configuração")]
    public int slotSelecionado = 0;

    private bool lanternaNaMaoAtiva = false;

    [Header("Animação de Entrada")]
    public RectTransform hotbarRect;
    public CanvasGroup hotbarCanvasGroup;
    public float duracaoAnimacao = 0.5f;
    public float distanciaSubida = 150f;
    public bool animarNoInicio = true;

    private Vector2 posicaoFinalHotbar;

    void Start()
    {
        if (slots.Length > 0)
            DefinirItem(0, nomeLanterna, iconeLanterna);

        if (slots.Length > 1)
            LimparSlotSemAtualizar(1);

        if (slots.Length > 2)
            LimparSlotSemAtualizar(2);

        if (slots.Length > 3)
            LimparSlotSemAtualizar(3);

        AtivarLanterna(false);

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AlternarLanterna();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelecionarSlot(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelecionarSlot(3);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            SelecionarProximoItem(1);
        }
        else if (scroll < 0f)
        {
            SelecionarProximoItem(-1);
        }
    }

    void AlternarLanterna()
    {
        if (slots.Length == 0 || slots[0].EstaVazio)
            return;

        AtivarLanterna(!lanternaNaMaoAtiva);

        slotSelecionado = 0;

        AtualizarVisual();
    }

    void AtivarLanterna(bool ativo)
    {
        lanternaNaMaoAtiva = ativo;

        if (lanternaNaMao != null)
            lanternaNaMao.SetActive(ativo);
    }

    public void SelecionarSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        if (slots[index].EstaVazio)
            return;

        // Se selecionar outro slot, desliga a lanterna
        if (index != 0 && lanternaNaMaoAtiva)
            AtivarLanterna(false);

        slotSelecionado = index;

        AtualizarVisual();
    }

    void SelecionarProximoItem(int direcao)
    {
        if (slots.Length == 0)
            return;

        int tentativa = slotSelecionado;

        for (int i = 0; i < slots.Length; i++)
        {
            tentativa += direcao;

            if (tentativa >= slots.Length)
                tentativa = 0;

            if (tentativa < 0)
                tentativa = slots.Length - 1;

            if (!slots[tentativa].EstaVazio)
            {
                SelecionarSlot(tentativa);
                return;
            }
        }
    }

    void AtualizarVisual()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].destaque == null)
                continue;

            bool mostrar = false;

            if (i == 0)
            {
                mostrar = lanternaNaMaoAtiva;
            }
            else
            {
                mostrar = i == slotSelecionado && !slots[i].EstaVazio;
            }

            slots[i].destaque.SetActive(mostrar);
        }
    }

    public bool AdicionarItem(string nome, Sprite icone)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].EstaVazio)
            {
                DefinirItem(i, nome, icone);

                slotSelecionado = i;

                AtualizarVisual();

                return true;
            }
        }

        return false;
    }

    void DefinirItem(int index, string nome, Sprite icone)
    {
        if (index < 0 || index >= slots.Length)
            return;

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

        if (index == 0)
            AtivarLanterna(false);

        AtualizarVisual();
    }

    void LimparSlotSemAtualizar(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        DefinirItem(index, "", null);
    }

    public string ObterItemSelecionado()
    {
        if (slotSelecionado < 0 || slotSelecionado >= slots.Length)
            return "";

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
            posicaoFinalHotbar + Vector2.down * distanciaSubida;

        hotbarRect.anchoredPosition = posicaoInicial;

        if (hotbarCanvasGroup != null)
            hotbarCanvasGroup.alpha = 0f;

        float tempo = 0f;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.deltaTime;

            float t = Mathf.Clamp01(tempo / duracaoAnimacao);

            float tSuave =
                1f - Mathf.Pow(1f - t, 3f);

            hotbarRect.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicial,
                    posicaoFinalHotbar,
                    tSuave
                );

            if (hotbarCanvasGroup != null)
                hotbarCanvasGroup.alpha = tSuave;

            yield return null;
        }

        hotbarRect.anchoredPosition = posicaoFinalHotbar;

        if (hotbarCanvasGroup != null)
            hotbarCanvasGroup.alpha = 1f;
    }

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

            float t =
                Mathf.Clamp01(tempo / duracaoAnimacao);

            float tSuave = t * t;

            hotbarRect.anchoredPosition =
                Vector2.Lerp(
                    posicaoInicial,
                    posicaoSaida,
                    tSuave
                );

            if (hotbarCanvasGroup != null)
                hotbarCanvasGroup.alpha =
                    1f - tSuave;

            yield return null;
        }

        hotbarRect.gameObject.SetActive(false);
    }
}