using UnityEngine;
using UnityEngine.UI;

public class DigitalUv1 : MonoBehaviour
{
    [Header("Referências")]
    public Light luzUV;
    public Image imagemDigital;
    public ItensEncontrados itensEncontrados;

    [Header("Configuração")]
    public float velocidadeAparecer = 1.5f;
    public float velocidadeDesaparecer = 1f;

    [Range(0f, 1f)]
    public float opacidadeMaxima = 1f;

    [Header("Distância para revelar")]
    public float distanciaMaxima = 250f;

    private float opacidadeAtual = 0f;

    void Start()
    {
        if (imagemDigital == null)
            imagemDigital = GetComponent<Image>();

        // Digital de evidência começa invisível
        Color cor = imagemDigital.color;
        cor.a = 0f;
        imagemDigital.color = cor;
    }

    void Update()
    {
        if (luzUV == null || imagemDigital == null)
            return;

        // Se a lanterna estiver desligada,
        // a digital NÃO pode ser iluminada.
        if (!luzUV.enabled)
        {
            DesligarDigital();
            return;
        }

        Vector3 posicaoTela = Camera.main.WorldToScreenPoint(
            transform.position
        );

        Vector3 posicaoLuz = Camera.main.WorldToScreenPoint(
            luzUV.transform.position
        );

        float distancia = Vector2.Distance(
            posicaoTela,
            posicaoLuz
        );

        bool iluminada = distancia < distanciaMaxima;

        if (iluminada)
        {
            opacidadeAtual = Mathf.MoveTowards(
                opacidadeAtual,
                opacidadeMaxima,
                velocidadeAparecer * Time.deltaTime
            );

            // Digital da hotbar fica branca
            if (itensEncontrados != null)
                itensEncontrados.DigitalMirada();
        }
        else
        {
            DesligarDigital();

            // Digital da hotbar volta para preta
            if (itensEncontrados != null)
                itensEncontrados.DigitalNaoMirada();
        }

        Color cor = imagemDigital.color;
        cor.a = opacidadeAtual;
        imagemDigital.color = cor;
    }

    void DesligarDigital()
    {
        opacidadeAtual = Mathf.MoveTowards(
            opacidadeAtual,
            0f,
            velocidadeDesaparecer * Time.deltaTime
        );

        Color cor = imagemDigital.color;
        cor.a = opacidadeAtual;
        imagemDigital.color = cor;

        if (itensEncontrados != null)
            itensEncontrados.DigitalNaoMirada();
    }
}