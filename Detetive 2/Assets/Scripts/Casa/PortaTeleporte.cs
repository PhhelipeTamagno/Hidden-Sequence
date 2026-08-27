using UnityEngine;
using UnityEngine.InputSystem;

public class PortaTeleporte : MonoBehaviour
{
    [Header("Destinos")]
    public Transform destinoDentro;
    public Transform destinoFora;

    [Header("Configuração")]
    public float distanciaInteracao = 4f;
    public Camera cameraPlayer;

    private bool estaDentro = false;

    void Start()
    {
        if (cameraPlayer == null)
            cameraPlayer = Camera.main;
    }

    void Update()
    {
        if (cameraPlayer == null)
            return;

        Ray ray = new Ray(
            cameraPlayer.transform.position,
            cameraPlayer.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteracao))
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
            {
                if (Keyboard.current != null &&
                    Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Teleportar();
                }
            }
        }
    }

    void Teleportar()
    {
        Transform destino;

        if (estaDentro)
            destino = destinoFora;
        else
            destino = destinoDentro;

        if (destino == null)
        {
            Debug.LogError("Destino não foi configurado na porta!");
            return;
        }

        GameObject player = cameraPlayer.transform.root.gameObject;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.transform.position = destino.position;
        player.transform.rotation = destino.rotation;

        if (controller != null)
            controller.enabled = true;

        estaDentro = !estaDentro;

        Debug.Log("Teleportou para: " + destino.name);
    }
}