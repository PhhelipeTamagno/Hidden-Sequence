using UnityEngine;
using UnityEngine.UI;

public class ItensEncontrados : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string nome;

        [Header("Imagem da Hotbar")]
        public Image imagem;

        [Header("Cor quando encontrado")]
        public Color corEncontrado = Color.white;

        [Header("Cor antes de encontrar")]
        public Color corNaoEncontrado = Color.black;

        [HideInInspector]
        public bool encontrado = false;
    }

    [Header("Itens que precisam ser encontrados")]
    public Item garrafa;
    public Item faca;
    public Item digital;

    void Start()
    {
        AtualizarItem(garrafa);
        AtualizarItem(faca);
        AtualizarItem(digital);
    }

    void Update()
    {
        // Garante que as imagens da hotbar nunca desapareçam
        GarantirImagemVisivel(garrafa);
        GarantirImagemVisivel(faca);
        GarantirImagemVisivel(digital);
    }

    void GarantirImagemVisivel(Item item)
    {
        if (item == null || item.imagem == null)
            return;

        item.imagem.enabled = true;

        Color cor = item.imagem.color;
        cor.a = 1f;
        item.imagem.color = cor;
    }

    // =========================
    // GARRAFA
    // =========================

    public void EncontrarGarrafa()
    {
        garrafa.encontrado = true;
        AtualizarItem(garrafa);
    }

    // =========================
    // FACA
    // =========================

    public void EncontrarFaca()
    {
        faca.encontrado = true;
        AtualizarItem(faca);
    }

    // =========================
    // DIGITAL ENCONTRADA
    // =========================

    public void EncontrarDigital()
    {
        digital.encontrado = true;
        AtualizarItem(digital);
    }

    // =========================
    // UV ESTÁ SOBRE A DIGITAL
    // =========================

    public void DigitalMirada()
    {
        if (digital == null || digital.imagem == null)
            return;

        if (!digital.encontrado)
        {
            digital.imagem.enabled = true;

            Color cor = digital.corEncontrado;
            cor.a = 1f;

            digital.imagem.color = cor;
        }
    }

    // =========================
    // UV SAIU DA DIGITAL
    // =========================

    public void DigitalNaoMirada()
    {
        if (digital == null || digital.imagem == null)
            return;

        if (!digital.encontrado)
        {
            digital.imagem.enabled = true;

            Color cor = digital.corNaoEncontrado;
            cor.a = 1f;

            digital.imagem.color = cor;
        }
    }

    // =========================
    // ATUALIZAR ITEM
    // =========================

    void AtualizarItem(Item item)
    {
        if (item == null || item.imagem == null)
            return;

        item.imagem.enabled = true;

        Color cor;

        if (item.encontrado)
            cor = item.corEncontrado;
        else
            cor = item.corNaoEncontrado;

        cor.a = 1f;

        item.imagem.color = cor;
    }

    public bool GarrafaFoiEncontrada()
    {
        return garrafa.encontrado;
    }

    public bool FacaFoiEncontrada()
    {
        return faca.encontrado;
    }

    public bool DigitalFoiEncontrada()
    {
        return digital.encontrado;
    }
}