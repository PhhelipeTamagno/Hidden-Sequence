using UnityEngine;
using TMPro;

public class GerenciadorDigitais : MonoBehaviour
{
    [Header("As 3 Digitais")]
    public DigitalUV[] digitais;

    [Header("Texto do Contador")]
    public TMP_Text textoContador;

    [Header("Sistema de Itens")]
    public ItensEncontrados itensEncontrados;

    [Header("Configuração")]
    public int totalDigitais = 3;

    private int digitaisEncontradas = 0;

    private bool digitalLiberada = false;

    private void Start()
    {
        digitaisEncontradas = 0;

        AtualizarTexto();
    }

    public void RegistrarDigital()
    {
        digitaisEncontradas++;

        if (digitaisEncontradas > totalDigitais)
        {
            digitaisEncontradas = totalDigitais;
        }

        AtualizarTexto();

        Debug.Log(
            "Digitais encontradas: " +
            digitaisEncontradas +
            "/" +
            totalDigitais
        );

        if (digitaisEncontradas >= totalDigitais)
        {
            LiberarDigital();
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

    void LiberarDigital()
    {
        if (digitalLiberada)
            return;

        digitalLiberada = true;

        Debug.Log("Todas as 3 digitais foram encontradas!");

        if (itensEncontrados != null)
        {
            itensEncontrados.EncontrarDigital();
        }
        else
        {
            Debug.LogWarning(
                "O campo 'Itens Encontrados' não foi preenchido!"
            );
        }
    }
}