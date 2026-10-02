using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarroSistema : MonoBehaviour
{
    // =========================================================
    // MOVIMENTO
    // =========================================================

    [Header("========== MOVIMENTO ==========")]

    public float velocidadeMaxima = 15f;
    public float forcaAceleracao = 1200f;
    public float forcaFreio = 1800f;
    public float velocidadeRe = 7f;

    public float resistenciaAr = 0.5f;

    private Rigidbody rb;

    private float acelerador;
    private float direcao;

    // =========================================================
    // DIREÇÃO
    // =========================================================

    [Header("========== DIREÇÃO ==========")]

    public float velocidadeRotacao = 70f;

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("========== PLAYER ==========")]

    public GameObject player;

    public Transform pontoEntrada;

    public Transform pontoSaida;

    public float distanciaEntrada = 3f;

    private CharacterController playerController;

    private bool jogadorDentro = false;

    // =========================================================
    // CÂMERA
    // =========================================================

    [Header("========== CÂMERA GTA ==========")]

    public Camera cameraCarro;

    public Transform alvoCamera;

    public float distanciaCamera = 7f;

    public float alturaCamera = 2.5f;

    public float sensibilidadeMouseX = 3f;

    public float sensibilidadeMouseY = 2f;

    public float suavidadeCamera = 8f;

    public float suavidadeRotacaoCamera = 10f;

    public float distanciaMinima = 3f;

    public float distanciaMaxima = 12f;

    public bool inverterMouseY = false;

    public bool esconderCursor = true;

    private float rotacaoCameraX;

    private float rotacaoCameraY = 15f;

    private float distanciaAtual;

    // =========================================================
    // RODAS
    // =========================================================

    [Header("========== RODAS DIANTEIRAS ==========")]

    public Transform rodaFrenteEsquerda;

    public Transform rodaFrenteDireita;

    public float anguloMaximoRodas = 30f;

    public float velocidadeRodas = 8f;

    private Quaternion rotacaoOriginalEsquerda;

    private Quaternion rotacaoOriginalDireita;

    // =========================================================
    // ÁUDIO
    // =========================================================

    [Header("========== ÁUDIO ==========")]

    public AudioSource somMotor;

    public AudioSource somPorta;

    public AudioClip somPortaAbrindo;

    public AudioClip somPortaFechando;

    // =========================================================
    // SOM DO MOTOR
    // =========================================================

    [Header("========== SOM DO MOTOR ==========")]

    public float pitchMinimo = 0.7f;

    public float pitchMaximo = 1.8f;

    [Range(0f, 1f)]
    public float volumeMotor = 1f;

    // =========================================================
    // ORIENTAÇÃO
    // =========================================================

    [Header("========== ORIENTAÇÃO ==========")]

    [Tooltip("Ative se a frente do carro estiver no eixo X.")]
    public bool frenteNoEixoX = true;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // =====================================================
        // RIGIDBODY
        // =====================================================

        rb.mass = 1200f;

        rb.useGravity = true;

        rb.isKinematic = false;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        // =====================================================
        // PLAYER
        // =====================================================

        if (player != null)
        {
            playerController =
                player.GetComponent<CharacterController>();
        }

        // =====================================================
        // CÂMERA
        // =====================================================

        distanciaAtual = distanciaCamera;

        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(false);
        }

        if (alvoCamera == null)
        {
            alvoCamera = transform;
        }

        // =====================================================
        // RODAS
        // =====================================================

        if (rodaFrenteEsquerda != null)
        {
            rotacaoOriginalEsquerda =
                rodaFrenteEsquerda.localRotation;
        }

        if (rodaFrenteDireita != null)
        {
            rotacaoOriginalDireita =
                rodaFrenteDireita.localRotation;
        }

        // =====================================================
        // SOM DO MOTOR
        // =====================================================

        if (somMotor != null)
        {
            somMotor.loop = true;
            somMotor.playOnAwake = false;
            somMotor.volume = 0f;
        }

        // =====================================================
        // CURSOR
        // =====================================================

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // =====================================================
        // FORA DO CARRO
        // =====================================================

        if (!jogadorDentro)
        {
            VerificarEntrada();
        }

        // =====================================================
        // DENTRO DO CARRO
        // =====================================================

        else
        {
            LerControles();

            AtualizarCamera();

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SairDoCarro();
            }
        }

        AtualizarSomMotor();
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    void FixedUpdate()
    {
        if (!jogadorDentro)
            return;

        MovimentarCarro();

        GirarCarro();

        AtualizarRodas();
    }

    // =========================================================
    // DIREÇÃO DA FRENTE
    // =========================================================

    Vector3 DirecaoFrente()
    {
        if (frenteNoEixoX)
        {
            return transform.right;
        }

        return transform.forward;
    }

    // =========================================================
    // VERIFICAR ENTRADA
    // =========================================================

    void VerificarEntrada()
    {
        if (player == null)
            return;

        if (pontoEntrada == null)
            return;

        float distancia =
            Vector3.Distance(
                player.transform.position,
                pontoEntrada.position
            );

        if (distancia <= distanciaEntrada)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                EntrarNoCarro();
            }
        }
    }

    // =========================================================
    // ENTRAR NO CARRO
    // =========================================================

    void EntrarNoCarro()
    {
        jogadorDentro = true;

        // PLAYER
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (player != null)
        {
            player.SetActive(false);
        }

        // CÂMERA
        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(true);
        }

        if (alvoCamera == null)
        {
            alvoCamera = transform;
        }

        rotacaoCameraX =
            transform.eulerAngles.y;

        rotacaoCameraY = 15f;

        // CURSOR
        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }

        // SOM DA PORTA
        if (somPorta != null &&
            somPortaFechando != null)
        {
            somPorta.PlayOneShot(
                somPortaFechando
            );
        }

        // SOM DO MOTOR
        if (somMotor != null &&
            !somMotor.isPlaying)
        {
            somMotor.Play();
        }
    }

    // =========================================================
    // SAIR DO CARRO
    // =========================================================

    void SairDoCarro()
    {
        jogadorDentro = false;

        // CÂMERA
        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(false);
        }

        // PLAYER
        if (player != null)
        {
            if (pontoSaida != null)
            {
                player.transform.position =
                    pontoSaida.position;

                player.transform.rotation =
                    pontoSaida.rotation;
            }

            player.SetActive(true);

            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }

        // CURSOR
        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }

        // SOM DA PORTA
        if (somPorta != null &&
            somPortaAbrindo != null)
        {
            somPorta.PlayOneShot(
                somPortaAbrindo
            );
        }

        // MOTOR
        if (somMotor != null)
        {
            somMotor.Stop();
        }

        acelerador = 0f;
        direcao = 0f;
    }

    // =========================================================
    // CONTROLES
    // =========================================================

    void LerControles()
    {
        acelerador = 0f;
        direcao = 0f;

        // W = FRENTE
        if (Keyboard.current.wKey.isPressed)
        {
            acelerador = 1f;
        }

        // S = RÉ
        if (Keyboard.current.sKey.isPressed)
        {
            acelerador = -1f;
        }

        // A = ESQUERDA
        if (Keyboard.current.aKey.isPressed)
        {
            direcao = -1f;
        }

        // D = DIREITA
        if (Keyboard.current.dKey.isPressed)
        {
            direcao = 1f;
        }
    }

    // =========================================================
    // MOVIMENTO FÍSICO
    // =========================================================

    void MovimentarCarro()
    {
        Vector3 frente =
            DirecaoFrente();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        // =====================================================
        // ACELERAR
        // =====================================================

        if (acelerador > 0f)
        {
            if (velocidadeFrente < velocidadeMaxima)
            {
                rb.AddForce(
                    frente *
                    forcaAceleracao,
                    ForceMode.Force
                );
            }
        }

        // =====================================================
        // RÉ / FREIO
        // =====================================================

        else if (acelerador < 0f)
        {
            if (velocidadeFrente > 0.5f)
            {
                rb.AddForce(
                    -frente *
                    forcaFreio,
                    ForceMode.Force
                );
            }
            else
            {
                if (velocidadeFrente > -velocidadeRe)
                {
                    rb.AddForce(
                        -frente *
                        forcaAceleracao *
                        0.6f,
                        ForceMode.Force
                    );
                }
            }
        }

        // =====================================================
        // DESACELERAÇÃO
        // =====================================================

        else
        {
            Vector3 velocidadeHorizontal =
                new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z
                );

            velocidadeHorizontal =
                Vector3.MoveTowards(
                    velocidadeHorizontal,
                    Vector3.zero,
                    resistenciaAr *
                    Time.fixedDeltaTime
                );

            rb.linearVelocity =
                new Vector3(
                    velocidadeHorizontal.x,
                    rb.linearVelocity.y,
                    velocidadeHorizontal.z
                );
        }

        // =====================================================
        // LIMITAR VELOCIDADE
        // =====================================================

        Vector3 velocidade =
            rb.linearVelocity;

        Vector3 velocidadeHorizontalAtual =
            new Vector3(
                velocidade.x,
                0f,
                velocidade.z
            );

        if (velocidadeHorizontalAtual.magnitude >
            velocidadeMaxima)
        {
            velocidadeHorizontalAtual =
                velocidadeHorizontalAtual.normalized *
                velocidadeMaxima;

            rb.linearVelocity =
                new Vector3(
                    velocidadeHorizontalAtual.x,
                    rb.linearVelocity.y,
                    velocidadeHorizontalAtual.z
                );
        }
    }

    // =========================================================
    // GIRAR CARRO
    // =========================================================

    void GirarCarro()
    {
        Vector3 frente =
            DirecaoFrente();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        if (Mathf.Abs(velocidadeFrente) < 0.5f)
            return;

        float fatorVelocidade =
            Mathf.Clamp01(
                Mathf.Abs(velocidadeFrente) /
                velocidadeMaxima
            );

        float sentido =
            velocidadeFrente >= 0f
            ? 1f
            : -1f;

        float angulo =
            direcao *
            velocidadeRotacao *
            fatorVelocidade *
            sentido *
            Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(
                0f,
                angulo,
                0f
            )
        );
    }

    // =========================================================
    // RODAS DIANTEIRAS
    // =========================================================

    void AtualizarRodas()
    {
        if (rodaFrenteEsquerda == null ||
            rodaFrenteDireita == null)
            return;

        float angulo =
            direcao *
            anguloMaximoRodas;

        Quaternion alvoEsquerda =
            rotacaoOriginalEsquerda *
            Quaternion.Euler(
                0f,
                angulo,
                0f
            );

        Quaternion alvoDireita =
            rotacaoOriginalDireita *
            Quaternion.Euler(
                0f,
                angulo,
                0f
            );

        float suavidade =
            1f -
            Mathf.Exp(
                -velocidadeRodas *
                Time.fixedDeltaTime
            );

        rodaFrenteEsquerda.localRotation =
            Quaternion.Slerp(
                rodaFrenteEsquerda.localRotation,
                alvoEsquerda,
                suavidade
            );

        rodaFrenteDireita.localRotation =
            Quaternion.Slerp(
                rodaFrenteDireita.localRotation,
                alvoDireita,
                suavidade
            );
    }

    // =========================================================
    // CÂMERA GTA
    // =========================================================

    void AtualizarCamera()
    {
        if (cameraCarro == null)
            return;

        if (alvoCamera == null)
        {
            alvoCamera = transform;
        }

        if (Mouse.current == null)
            return;

        Vector2 mouse =
            Mouse.current.delta.ReadValue();

        rotacaoCameraX +=
            mouse.x *
            sensibilidadeMouseX *
            Time.deltaTime;

        if (inverterMouseY)
        {
            rotacaoCameraY +=
                mouse.y *
                sensibilidadeMouseY *
                Time.deltaTime;
        }
        else
        {
            rotacaoCameraY -=
                mouse.y *
                sensibilidadeMouseY *
                Time.deltaTime;
        }

        rotacaoCameraY =
            Mathf.Clamp(
                rotacaoCameraY,
                -10f,
                65f
            );

        // =====================================================
        // ZOOM
        // =====================================================

        Vector2 scroll =
            Mouse.current.scroll.ReadValue();

        distanciaAtual -=
            scroll.y * 0.01f;

        distanciaAtual =
            Mathf.Clamp(
                distanciaAtual,
                distanciaMinima,
                distanciaMaxima
            );

        // =====================================================
        // POSIÇÃO
        // =====================================================

        Quaternion rotacao =
            Quaternion.Euler(
                rotacaoCameraY,
                rotacaoCameraX,
                0f
            );

        Vector3 posicaoDesejada =
            alvoCamera.position +
            rotacao *
            new Vector3(
                0f,
                alturaCamera,
                -distanciaAtual
            );

        float suavidade =
            1f -
            Mathf.Exp(
                -suavidadeCamera *
                Time.deltaTime
            );

        cameraCarro.transform.position =
            Vector3.Lerp(
                cameraCarro.transform.position,
                posicaoDesejada,
                suavidade
            );

        // =====================================================
        // OLHAR PARA O CARRO
        // =====================================================

        Vector3 pontoOlhar =
            alvoCamera.position +
            Vector3.up;

        Vector3 direcaoOlhar =
            pontoOlhar -
            cameraCarro.transform.position;

        if (direcaoOlhar.sqrMagnitude > 0.001f)
        {
            Quaternion rotacaoDesejada =
                Quaternion.LookRotation(
                    direcaoOlhar
                );

            float suavidadeRotacao =
                1f -
                Mathf.Exp(
                    -suavidadeRotacaoCamera *
                    Time.deltaTime
                );

            cameraCarro.transform.rotation =
                Quaternion.Slerp(
                    cameraCarro.transform.rotation,
                    rotacaoDesejada,
                    suavidadeRotacao
                );
        }
    }

    // =========================================================
    // SOM DO MOTOR
    // =========================================================

    void AtualizarSomMotor()
    {
        if (!jogadorDentro)
            return;

        if (somMotor == null)
            return;

        float velocidade =
            rb.linearVelocity.magnitude;

        float percentual =
            Mathf.Clamp01(
                velocidade /
                velocidadeMaxima
            );

        somMotor.pitch =
            Mathf.Lerp(
                pitchMinimo,
                pitchMaximo,
                percentual
            );

        somMotor.volume =
            Mathf.Lerp(
                0.15f,
                volumeMotor,
                percentual
            );
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    void OnDrawGizmosSelected()
    {
        if (pontoEntrada != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(
                pontoEntrada.position,
                0.3f
            );

            Gizmos.DrawWireSphere(
                pontoEntrada.position,
                distanciaEntrada
            );
        }

        if (pontoSaida != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(
                pontoSaida.position,
                0.3f
            );
        }

        Gizmos.color = Color.blue;

        Vector3 frente =
            frenteNoEixoX
            ? transform.right
            : transform.forward;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            frente * 3f
        );
    }
}