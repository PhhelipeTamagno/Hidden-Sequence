using UnityEngine;
using UnityEngine.UI;

public class DigitalUv1 : MonoBehaviour
{
    [Header("Referências")]
    public Light luzUV;
    public Image imagemDigital;

    [Header("Configuração")]
    public float velocidadeAparecer = 1.5f;
    public float velocidadeDesaparecer = 1f;

    [Range(0f, 1f)]
    public float opacidadeMaxima = 1f;

    private float opacidadeAtual = 0f;

    void Start()
    {
        if (imagemDigital == null)
            imagemDigital = GetComponent<Image>();

        // Começa invisível
        Color cor = imagemDigital.color;
        cor.a = 0f;
        imagemDigital.color = cor;
    }

    void Update()
    {
        if (luzUV == null || imagemDigital == null)
            return;

        // Posição da digital na tela
        Vector3 posicaoTela = Camera.main.WorldToScreenPoint(
            transform.position
        );

        // Posição da luz na tela
        Vector3 posicaoLuz = Camera.main.WorldToScreenPoint(
            luzUV.transform.position
        );

        // Distância entre a digital e a luz na tela
        float distancia = Vector2.Distance(
            posicaoTela,
            posicaoLuz
        );

        bool iluminada = distancia < 250f;

        if (iluminada)
        {
            opacidadeAtual = Mathf.MoveTowards(
                opacidadeAtual,
                opacidadeMaxima,
                velocidadeAparecer * Time.deltaTime
            );
        }
        else
        {
            opacidadeAtual = Mathf.MoveTowards(
                opacidadeAtual,
                0f,
                velocidadeDesaparecer * Time.deltaTime
            );
        }

        Color cor = imagemDigital.color;
        cor.a = opacidadeAtual;
        imagemDigital.color = cor;
    }
}