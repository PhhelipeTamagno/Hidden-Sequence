using UnityEngine;

public class InteracaoMira : MonoBehaviour
{
    [Header("Câmera")]
    public Camera cameraPlayer;

    [Header("Configuração")]
    public float distanciaMaxima = 5f;

    [Header("Outline")]
    public float larguraNormal = 0f;
    public float larguraAoMirar = 4f;

    private Outline outlineAtual;

    void Start()
    {
        if (cameraPlayer == null)
        {
            cameraPlayer = Camera.main;
        }
    }

    void Update()
    {
        VerificarMira();
    }

    void VerificarMira()
    {
        if (outlineAtual != null)
        {
            outlineAtual.OutlineWidth = larguraNormal;
            outlineAtual = null;
        }

        if (cameraPlayer == null)
            return;

        Ray ray = new Ray(
            cameraPlayer.transform.position,
            cameraPlayer.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            distanciaMaxima,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            Transform objeto = hit.transform;

            if (!objeto.CompareTag("Interagivel"))
            {
                if (objeto.parent != null &&
                    objeto.parent.CompareTag("Interagivel"))
                {
                    objeto = objeto.parent;
                }
                else
                {
                    return;
                }
            }
            Outline outline = objeto.GetComponent<Outline>();

      
            if (outline == null)
            {
                outline = objeto.GetComponentInChildren<Outline>();
            }

            if (outline == null)
            {
                outline = objeto.GetComponentInParent<Outline>();
            }

            if (outline != null)
            {
                outlineAtual = outline;

                outlineAtual.OutlineWidth = larguraAoMirar;
            }
        }
    }
}