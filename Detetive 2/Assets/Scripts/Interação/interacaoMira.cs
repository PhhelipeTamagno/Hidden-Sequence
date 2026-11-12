using System.Linq;
using UnityEngine;

public class InteracaoMira : MonoBehaviour
{
    [Header("Câmera")]
    public Camera cameraPlayer;

    [Header("Configuração")]
    public float distanciaMaxima = 5f;

    [Tooltip("Espessura do raio. Deixa a mira mais tolerante.")]
    public float raioMira = 0.1f;

    [Tooltip("Raiz do Player, para ignorar os colliders dele.")]
    public Transform raizPlayer;

    public LayerMask camadas = Physics.DefaultRaycastLayers;

    [Header("Outline")]
    public float larguraNormal = 0f;
    public float larguraAoMirar = 4f;

    [Header("Debug")]
    public bool debug = false;

    private Transform alvoAtual;
    private Outline outlineAtual;
    private NPCDialogo npcAtual;

    void Start()
    {
        if (cameraPlayer == null)
            cameraPlayer = Camera.main;

        if (raizPlayer == null)
            raizPlayer = transform.root;
    }

    void Update()
    {
        VerificarMira();
    }

    void VerificarMira()
    {
        if (cameraPlayer == null)
            return;

        Transform novoAlvo = ProcurarInteragivelNaMira();

        if (novoAlvo == alvoAtual)
            return;

        DesativarAtual();

        if (novoAlvo == null)
            return;

        alvoAtual = novoAlvo;

        outlineAtual = alvoAtual.GetComponent<Outline>();
        if (outlineAtual == null)
            outlineAtual = alvoAtual.GetComponentInChildren<Outline>();

        if (outlineAtual != null)
        {
            outlineAtual.OutlineWidth = larguraAoMirar;
            outlineAtual.AtivarOutline();
        }
        npcAtual = alvoAtual.GetComponent<NPCDialogo>();
        if (npcAtual == null)
            npcAtual = alvoAtual.GetComponentInParent<NPCDialogo>();
        if (npcAtual == null)
            npcAtual = alvoAtual.GetComponentInChildren<NPCDialogo>();

        if (npcAtual != null)
            npcAtual.DefinirMirado(true);
    }

    Transform ProcurarInteragivelNaMira()
    {
        Ray ray = cameraPlayer.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            raioMira,
            distanciaMaxima,
            camadas,
            QueryTriggerInteraction.Collide
        );

        if (hits.Length == 0)
            return null;

        hits = hits.OrderBy(h => h.distance).ToArray();

        foreach (RaycastHit hit in hits)
        {
            if (hit.distance <= 0.001f)
                continue;

            Transform t = hit.collider.transform;

            if (raizPlayer != null && t.IsChildOf(raizPlayer))
                continue;

            Transform interagivel = t;
            while (interagivel != null && !interagivel.CompareTag("Interagivel"))
                interagivel = interagivel.parent;

            if (debug)
            {
                Debug.Log("Mira acertou: " + hit.collider.name +
                          " | trigger: " + hit.collider.isTrigger +
                          " | dist: " + hit.distance.ToString("F2") +
                          " | Interagivel: " + (interagivel != null ? interagivel.name : "não"));
            }

            if (interagivel != null)
            {
                if (!RaioFinoAcerta(ray, hit.collider))
                {
                    if (!hit.collider.isTrigger)
                        return null;

                    continue;
                }

                return interagivel;
            }

            if (!hit.collider.isTrigger)
                return null;
        }

        return null;
    }

    bool RaioFinoAcerta(Ray ray, Collider alvo)
    {
        RaycastHit[] finos = Physics.RaycastAll(
            ray,
            distanciaMaxima,
            camadas,
            QueryTriggerInteraction.Collide
        );

        foreach (RaycastHit f in finos)
        {
            if (f.collider == alvo && f.distance > 0.001f)
                return true;
        }

        return false;
    }

    void DesativarAtual()
    {
        if (outlineAtual != null)
        {
            outlineAtual.OutlineWidth = larguraNormal;
            outlineAtual.DesativarOutline();
        }

        if (npcAtual != null)
            npcAtual.DefinirMirado(false);

        outlineAtual = null;
        npcAtual = null;
        alvoAtual = null;
    }

    void OnDisable()
    {
        DesativarAtual();
    }
}