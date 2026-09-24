using UnityEngine;
using System.Collections.Generic;

public class TrajetoCarro : MonoBehaviour
{
    [Header("Pontos do trajeto (em ordem)")]
    public List<Transform> waypoints;

    [Header("Configuração")]
    public float distanciaParaAlcancar = 3f;
    public LineRenderer linha;
    public Transform carro;

    private int waypointAtual = 0;

    void Start()
    {
        if (linha == null)
            linha = GetComponent<LineRenderer>();

        DesenharTrajeto();
    }

    void Update()
    {
        if (waypointAtual >= waypoints.Count) return;

        float distancia = Vector3.Distance(carro.position, waypoints[waypointAtual].position);

        if (distancia <= distanciaParaAlcancar)
        {
            waypointAtual++;

            if (waypointAtual >= waypoints.Count)
            {
                OnChegouNoDestinoFinal();
            }
            else
            {
                DesenharTrajeto(); // redesenha só o restante do caminho
            }
        }
    }

    void DesenharTrajeto()
    {
        // Desenha do carro até o waypoint atual, e daí em diante pelos próximos
        List<Vector3> pontos = new List<Vector3> { carro.position };

        for (int i = waypointAtual; i < waypoints.Count; i++)
            pontos.Add(waypoints[i].position);

        linha.positionCount = pontos.Count;
        linha.SetPositions(pontos.ToArray());
    }

    void LateUpdate()
    {
        // Atualiza o ponto inicial da linha pra seguir o carro em tempo real
        if (waypointAtual < waypoints.Count && linha.positionCount > 0)
            linha.SetPosition(0, carro.position);
    }

    void OnChegouNoDestinoFinal()
    {
        linha.positionCount = 0; // some com a linha
        Debug.Log("Carro chegou ao destino!");
        // Aqui você dispara o que precisar: fim de missão, som, UI, etc.
    }
}