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

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        if (!enabled)
            return;

        float velAtual = vertical >= 0 ? velocidade : velocidadeRe;

        Vector3 movimento =
            transform.forward * vertical * velAtual;

        rb.MovePosition(
            rb.position +
            movimento * Time.fixedDeltaTime
        );

        float fatorGiro = Mathf.Clamp01(Mathf.Abs(vertical));

        float direcaoGiro =
            vertical >= 0 ? 1f : -1f;

        float anguloGiro =
            horizontal *
            velocidadeRotacao *
            fatorGiro *
            direcaoGiro *
            Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(0f, anguloGiro, 0f)
        );
    }
}