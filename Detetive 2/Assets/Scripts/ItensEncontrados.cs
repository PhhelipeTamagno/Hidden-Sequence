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

    void Start()
    {
        // Começa tudo preto
        AtualizarItem(garrafa);
        AtualizarItem(faca);
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

    void AtualizarItem(Item item)
    {
        if (item.imagem == null)
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
}