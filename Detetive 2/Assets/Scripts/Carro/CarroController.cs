using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarroController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 10f;
    public float velocidadeRe = 5f;
    public float velocidadeRotacao = 50f;

    [Header("Frenagem")]
    public float desaceleracao = 2.5f;
    public float forcaFreio = 8f;

    [Header("Sons do Carro")]
    public AudioSource audioSource;
    public AudioClip somEntrar;
    public AudioClip somSair;

    private Rigidbody rb;
    private float vertical;
    private float horizontal;

    private float velocidadeAtual = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;
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

        if (vertical > 0)
        {
            velocidadeAtual = Mathf.MoveTowards(
                velocidadeAtual,
                velocidade,
                desaceleracao * Time.fixedDeltaTime
            );
        }
        else if (vertical < 0)
        {
            if (velocidadeAtual > 0)
            {
                velocidadeAtual = Mathf.MoveTowards(
                    velocidadeAtual,
                    0f,
                    forcaFreio * Time.fixedDeltaTime
                );
            }
            else
            {
                velocidadeAtual = Mathf.MoveTowards(
                    velocidadeAtual,
                    -velocidadeRe,
                    desaceleracao * Time.fixedDeltaTime
                );
            }
        }
        else
        {
            velocidadeAtual = Mathf.MoveTowards(
                velocidadeAtual,
                0f,
                desaceleracao * Time.fixedDeltaTime
            );
        }

        Vector3 movimento =
            transform.forward *
            velocidadeAtual;

        rb.MovePosition(
            rb.position +
            movimento * Time.fixedDeltaTime
        );

        float fatorGiro = Mathf.Clamp01(
            Mathf.Abs(velocidadeAtual) / velocidade
        );

        float direcaoGiro =
            velocidadeAtual >= 0
            ? 1f
            : -1f;

        float anguloGiro =
            horizontal *
            velocidadeRotacao *
            fatorGiro *
            direcaoGiro *
            Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(
                0f,
                anguloGiro,
                0f
            )
        );
    }

    // Chamar quando o jogador entrar no carro
    public void TocarSomEntrar()
    {
        if (audioSource != null && somEntrar != null)
        {
            audioSource.PlayOneShot(somEntrar);
        }
    }

    // Chamar quando o jogador sair do carro
    public void TocarSomSair()
    {
        if (audioSource != null && somSair != null)
        {
            audioSource.PlayOneShot(somSair);
        }
    }
}