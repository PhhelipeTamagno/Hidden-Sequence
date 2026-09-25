using UnityEngine;
using UnityEngine.UI;

public class RevelarComUV : MonoBehaviour
{
    [Header("Imagem")]
    public Image imagem;

    [Header("Velocidade")]
    public float velocidadeAparecer = 5f;
    public float velocidadeDesaparecer = 2f;

    private float alphaAtual = 0f;
    private bool estaNaLuz = false;

    void Start()
    {
        if (imagem == null)
            imagem = GetComponent<Image>();

        DefinirAlpha(0f);
    }

    void Update()
    {
        float alphaAlvo = estaNaLuz ? 1f : 0f;

        float velocidade = estaNaLuz
            ? velocidadeAparecer
            : velocidadeDesaparecer;

        alphaAtual = Mathf.MoveTowards(
            alphaAtual,
            alphaAlvo,
            velocidade * Time.deltaTime
        );

        DefinirAlpha(alphaAtual);
    }

    private void DefinirAlpha(float alpha)
    {
        Color cor = imagem.color;
        cor.a = alpha;
        imagem.color = cor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.name == "Player")
            return;

        if (other.GetComponentInParent<LuzUV>())
        {
            estaNaLuz = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<LuzUV>())
        {
            estaNaLuz = false;
        }
    }
}