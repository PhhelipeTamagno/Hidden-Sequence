using UnityEngine;

public class IndicadorDestino : MonoBehaviour
{
    [Header("Referências")]
    public Transform jogador;
    public Transform destino;

    [Header("Seta UI")]
    public RectTransform seta;

    [Header("Waypoint Beacon")]
    public GameObject waypointBeacon;

    [Header("Popup existente")]
    public CenaDoCrimeIntro cenaDoCrimeIntro;

    [TextArea]
    public string mensagemAoChegar =
        "Agora entre na casa da vítima e procure pistas.";

    [Header("Distância para concluir")]
    public float distanciaParaConcluir = 8f;

    [Header("Posição da seta")]
    public float alturaNaTela = -80f;

    [Header("Ajuste da seta")]
    public float ajusteRotacao = 0f;

    [Header("Sistema")]
    public bool mostrarIndicador = false;

    private bool destinoConcluido = false;


    void Start()
    {
        ConfigurarSeta();

        if (seta != null)
            seta.gameObject.SetActive(false);

        if (waypointBeacon != null)
            waypointBeacon.SetActive(false);
    }


    void ConfigurarSeta()
    {
        if (seta == null)
            return;

        // Mantém a seta exatamente no centro superior
        seta.anchorMin = new Vector2(0.5f, 1f);
        seta.anchorMax = new Vector2(0.5f, 1f);

        seta.pivot = new Vector2(0.5f, 0.5f);

        seta.anchoredPosition =
            new Vector2(0f, alturaNaTela);
    }


    void Update()
    {
        // Não existe destino
        if (!mostrarIndicador ||
            jogador == null ||
            destino == null)
        {
            EsconderIndicadores();
            return;
        }


        // =========================================
        // DISTÂNCIA ENTRE CARRO E DESTINO
        // =========================================

        Vector3 posJogador = jogador.position;
        Vector3 posDestino = destino.position;

        // Ignora a altura
        posJogador.y = 0f;
        posDestino.y = 0f;

        float distancia =
            Vector3.Distance(
                posJogador,
                posDestino
            );


        // =========================================
        // CHEGOU AO DESTINO
        // =========================================

        if (distancia <= distanciaParaConcluir)
        {
            if (!destinoConcluido)
            {
                ChegouAoDestino();
            }

            return;
        }


        // =========================================
        // MOSTRA INDICADORES
        // =========================================

        if (seta != null)
            seta.gameObject.SetActive(true);

        if (waypointBeacon != null)
            waypointBeacon.SetActive(true);


        // =========================================
        // ATUALIZA A DIREÇÃO
        // =========================================

        AtualizarDirecao();
    }


    void AtualizarDirecao()
    {
        if (jogador == null || destino == null)
            return;


        // Direção do carro até o destino
        Vector3 direcaoDestino =
            destino.position -
            jogador.position;

        direcaoDestino.y = 0f;

        if (direcaoDestino.sqrMagnitude < 0.001f)
            return;

        direcaoDestino.Normalize();


        // Frente do carro
        Vector3 frente =
            jogador.forward;

        frente.y = 0f;

        if (frente.sqrMagnitude < 0.001f)
            return;

        frente.Normalize();


        // Calcula exatamente para qual lado o destino está
        float angulo =
            Vector3.SignedAngle(
                frente,
                direcaoDestino,
                Vector3.up
            );


        // Ajuste caso sua imagem da seta esteja virada
        float rotacao =
            -angulo +
            ajusteRotacao;


        if (seta != null)
        {
            // Rotação instantânea.
            // Se o carro virar 1 grau,
            // a seta já muda 1 grau.
            seta.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    rotacao
                );


            // Mantém a seta fixa no topo
            seta.anchoredPosition =
                new Vector2(
                    0f,
                    alturaNaTela
                );
        }
    }


    // =====================================================
    // CHEGOU AO WAYPOINT
    // =====================================================

    void ChegouAoDestino()
    {
        destinoConcluido = true;

        mostrarIndicador = false;


        // =========================================
        // ESCONDE A SETA
        // =========================================

        if (seta != null)
            seta.gameObject.SetActive(false);


        // =========================================
        // ESCONDE O RAIO
        // =========================================

        if (waypointBeacon != null)
            waypointBeacon.SetActive(false);


        Debug.Log("Waypoint alcançado!");


        // =========================================
        // REUTILIZA O MESMO POPUP
        // =========================================

        if (cenaDoCrimeIntro != null)
        {
            cenaDoCrimeIntro.MostrarPopup(
                mensagemAoChegar
            );
        }
    }


    // =====================================================
    // ESCONDER INDICADORES
    // =====================================================

    void EsconderIndicadores()
    {
        if (seta != null)
            seta.gameObject.SetActive(false);

        if (waypointBeacon != null)
            waypointBeacon.SetActive(false);
    }


    // =====================================================
    // DEFINIR DESTINO
    // =====================================================

    public void DefinirDestino(Transform novoDestino)
    {
        destino = novoDestino;

        destinoConcluido = false;

        mostrarIndicador = true;


        if (seta != null)
            seta.gameObject.SetActive(true);

        if (waypointBeacon != null)
            waypointBeacon.SetActive(true);
    }


    // =====================================================
    // DEFINIR QUEM ESTÁ SE MOVENDO
    // =====================================================

    public void DefinirJogador(Transform novoJogador)
    {
        jogador = novoJogador;
    }


    // =====================================================
    // REMOVER DESTINO
    // =====================================================

    public void RemoverDestino()
    {
        destino = null;

        mostrarIndicador = false;

        destinoConcluido = false;


        if (seta != null)
            seta.gameObject.SetActive(false);

        if (waypointBeacon != null)
            waypointBeacon.SetActive(false);
    }
}