using UnityEngine;
using UnityEngine.InputSystem;

public class PortaTeleporte : MonoBehaviour
{
    [Header("Destino")]
    public Transform destino;

    [Header("Interação")]
    public float distancia = 4f;
    public Camera cameraPlayer;

    void Update()
    {
        if (cameraPlayer == null)
            cameraPlayer = Camera.main;

        if (destino == null || cameraPlayer == null)
            return;

        Ray ray = new Ray(
            cameraPlayer.transform.position,
            cameraPlayer.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distancia))
        {
            // Verifica se a mira está na porta
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
        GameObject player = cameraPlayer.transform.root.gameObject;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.transform.position = destino.position;
        player.transform.rotation = destino.rotation;

        if (controller != null)
            controller.enabled = true;

        Debug.Log("TELEPORTOU PARA: " + destino.position);
    }
}