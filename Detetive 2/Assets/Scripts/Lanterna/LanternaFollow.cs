using UnityEngine;

public class LanternaFollow : MonoBehaviour
{
    public Transform cameraPlayer;
    public float distancia = 0.5f;
    public LayerMask paredes;

    void LateUpdate()
    {
        Vector3 dir = cameraPlayer.forward;
        float dist = distancia;

        if (Physics.Raycast(cameraPlayer.position, dir, out RaycastHit hit, distancia, paredes))
            dist = hit.distance - 0.05f;

        transform.position = cameraPlayer.position + dir * Mathf.Max(dist, 0.05f);
        transform.rotation = cameraPlayer.rotation;
    }
}