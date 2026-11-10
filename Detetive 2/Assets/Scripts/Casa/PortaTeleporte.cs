
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportDoor : MonoBehaviour
{
    public Transform destino;

    private bool jogadorPerto = false;
    private GameObject jogador;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            jogador = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            jogador = null;
        }
    }

    private void Update()
    {
        if (jogadorPerto && Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (destino != null && jogador != null)
            {
                jogador.transform.position = destino.position;
            }
        }
    }
}
