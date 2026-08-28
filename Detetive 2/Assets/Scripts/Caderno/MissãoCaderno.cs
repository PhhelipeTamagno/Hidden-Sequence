using UnityEngine;
using UnityEngine.UI;

public class MissoesCaderno : MonoBehaviour
{
    [Header("Referências")]
    public ItensEncontrados itensEncontrados;

    [Header("UI do Caderno")]
    [Tooltip("Painel que aparece na tela quando o caderno está na mão")]
    public GameObject painelCaderno;
    public Text textoMissoes;

    [Header("Textos das Missões")]
    public string missaoGarrafa = "Encontrar a garrafa";
    public string missaoFaca = "Encontrar a faca";
    public string missaoDigital = "Encontrar a digital";

    [Header("Cor das missões concluídas")]
    public Color corConcluida = new Color(0.35f, 0.85f, 0.35f);

    void Update()
    {
        // Enquanto o caderno estiver aberto, mantém as missões sempre atualizadas
        if (painelCaderno != null && painelCaderno.activeSelf)
        {
            AtualizarTextoMissoes();
        }
    }

    // =========================
    // MOSTRAR / ESCONDER
    // =========================
    public void MostrarCaderno()
    {
        if (painelCaderno != null)
            painelCaderno.SetActive(true);

        AtualizarTextoMissoes();
    }

    public void EsconderCaderno()
    {
        if (painelCaderno != null)
            painelCaderno.SetActive(false);
    }

    // =========================
    // ATUALIZAR TEXTO
    // =========================
    void AtualizarTextoMissoes()
    {
        if (textoMissoes == null || itensEncontrados == null)
            return;

        string linha1 = FormatarMissao(missaoGarrafa, itensEncontrados.GarrafaFoiEncontrada());
        string linha2 = FormatarMissao(missaoFaca, itensEncontrados.FacaFoiEncontrada());
        string linha3 = FormatarMissao(missaoDigital, itensEncontrados.DigitalFoiEncontrada());

        textoMissoes.text = linha1 + "\n" + linha2 + "\n" + linha3;
    }

    string FormatarMissao(string texto, bool concluida)
    {
        string corHex = ColorUtility.ToHtmlStringRGB(corConcluida);

        if (concluida)
            return $"[X] <color=#{corHex}>{texto}</color>";

        return $"[ ] {texto}";
    }
}