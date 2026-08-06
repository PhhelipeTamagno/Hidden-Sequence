using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarroController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 10f;
    public float velocidadeRe = 5f;
    public float velocidadeRotacao = 50f;

    private Rigidbody rb;
    private float vertical;
    private float horizontal;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Evita que o carro tombe (bom pra carro arcade/top-down)
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Evita "atravessar" paredes finas em alta velocidade
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // IMPORTANTE: Is Kinematic deve estar DESMARCADO no Inspector
    }

    void Update()
    {
        // Input é lido a cada frame pra ficar responsivo
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        float velAtual = vertical >= 0 ? velocidade : velocidadeRe;

        Vector3 movimento = transform.forward * vertical * velAtual;
        rb.MovePosition(rb.position + movimento * Time.fixedDeltaTime);

        float fatorGiro = Mathf.Clamp01(Mathf.Abs(vertical));
        float direcaoGiro = vertical >= 0 ? 1f : -1f;
        float anguloGiro = horizontal * velocidadeRotacao * fatorGiro * direcaoGiro * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, anguloGiro, 0f));
    }
}