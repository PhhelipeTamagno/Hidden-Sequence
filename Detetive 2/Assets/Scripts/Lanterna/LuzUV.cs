using UnityEngine;
using UnityEngine.UI;

public class LuzUV : MonoBehaviour
{
    [Header("Referências")]
    public Light spotLight;

    [Header("Imagens das Digitais")]
    public Image[] imagens;

    [Header("Alpha")]
    public float velocidadeAparecer = 3f;
    public float velocidadeDesaparecer = 5f;

    [Header("Luz Normal")]
    public float alcanceNormal = 10f;
    public float intensidadeNormal = 5f;

    [Header("Luz Perto da Digital")]
    public float distanciaParaAmpliar = 2.5f;
    public float alcancePerto = 15f;
    public float intensidadePerto = 2f;

    private void Start()
    {
        ApagarPegadas();

        if (spotLight != null)
        {
            spotLight.range = alcanceNormal;
            spotLight.intensity = intensidadeNormal;
        }
    }

    private void Update()
    {
        if (spotLight == null)
        {
            ApagarPegadas();
            return;
        }

        if (!spotLight.gameObject.activeInHierarchy || !spotLight.enabled)
        {
            ApagarPegadas();
            return;
        }

        float distanciaMaisProxima = EncontrarImagemMaisProxima();

        AtualizarLuz(distanciaMaisProxima);

        foreach (Image imagem in imagens)
        {
            if (imagem == null)
                continue;

            bool estaNoFeixe = EstaNoFeixeUV(imagem);

            float alphaAtual = imagem.color.a;

            float alphaAlvo = estaNoFeixe ? 1f : 0f;

            float velocidade = estaNoFeixe
                ? velocidadeAparecer
                : velocidadeDesaparecer;

            float novoAlpha = Mathf.MoveTowards(
                alphaAtual,
                alphaAlvo,
                velocidade * Time.deltaTime
            );

            DefinirAlpha(imagem, novoAlpha);

            if (estaNoFeixe && novoAlpha >= 0.95f)
            {
                DigitalUV digital = imagem.GetComponent<DigitalUV>();

                if (digital != null)
                {
                    digital.MarcarEncontrada();
                }
            }
        }
    }

    float EncontrarImagemMaisProxima()
    {
        float menorDistancia = Mathf.Infinity;

        foreach (Image imagem in imagens)
        {
            if (imagem == null)
                continue;

            float distancia = Vector3.Distance(
                transform.position,
                imagem.transform.position
            );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
            }
        }

        return menorDistancia;
    }

    void AtualizarLuz(float distancia)
    {
        if (distancia <= distanciaParaAmpliar)
        {
            spotLight.range = alcancePerto;
            spotLight.intensity = intensidadePerto;
        }
        else
        {
            spotLight.range = alcanceNormal;
            spotLight.intensity = intensidadeNormal;
        }
    }

    bool EstaNoFeixeUV(Image imagem)
    {
        if (!spotLight.enabled)
            return false;

        Vector3 direcao =
            imagem.transform.position -
            spotLight.transform.position;

        float distancia = direcao.magnitude;

        if (distancia > spotLight.range)
            return false;

        direcao.Normalize();

        float angulo = Vector3.Angle(
            spotLight.transform.forward,
            direcao
        );

        float metadeDoAngulo =
            spotLight.spotAngle / 2f;

        return angulo <= metadeDoAngulo;
    }

    void DefinirAlpha(Image imagem, float alpha)
    {
        Color cor = imagem.color;

        cor.a = alpha;

        imagem.color = cor;
    }

    public void ApagarPegadas()
    {
        foreach (Image imagem in imagens)
        {
            if (imagem == null)
                continue;

            DefinirAlpha(imagem, 0f);
        }
    }

    
    private void OnDisable()
    {
        ApagarPegadas();
    }
}