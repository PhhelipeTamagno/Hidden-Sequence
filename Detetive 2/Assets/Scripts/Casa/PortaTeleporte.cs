using UnityEngine;

// Coloque este script em TODAS as portas (entradas e porta da sala).
// Cada porta precisa de um Collider com "Is Trigger" marcado.
public class PortaTeleport : MonoBehaviour
{
    [Tooltip("Porta de entrada: arraste aqui a porta da sala. Na porta da sala, deixe vazio (ela volta para a porta de origem).")]
    public PortaTeleport destino;

    [Tooltip("Objeto vazio na frente da porta, onde o jogador aparece ao CHEGAR nela. O eixo azul (Z) deve apontar para longe da porta.")]
    public Transform pontoDeChegada;

    public KeyCode tecla = KeyCode.E;

    static PortaTeleport ultimaOrigem;   // porta pela qual o jogador entrou
    static float proximoUso;     // pequeno intervalo para evitar uso duplo

    Transform jogador;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jogador = other.transform;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == jogador) jogador = null;
    }

    void Update()
    {
        if (jogador == null || !Input.GetKeyDown(tecla) || Time.time < proximoUso) return;

        // Porta de entrada vai para o destino; porta da sala volta para a origem.
        PortaTeleport alvo = destino != null ? destino : ultimaOrigem;
        if (alvo == null || alvo.pontoDeChegada == null) return;

        if (destino != null) ultimaOrigem = this;

        proximoUso = Time.time + 0.3f;
        Teleportar(jogador, alvo.pontoDeChegada);
    }

    void Teleportar(Transform quem, Transform ponto)
    {
        // O CharacterController precisa ser desligado, senão ele desfaz o teleporte.
        var cc = quem.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        quem.SetPositionAndRotation(ponto.position, ponto.rotation);

        if (cc != null) cc.enabled = true;
    }
}