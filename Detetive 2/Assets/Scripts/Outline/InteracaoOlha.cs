using UnityEngine;

public class OutlineAoMirar : MonoBehaviour
{
    [Header("Câmera")]
    public Camera cameraPlayer;

    [Header("Configuração")]
    public float distancia = 5f;

    private Outline outlineAtual;

    void Update()
    {
        // Desliga o outline anterior
        if (outlineAtual != null)
        {
            outlineAtual.enabled = false;
            outlineAtual = null;
        }

        Ray ray = new Ray(
            cameraPlayer.transform.position,
            cameraPlayer.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distancia))
        {
            if (hit.collider.CompareTag("Interagivel"))
            {
                Outline outline = hit.collider.GetComponent<Outline>();

                if (outline != null)
                {
                    outline.enabled = true;
                    outlineAtual = outline;
                }
            }
        }
    }
}