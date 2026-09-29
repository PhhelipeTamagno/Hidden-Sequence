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
        // Primeiro remove o destaque do objeto anterior
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
            // Tenta pegar a tag do objeto atingido
            Transform objeto = hit.transform;

            // Se o Collider estiver em um filho,
            // procura o objeto pai com a tag
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

            // Procura o Outline
            Outline outline = objeto.GetComponent<Outline>();

            // Caso o Outline esteja no filho
            if (outline == null)
            {
                outline = objeto.GetComponentInChildren<Outline>();
            }

            // Caso o Outline esteja no pai
            if (outline == null)
            {
                outline = objeto.GetComponentInParent<Outline>();
            }

            if (outline != null)
            {
                outlineAtual = outline;

                // Faz o contorno aparecer
                outlineAtual.OutlineWidth = larguraAoMirar;
            }
        }
    }
}