using UnityEngine;

public class MovimentoLoop : MonoBehaviour
{
    [Header("Configuração")]
    public Transform pontoDestino;
    public float velocidade = 3f;

    private Vector3 pontoInicial;

    void Start()
    {
        pontoInicial = transform.position;
    }

    void Update()
    {
        if (pontoDestino == null)
            return;

        // Move para frente em direção ao destino
        transform.position = Vector3.MoveTowards(
            transform.position,
            pontoDestino.position,
            velocidade * Time.deltaTime
        );

        // Chegou no destino
        if (Vector3.Distance(transform.position, pontoDestino.position) < 0.01f)
        {
            // Teleporta de volta para o início
            transform.position = pontoInicial;
        }
    }
}