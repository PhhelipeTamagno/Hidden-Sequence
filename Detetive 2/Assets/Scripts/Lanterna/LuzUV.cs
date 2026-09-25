using UnityEngine;
using UnityEngine.UI;

public class LuzUV : MonoBehaviour
{
    [Header("Referências")]
    public Light spotLight;

    [Header("Imagens")]
    public Image[] imagens;

    [Header("Alpha")]
    public float velocidadeAparecer = 3f;
    public float velocidadeDesaparecer = 2f;

    [Header("Luz Normal")]
    public float alcanceNormal = 10f;
    public float intensidadeNormal = 5f;

    [Header("Luz Perto da Digital")]
    public float distanciaParaAmpliar = 2.5f;
    public float alcancePerto = 15f;
    public float intensidadePerto = 2f;

    private void Start()
    {
        foreach (Image imagem in imagens)
        {
            if (imagem == null)
                continue;

            DefinirAlpha(imagem, 0f);
        }

        if (spotLight != null)
        {
            spotLight.range = alcanceNormal;
            spotLight.intensity = intensidadeNormal;
        }
    }

    private void Update()
    {
        if (spotLight == null)
            return;

        float distanciaMaisProxima =
            EncontrarImagemMaisProxima();

        AtualizarLuz(distanciaMaisProxima);

        foreach (Image imagem in imagens)
        {
            if (imagem == null)
                continue;

            bool estaNoFeixe =
                EstaNoFeixeUV(imagem);

            float alphaAtual =
                imagem.color.a;

            float alphaAlvo =
                estaNoFeixe ? 1f : 0f;

            float velocidade =
                estaNoFeixe
                ? velocidadeAparecer
                : velocidadeDesaparecer;

            float novoAlpha =
                Mathf.MoveTowards(
                    alphaAtual,
                    alphaAlvo,
                    velocidade * Time.deltaTime
                );

            DefinirAlpha(
                imagem,
                novoAlpha
            );
        }
    }

    // =================================
    // DETECTAR DIGITAL MAIS PRÓXIMA
    // =================================

    float EncontrarImagemMaisProxima()
    {
        float menorDistancia = Mathf.Infinity;

        foreach (Image imagem in imagens)
        {
            if (imagem == null)
                continue;

            float distancia =
                Vector3.Distance(
                    transform.position,
                    imagem.transform.position
                );

            if (distancia < menorDistancia)
                menorDistancia = distancia;
        }

        return menorDistancia;
    }

    // =================================
    // ALTERAR LUZ
    // =================================

    void AtualizarLuz(float distancia)
    {
        if (distancia <= distanciaParaAmpliar)
        {
            spotLight.range =
                alcancePerto;

            spotLight.intensity =
                intensidadePerto;
        }
        else
        {
            spotLight.range =
                alcanceNormal;

            spotLight.intensity =
                intensidadeNormal;
        }
    }

    // =================================
    // VERIFICAR FEIXE
    // =================================

    bool EstaNoFeixeUV(Image imagem)
    {
        if (!spotLight.enabled)
            return false;

        Vector3 direcao =
            imagem.transform.position -
            spotLight.transform.position;

        float distancia =
            direcao.magnitude;

        if (distancia > spotLight.range)
            return false;

        direcao.Normalize();

        float angulo =
            Vector3.Angle(
                spotLight.transform.forward,
                direcao
            );

        float metadeDoAngulo =
            spotLight.spotAngle / 2f;

        return angulo <= metadeDoAngulo;
    }

    // =================================
    // ALPHA
    // =================================

    void DefinirAlpha(
        Image imagem,
        float alpha
    )
    {
        Color cor =
            imagem.color;

        cor.a = alpha;

        imagem.color = cor;
    }
}