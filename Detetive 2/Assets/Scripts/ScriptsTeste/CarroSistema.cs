using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarroSistema : MonoBehaviour
{
    // =========================================================
    // MOVIMENTO
    // =========================================================

    [Header("========== MOVIMENTO ==========")]

    public float velocidadeMaxima = 25f;
    public float velocidadeRe = 8f;
    public float forcaMotor = 9000f;

    // =========================================================
    // FRENAGEM
    // =========================================================

    [Header("========== FRENAGEM ==========")]

    public float forcaFreio = 15000f;
    public float desaceleracao = 2.5f;

    // =========================================================
    // DIREÇÃO DO CARRO
    // =========================================================

    [Header("========== DIREÇÃO ==========")]

    public float anguloMaximo = 32f;
    public float velocidadeDirecao = 4f;

    // =========================================================
    // ADERÊNCIA
    // =========================================================

    [Header("========== ADERÊNCIA ==========")]

    public float aderencia = 4f;

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("========== PLAYER ==========")]

    public GameObject player;

    [Tooltip("Local onde o jogador entra no carro.")]
    public Transform pontoEntrada;

    [Tooltip("Local onde o jogador aparece ao sair.")]
    public Transform pontoSaida;

    [Tooltip("Distância para poder entrar no carro.")]
    public float distanciaEntrada = 3f;

    // =========================================================
    // CÂMERA
    // =========================================================

    [Header("========== CÂMERA GTA ==========")]

    [Tooltip("Câmera usada quando o jogador está dentro do carro.")]
    public Camera cameraCarro;

    [Tooltip("Alvo que a câmera acompanha.")]
    public Transform alvoCamera;

    [Tooltip("Distância da câmera até o carro.")]
    public float distanciaCamera = 7f;

    [Tooltip("Altura da câmera.")]
    public float alturaCamera = 2.5f;

    [Tooltip("Sensibilidade horizontal do mouse.")]
    public float sensibilidadeMouseX = 3f;

    [Tooltip("Sensibilidade vertical do mouse.")]
    public float sensibilidadeMouseY = 2f;

    [Tooltip("Suavidade da posição da câmera.")]
    public float suavidadeCamera = 8f;

    [Tooltip("Suavidade da rotação da câmera.")]
    public float suavidadeRotacaoCamera = 10f;

    [Tooltip("Distância mínima da câmera.")]
    public float distanciaMinima = 3f;

    [Tooltip("Distância máxima da câmera.")]
    public float distanciaMaxima = 12f;

    [Tooltip("Altura mínima da câmera.")]
    public float alturaMinima = 0.5f;

    [Tooltip("Altura máxima da câmera.")]
    public float alturaMaxima = 6f;

    public bool inverterMouseY = false;

    public bool esconderCursor = true;

    // =========================================================
    // RODAS DIANTEIRAS
    // =========================================================

    [Header("========== RODAS DIANTEIRAS ==========")]

    [Tooltip("Roda dianteira esquerda.")]
    public Transform rodaFrenteEsquerda;

    [Tooltip("Roda dianteira direita.")]
    public Transform rodaFrenteDireita;

    [Tooltip("Ângulo máximo das rodas dianteiras.")]
    public float anguloMaximoRodas = 32f;

    [Tooltip("Velocidade com que as rodas viram.")]
    public float velocidadeRodas = 8f;

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

    [Tooltip("Ative se a frente do carro estiver apontando para o eixo X.")]
    public bool frenteNoEixoX = true;

    // =========================================================
    // VARIÁVEIS INTERNAS
    // =========================================================

    private Rigidbody rb;

    private CharacterController playerController;

    private bool jogadorDentro = false;

    private float aceleracao = 0f;

    private float direcao = 0f;

    // =========================================================
    // VARIÁVEIS DA CÂMERA
    // =========================================================

    private float rotacaoCameraX = 0f;

    private float rotacaoCameraY = 15f;

    private float distanciaAtual;

    private float alturaAtual;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Pega o CharacterController do Player
        if (player != null)
        {
            playerController =
                player.GetComponent<CharacterController>();
        }

        // =====================================================
        // CONFIGURAÇÃO DO RIGIDBODY
        // =====================================================

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        // O carro pode se movimentar em:
        // X
        // Y
        // Z
        //
        // Mas só pode girar no eixo Y.

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        // =====================================================
        // CÂMERA
        // =====================================================

        distanciaAtual = distanciaCamera;

        alturaAtual = alturaCamera;

        // A câmera começa DESLIGADA
        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(false);
        }

        // =====================================================
        // SOM DO MOTOR
        // =====================================================

        if (somMotor != null)
        {
            somMotor.loop = true;

            somMotor.playOnAwake = false;

            somMotor.volume = volumeMotor;
        }

        // =====================================================
        // CURSOR
        // =====================================================

        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // =====================================================
        // PLAYER FORA DO CARRO
        // =====================================================

        if (!jogadorDentro)
        {
            VerificarEntrada();
        }

        // =====================================================
        // PLAYER DENTRO DO CARRO
        // =====================================================

        else
        {
            LerControles();

            AtualizarCamera();

            // E = sair
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SairDoCarro();
            }
        }

        // Atualiza o som
        AtualizarSomMotor();
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    void FixedUpdate()
    {
        if (!jogadorDentro)
            return;

        AplicarMotor();

        AplicarDirecao();

        AplicarAderencia();

        AtualizarRodasDianteiras();
    }

    // =========================================================
    // DIREÇÃO DA FRENTE DO CARRO
    // =========================================================

    Vector3 GetDirecaoFrente()
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
        if (jogadorDentro)
            return;

        jogadorDentro = true;

        // =====================================================
        // DESATIVA O CONTROLE DO PLAYER
        // =====================================================

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // =====================================================
        // COLOCA PLAYER NO CARRO
        // =====================================================

        if (player != null && pontoEntrada != null)
        {
            player.transform.position =
                pontoEntrada.position;

            player.transform.rotation =
                pontoEntrada.rotation;
        }

        // =====================================================
        // ESCONDE PLAYER
        // =====================================================

        if (player != null)
        {
            player.SetActive(false);
        }

        // =====================================================
        // ATIVA CÂMERA
        // =====================================================

        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(true);
        }

        // =====================================================
        // DEFINE ALVO DA CÂMERA
        // =====================================================

        if (alvoCamera == null)
        {
            alvoCamera = transform;
        }

        // =====================================================
        // POSIÇÃO INICIAL DA CÂMERA
        // =====================================================

        Vector3 direcaoInicial =
            -GetDirecaoFrente();

        Vector3 posicaoInicial =
            alvoCamera.position +
            direcaoInicial *
            distanciaAtual +
            Vector3.up *
            alturaAtual;

        cameraCarro.transform.position =
            posicaoInicial;

        // =====================================================
        // ROTAÇÃO INICIAL
        // =====================================================

        rotacaoCameraX =
            transform.eulerAngles.y;

        rotacaoCameraY = 15f;

        // =====================================================
        // CURSOR
        // =====================================================

        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }

        // =====================================================
        // SOM DA PORTA
        // =====================================================

        if (somPorta != null &&
            somPortaFechando != null)
        {
            somPorta.PlayOneShot(
                somPortaFechando
            );
        }

        // =====================================================
        // SOM DO MOTOR
        // =====================================================

        if (somMotor != null)
        {
            somMotor.Play();
        }
    }

    // =========================================================
    // SAIR DO CARRO
    // =========================================================

    void SairDoCarro()
    {
        if (!jogadorDentro)
            return;

        jogadorDentro = false;

        // =====================================================
        // DESLIGA CÂMERA
        // =====================================================

        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(false);
        }

        // =====================================================
        // PLAYER
        // =====================================================

        if (player != null &&
            pontoSaida != null)
        {
            player.transform.position =
                pontoSaida.position;

            player.transform.rotation =
                pontoSaida.rotation;

            player.SetActive(true);

            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }

        // =====================================================
        // CURSOR
        // =====================================================

        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }

        // =====================================================
        // SOM DA PORTA
        // =====================================================

        if (somPorta != null &&
            somPortaAbrindo != null)
        {
            somPorta.PlayOneShot(
                somPortaAbrindo
            );
        }

        // =====================================================
        // DESLIGA MOTOR
        // =====================================================

        if (somMotor != null)
        {
            somMotor.Stop();
        }

        aceleracao = 0f;

        direcao = 0f;
    }

    // =========================================================
    // CONTROLES
    // =========================================================

    void LerControles()
    {
        aceleracao = 0f;

        direcao = 0f;

        // =====================================================
        // W = FRENTE
        // =====================================================

        if (Keyboard.current.wKey.isPressed)
        {
            aceleracao = 1f;
        }

        // =====================================================
        // S = FREIO / RÉ
        // =====================================================

        if (Keyboard.current.sKey.isPressed)
        {
            aceleracao = -1f;
        }

        // =====================================================
        // A = ESQUERDA
        // =====================================================

        if (Keyboard.current.aKey.isPressed)
        {
            direcao = -1f;
        }

        // =====================================================
        // D = DIREITA
        // =====================================================

        if (Keyboard.current.dKey.isPressed)
        {
            direcao = 1f;
        }
    }

    // =========================================================
    // MOTOR
    // =========================================================

    void AplicarMotor()
    {
        Vector3 frente =
            GetDirecaoFrente();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        // =====================================================
        // W = FRENTE
        // =====================================================

        if (aceleracao > 0f)
        {
            if (velocidadeFrente <
                velocidadeMaxima)
            {
                rb.AddForce(
                    frente *
                    forcaMotor,
                    ForceMode.Force
                );
            }

            return;
        }

        // =====================================================
        // S = FREIO / RÉ
        // =====================================================

        if (aceleracao < 0f)
        {
            // Se está andando para frente,
            // primeiro freia.
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
                // Depois começa a dar ré.
                float velocidadeReAtual =
                    Vector3.Dot(
                        rb.linearVelocity,
                        -frente
                    );

                if (velocidadeReAtual <
                    velocidadeRe)
                {
                    rb.AddForce(
                        -frente *
                        (forcaMotor * 0.5f),
                        ForceMode.Force
                    );
                }
            }

            return;
        }

        // =====================================================
        // SOLTOU W E S
        // =====================================================

        Vector3 velocidade =
            rb.linearVelocity;

        Vector3 velocidadeHorizontal =
            new Vector3(
                velocidade.x,
                0f,
                velocidade.z
            );

        rb.AddForce(
            -velocidadeHorizontal *
            desaceleracao,
            ForceMode.Force
        );
    }

    // =========================================================
    // DIREÇÃO DO CARRO
    // =========================================================

    void AplicarDirecao()
    {
        float velocidade =
            rb.linearVelocity.magnitude;

        if (velocidade < 0.5f)
            return;

        Vector3 frente =
            GetDirecaoFrente();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        // =====================================================
        // REDUZ DIREÇÃO EM ALTA VELOCIDADE
        // =====================================================

        float percentual =
            Mathf.Clamp01(
                velocidade /
                velocidadeMaxima
            );

        float fatorDirecao =
            Mathf.Lerp(
                1f,
                0.45f,
                percentual
            );

        float angulo =
            direcao *
            anguloMaximo *
            fatorDirecao;

        // =====================================================
        // RÉ
        // =====================================================

        if (velocidadeFrente < -0.5f)
        {
            angulo *= -1f;
        }

        // =====================================================
        // ROTAÇÃO
        // =====================================================

        float rotacao =
            angulo *
            velocidadeDirecao *
            Time.fixedDeltaTime;

        // Só rotaciona no eixo Y
        Quaternion novaRotacao =
            Quaternion.Euler(
                0f,
                rotacao,
                0f
            );

        rb.MoveRotation(
            rb.rotation *
            novaRotacao
        );
    }

    // =========================================================
    // RODAS DIANTEIRAS
    // =========================================================

    void AtualizarRodasDianteiras()
    {
        if (rodaFrenteEsquerda == null)
            return;

        if (rodaFrenteDireita == null)
            return;

        // =====================================================
        // VELOCIDADE
        // =====================================================

        float velocidade =
            rb.linearVelocity.magnitude;

        // =====================================================
        // REDUZ ANGULO EM ALTA VELOCIDADE
        // =====================================================

        float percentual =
            Mathf.Clamp01(
                velocidade /
                velocidadeMaxima
            );

        float fatorVelocidade =
            Mathf.Lerp(
                1f,
                0.45f,
                percentual
            );

        // =====================================================
        // ANGULO DAS RODAS
        // =====================================================

        float angulo =
            direcao *
            anguloMaximoRodas *
            fatorVelocidade;

        // =====================================================
        // RÉ
        // =====================================================

        Vector3 frente =
            GetDirecaoFrente();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        if (velocidadeFrente < -0.5f)
        {
            angulo *= -1f;
        }

        // =====================================================
        // RODA ESQUERDA
        // =====================================================

        Quaternion rotacaoEsquerda =
            Quaternion.Euler(
                0f,
                angulo,
                0f
            );

        rodaFrenteEsquerda.localRotation =
            Quaternion.Slerp(
                rodaFrenteEsquerda.localRotation,
                rotacaoEsquerda,
                velocidadeRodas *
                Time.fixedDeltaTime
            );

        // =====================================================
        // RODA DIREITA
        // =====================================================

        Quaternion rotacaoDireita =
            Quaternion.Euler(
                0f,
                angulo,
                0f
            );

        rodaFrenteDireita.localRotation =
            Quaternion.Slerp(
                rodaFrenteDireita.localRotation,
                rotacaoDireita,
                velocidadeRodas *
                Time.fixedDeltaTime
            );
    }

    // =========================================================
    // ADERÊNCIA
    // =========================================================

    void AplicarAderencia()
    {
        Vector3 velocidadeLocal =
            transform.InverseTransformDirection(
                rb.linearVelocity
            );

        // Diminui o deslizamento lateral
        velocidadeLocal.x *=
            1f / aderencia;

        rb.linearVelocity =
            transform.TransformDirection(
                velocidadeLocal
            );
    }

    // =========================================================
    // CÂMERA ESTILO GTA
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

        // =====================================================
        // MOUSE
        // =====================================================

        Vector2 mouse =
            Mouse.current.delta.ReadValue();

        float mouseX =
            mouse.x *
            sensibilidadeMouseX *
            Time.deltaTime;

        float mouseY =
            mouse.y *
            sensibilidadeMouseY *
            Time.deltaTime;

        // =====================================================
        // ROTAÇÃO HORIZONTAL
        // =====================================================

        rotacaoCameraX += mouseX;

        // =====================================================
        // ROTAÇÃO VERTICAL
        // =====================================================

        if (inverterMouseY)
        {
            rotacaoCameraY += mouseY;
        }
        else
        {
            rotacaoCameraY -= mouseY;
        }

        // Limita câmera para não virar demais
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

        float scrollY =
            scroll.y * 0.01f;

        distanciaAtual -= scrollY;

        distanciaAtual =
            Mathf.Clamp(
                distanciaAtual,
                distanciaMinima,
                distanciaMaxima
            );

        // =====================================================
        // ROTAÇÃO DA CÂMERA
        // =====================================================

        Quaternion rotacao =
            Quaternion.Euler(
                rotacaoCameraY,
                rotacaoCameraX,
                0f
            );

        // =====================================================
        // POSIÇÃO
        // =====================================================

        Vector3 posicaoDesejada =
            alvoCamera.position +
            rotacao *
            new Vector3(
                0f,
                0f,
                -distanciaAtual
            );

        posicaoDesejada +=
            Vector3.up *
            alturaAtual;

        // =====================================================
        // SEGUE O CARRO
        // =====================================================

        cameraCarro.transform.position =
            Vector3.Lerp(
                cameraCarro.transform.position,
                posicaoDesejada,
                suavidadeCamera *
                Time.deltaTime
            );

        // =====================================================
        // OLHA PARA O CARRO
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

            cameraCarro.transform.rotation =
                Quaternion.Slerp(
                    cameraCarro.transform.rotation,
                    rotacaoDesejada,
                    suavidadeRotacaoCamera *
                    Time.deltaTime
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

        float velocidadeNormalizada =
            Mathf.Clamp01(
                velocidade /
                velocidadeMaxima
            );

        somMotor.pitch =
            Mathf.Lerp(
                pitchMinimo,
                pitchMaximo,
                velocidadeNormalizada
            );

        somMotor.volume =
            volumeMotor;
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    void OnDrawGizmosSelected()
    {
        // =====================================================
        // ENTRADA
        // =====================================================

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

        // =====================================================
        // SAÍDA
        // =====================================================

        if (pontoSaida != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(
                pontoSaida.position,
                0.3f
            );
        }

        // =====================================================
        // FRENTE DO CARRO
        // =====================================================

        Vector3 frente;

        if (frenteNoEixoX)
        {
            frente = transform.right;
        }
        else
        {
            frente = transform.forward;
        }

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            frente * 3f
        );
    }
}