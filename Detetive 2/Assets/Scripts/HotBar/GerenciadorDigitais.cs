using UnityEngine;
using UnityEngine.UI;

public class GerenciadorDigitais : MonoBehaviour
{
    [Header("Hotbar")]
    public Hotbar hotbar;

    [Header("Item das Digitais")]
    public string nomeItem = "Digitais";
    public Sprite iconeDigitais;

    [Header("Texto das Digitais")]
    public Text textoContador;

    [Header("Configuração")]
    public int totalDigitais = 3;

    private int digitaisEncontradas = 0;
    private bool itemAdicionado = false;

    private void Start()
    {
        AtualizarTexto();
    }

    public void RegistrarDigital()
    {
        
        digitaisEncontradas++;

     
        digitaisEncontradas = Mathf.Clamp(
            digitaisEncontradas,
            0,
            totalDigitais
        );

        AtualizarTexto();

        Debug.Log(
            "Digitais encontradas: " +
            digitaisEncontradas +
            "/" +
            totalDigitais
        );

       
        if (digitaisEncontradas >= totalDigitais && !itemAdicionado)
        {
            itemAdicionado = true;

            if (hotbar != null)
            {
                bool adicionou = hotbar.AdicionarItem(
                    nomeItem,
                    iconeDigitais
                );

                if (adicionou)
                {
                    Debug.Log("Digitais liberadas na Hotbar!");
                }
                else
                {
                    Debug.LogWarning(
                        "Não foi possível adicionar as digitais. A Hotbar está cheia."
                    );
                }
            }
        }
    }

    void AtualizarTexto()
    {
        if (textoContador != null)
        {
            textoContador.text =
                digitaisEncontradas +
                "/" +
                totalDigitais;
        }
    }
}