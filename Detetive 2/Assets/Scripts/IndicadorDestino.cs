using UnityEngine;

public class IndicadorDestino : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Transform destino;
    public RectTransform seta;

    [Header("Configuração")]
    public float distanciaParaConcluir = 8f;
    public float velocidadeRotacao = 10f;

    [Header("Posição da Seta")]
    public float alturaNaTela = -80f;

    public bool mostrarIndicador = false;

    void Start()
    {
        if (seta != null)
        {
            seta.gameObject.SetActive(false);

            // Deixa a seta fixa no centro superior
            seta.anchorMin = new Vector2(0.5f, 1f);
            seta.anchorMax = new Vector2(0.5f, 1f);

            seta.pivot = new Vector2(0.5f, 0.5f);

            seta.anchoredPosition =
                new Vector2(0f, alturaNaTela);
        }
    }

    void Update()
    {
        if (!mostrarIndicador ||
            jogador == null ||
            destino == null ||
            seta == null)
        {
            Esconder();
            return;
        }

        float distancia =
            Vector3.Distance(
                jogador.position,
                destino.position
            );

        // Chegou ao destino
        if (distancia <= distanciaParaConcluir)
        {
            Esconder();
            return;
        }

        seta.gameObject.SetActive(true);

        AtualizarSeta();
    }

    void AtualizarSeta()
    {
        // Direção do jogador até o destino
        Vector3 direcao =
            destino.position -
            jogador.position;

        direcao.y = 0f;

        if (direcao.sqrMagnitude < 0.01f)
            return;

        direcao.Normalize();

        // Direção para frente do jogador
        Vector3 frente =
            jogador.forward;

        frente.y = 0f;
        frente.Normalize();

        // Calcula o ângulo entre o jogador e o destino
        float angulo =
            Vector3.SignedAngle(
                frente,
                direcao,
                Vector3.up
            );

        // A seta começa apontando para cima.
        // Por isso usamos -angulo.
        Quaternion rotacaoDesejada =
            Quaternion.Euler(
                0f,
                0f,
                -angulo
            );

        seta.rotation =
            Quaternion.Lerp(
                seta.rotation,
                rotacaoDesejada,
                Time.deltaTime * velocidadeRotacao
            );

        // Garante que continua exatamente no centro superior
        seta.anchoredPosition =
            new Vector2(
                0f,
                alturaNaTela
            );
    }

    void Esconder()
    {
        if (seta != null)
            seta.gameObject.SetActive(false);
    }

    public void DefinirDestino(Transform novoDestino)
    {
        destino = novoDestino;
        mostrarIndicador = true;
    }

    public void RemoverDestino()
    {
        destino = null;
        mostrarIndicador = false;

        Esconder();
    }
}