using UnityEngine;

public class LanternaFPS : MonoBehaviour
{
    public Transform cameraPlayer;

    public Vector3 posicao = new Vector3(0.35f, -0.3f, 0.5f);
    public Vector3 rotacao = new Vector3(0f, 0f, 0f);

    void LateUpdate()
    {
        transform.position =
            cameraPlayer.position +
            cameraPlayer.right * posicao.x +
            cameraPlayer.up * posicao.y +
            cameraPlayer.forward * posicao.z;

        transform.rotation = cameraPlayer.rotation * Quaternion.Euler(rotacao);
    }
}