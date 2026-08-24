using UnityEngine;
using UnityEngine.UI;

public class ItensEncontrados : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string nome;

        [Header("Imagem")]
        public Image imagem;

        [Header("Cor do item")]
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

    public void EncontrarGarrafa()
    {
        garrafa.encontrado = true;
        AtualizarItem(garrafa);
    }

    public void EncontrarFaca()
    {
        faca.encontrado = true;
        AtualizarItem(faca);
    }

    public void EncontrarDigital()
    {
        digital.encontrado = true;
        AtualizarItem(digital);
    }

    void AtualizarItem(Item item)
    {
        if (item == null || item.imagem == null)
            return;

        if (item.encontrado)
        {
            item.imagem.color = item.corEncontrado;
        }
        else
        {
            item.imagem.color = item.corNaoEncontrado;
        }
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