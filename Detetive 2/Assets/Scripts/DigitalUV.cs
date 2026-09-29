using UnityEngine;

public class DigitalUV : MonoBehaviour
{
    private bool encontrada = false;
    private GerenciadorDigitais gerenciador;

    void Start()
    {
        gerenciador = FindFirstObjectByType<GerenciadorDigitais>();
    }

    public void MarcarEncontrada()
    {
        if (encontrada)
            return;

        encontrada = true;

        if (gerenciador != null)
            gerenciador.RegistrarDigital();
    }
}