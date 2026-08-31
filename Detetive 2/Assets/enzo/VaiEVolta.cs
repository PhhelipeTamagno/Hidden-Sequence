using UnityEngine;

public class VaiEVolta : MonoBehaviour
{
    [Header("Configuração")]
    public Transform pontoDestino;
    public float velocidade = 3f;

    private Vector3 pontoInicial;
    private bool indo = true;

    void Start()
    {
        pontoInicial = transform.position;
    }

    void Update()
    {
        if (pontoDestino == null)
            return;

        Vector3 destinoAtual = indo ? pontoDestino.position : pontoInicial;

        // Move o objeto em direção ao destino
        transform.position = Vector3.MoveTowards(
            transform.position,
            destinoAtual,
            velocidade * Time.deltaTime
        );

        // Chegou ao destino
        if (Vector3.Distance(transform.position, destinoAtual) < 0.01f)
        {
            indo = !indo;
        }
    }
}