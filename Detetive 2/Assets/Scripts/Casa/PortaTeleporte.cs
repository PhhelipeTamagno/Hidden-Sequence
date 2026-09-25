using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    public Transform destino;

    private bool jogadorPerto = false;
    private GameObject jogador;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            jogadorPerto = true;
            jogador = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            jogadorPerto = false;
            jogador = null;
        }
    }

    private void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            jogador.transform.position = destino.position;
        }
    }
}