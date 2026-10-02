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

    [Tooltip("Freio motor: quanto o carro perde de velocidade (m/s por segundo) quando você solta W e S.")]
    public float desaceleracao = 6f;

    // =========================================================
    // DIREÇÃO DO CARRO
    // =========================================================

    [Header("========== DIREÇÃO ==========")]

    [Tooltip("Ângulo máximo do volante (graus).")]
    public float anguloMaximo = 32f;

    [Tooltip("Distância entre o eixo da frente e o de trás. Menor = curva mais fechada.")]
    public float distanciaEixos = 2.6f;

    [Tooltip("Velocidade com que o volante vira e volta ao centro.")]
    public float velocidadeVolante = 5f;

    [Tooltip("Quanto o carro perde de velocidade (m/s por segundo) ao virar o volante enquanto anda. 0 = não perde.")]
    public float perdaVelocidadeCurva = 8f;

    // =========================================================
    // ADERÊNCIA
    // =========================================================

    [Header("========== ADERÊNCIA ==========")]

    [Tooltip("Quanto maior, menos o carro derrapa de lado. Ele sempre segue para onde as rodas apontam.")]
    public float aderencia = 10f;

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

    [Header("========== CÂMERA ==========")]

    [Tooltip("Câmera do carro. Deixe como FILHA do carro e posicione ela no Inspector. O script só liga, desliga e gira ela com o mouse.")]
    public Camera cameraCarro;

    [Tooltip("Sensibilidade horizontal do mouse.")]
    public float sensibilidadeMouseX = 0.1f;

    [Tooltip("Sensibilidade vertical do mouse.")]
    public float sensibilidadeMouseY = 0.07f;

    [Tooltip("Quanto a câmera pode girar para os lados (graus).")]
    public float limiteHorizontal = 180f;

    [Tooltip("Quanto a câmera pode olhar para baixo (graus).")]
    public float limiteBaixo = 20f;

    [Tooltip("Quanto a câmera pode olhar para cima (graus).")]
    public float limiteCima = 60f;

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
    // BUZINA E SIRENE
    // =========================================================

    [Header("========== BUZINA (H) ==========")]

    [Tooltip("AudioSource da buzina.")]
    public AudioSource somBuzina;

    [Tooltip("Som da buzina. Toca enquanto você segura H.")]
    public AudioClip clipeBuzina;

    [Header("========== SIRENE (T) ==========")]

    [Tooltip("AudioSource da sirene.")]
    public AudioSource somSirene;

    [Tooltip("Som da sirene. T liga e desliga.")]
    public AudioClip clipeSirene;

    [Tooltip("Objetos das luzes da sirene (opcional). São ligados e desligados junto com a sirene.")]
    public GameObject[] luzesSirene;

    private bool sireneLigada = false;

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

    private float direcaoSuave = 0f;

    // Câmera
    private Quaternion rotacaoInicialCamera = Quaternion.identity;

    private float giroHorizontal = 0f;

    private float giroVertical = 0f;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player != null)
        {
            playerController =
                player.GetComponent<CharacterController>();
        }

        // Rigidbody
        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        // O carro só pode girar no eixo Y
        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        // Câmera: guarda a rotação que você deixou no Inspector
        // e começa DESLIGADA
        if (cameraCarro != null)
        {
            rotacaoInicialCamera =
                cameraCarro.transform.localRotation;

            cameraCarro.gameObject.SetActive(false);
        }

        // Som do motor
        if (somMotor != null)
        {
            somMotor.loop = true;

            somMotor.playOnAwake = false;

            somMotor.volume = volumeMotor;
        }

        // Buzina
        if (somBuzina != null)
        {
            somBuzina.clip = clipeBuzina;

            somBuzina.loop = true;

            somBuzina.playOnAwake = false;
        }

        // Sirene
        if (somSirene != null)
        {
            somSirene.clip = clipeSirene;

            somSirene.loop = true;

            somSirene.playOnAwake = false;
        }

        DefinirLuzesSirene(false);

        // Cursor
        if (esconderCursor)
        {
            Cursor.lockState = CursorLockMode.None;

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

        if (!jogadorDentro)
        {
            VerificarEntrada();
        }
        else
        {
            LerControles();

            GirarCamera();

            LerBuzinaESirene();

            // E = sair
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

        AplicarMotor();

        AplicarAderencia();

        AplicarDirecao();

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

    Vector3 GetDirecaoFrenteHorizontal()
    {
        Vector3 f = GetDirecaoFrente();

        f.y = 0f;

        return f.normalized;
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

        // Desativa o controle do player
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Coloca o player no carro
        if (player != null && pontoEntrada != null)
        {
            player.transform.position =
                pontoEntrada.position;

            player.transform.rotation =
                pontoEntrada.rotation;
        }

        // Esconde o player
        if (player != null)
        {
            player.SetActive(false);
        }

        // Liga a câmera na posição que está no Inspector
        if (cameraCarro != null)
        {
            giroHorizontal = 0f;

            giroVertical = 0f;

            cameraCarro.transform.localRotation =
                rotacaoInicialCamera;

            cameraCarro.gameObject.SetActive(true);
        }

        // Cursor
        if (esconderCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;

            Cursor.visible = false;
        }

        // Som da porta
        if (somPorta != null &&
            somPortaFechando != null)
        {
            somPorta.PlayOneShot(somPortaFechando);
        }

        // Som do motor
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

        // Desliga a câmera
        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(false);
        }

        // Player
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

        // Cursor
        if (esconderCursor)
        {
            Cursor.lockState = CursorLockMode.None;

            Cursor.visible = true;
        }

        // Som da porta
        if (somPorta != null &&
            somPortaAbrindo != null)
        {
            somPorta.PlayOneShot(somPortaAbrindo);
        }

        // Desliga o motor
        if (somMotor != null)
        {
            somMotor.Stop();
        }

        // Para a buzina (a sirene continua como estiver)
        if (somBuzina != null && somBuzina.isPlaying)
        {
            somBuzina.Stop();
        }

        // Carro para ao sair
        rb.linearVelocity = new Vector3(
            0f,
            rb.linearVelocity.y,
            0f
        );

        rb.angularVelocity = Vector3.zero;

        aceleracao = 0f;

        direcao = 0f;

        direcaoSuave = 0f;
    }

    // =========================================================
    // CONTROLES (W A S D)
    // =========================================================

    void LerControles()
    {
        aceleracao = 0f;

        direcao = 0f;

        // W = frente
        if (Keyboard.current.wKey.isPressed)
        {
            aceleracao = 1f;
        }

        // S = freio / ré
        if (Keyboard.current.sKey.isPressed)
        {
            aceleracao = -1f;
        }

        // A = esquerda
        if (Keyboard.current.aKey.isPressed)
        {
            direcao = -1f;
        }

        // D = direita
        if (Keyboard.current.dKey.isPressed)
        {
            direcao = 1f;
        }
    }

    // =========================================================
    // BUZINA (H) E SIRENE (T)
    // =========================================================

    void LerBuzinaESirene()
    {
        // H = buzina (toca enquanto segura)
        if (somBuzina != null && clipeBuzina != null)
        {
            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                somBuzina.Play();
            }

            if (Keyboard.current.hKey.wasReleasedThisFrame)
            {
                somBuzina.Stop();
            }
        }

        // T = liga / desliga a sirene
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            sireneLigada = !sireneLigada;

            if (somSirene != null && clipeSirene != null)
            {
                if (sireneLigada)
                {
                    somSirene.Play();
                }
                else
                {
                    somSirene.Stop();
                }
            }

            DefinirLuzesSirene(sireneLigada);
        }
    }

    void DefinirLuzesSirene(bool ligado)
    {
        if (luzesSirene == null)
            return;

        foreach (GameObject luz in luzesSirene)
        {
            if (luz != null)
            {
                luz.SetActive(ligado);
            }
        }
    }

    // =========================================================
    // CÂMERA: SÓ GIRA COM O MOUSE
    // (a posição é a do Inspector, a câmera é filha do carro)
    // =========================================================

    void GirarCamera()
    {
        if (cameraCarro == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouse =
            Mouse.current.delta.ReadValue();

        // Horizontal
        giroHorizontal +=
            mouse.x * sensibilidadeMouseX;

        giroHorizontal =
            Mathf.Clamp(
                giroHorizontal,
                -limiteHorizontal,
                limiteHorizontal
            );

        // Vertical (positivo = olhar para baixo)
        if (inverterMouseY)
        {
            giroVertical += mouse.y * sensibilidadeMouseY;
        }
        else
        {
            giroVertical -= mouse.y * sensibilidadeMouseY;
        }

        giroVertical =
            Mathf.Clamp(
                giroVertical,
                -limiteCima,
                limiteBaixo
            );

        cameraCarro.transform.localRotation =
            rotacaoInicialCamera *
            Quaternion.Euler(
                giroVertical,
                giroHorizontal,
                0f
            );
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

        // W = frente
        if (aceleracao > 0f)
        {
            if (velocidadeFrente < velocidadeMaxima)
            {
                rb.AddForce(
                    frente * forcaMotor,
                    ForceMode.Force
                );
            }

            return;
        }

        // S = freio / ré
        if (aceleracao < 0f)
        {
            // Se está andando para frente, primeiro freia
            if (velocidadeFrente > 0.5f)
            {
                rb.AddForce(
                    -frente * forcaFreio,
                    ForceMode.Force
                );
            }
            else
            {
                // Depois começa a dar ré
                float velocidadeReAtual =
                    Vector3.Dot(
                        rb.linearVelocity,
                        -frente
                    );

                if (velocidadeReAtual < velocidadeRe)
                {
                    rb.AddForce(
                        -frente * (forcaMotor * 0.5f),
                        ForceMode.Force
                    );
                }
            }

            return;
        }

        // Soltou W e S -> freio motor (perde velocidade aos poucos)
        Vector3 vel = rb.linearVelocity;

        Vector3 horizontal =
            new Vector3(vel.x, 0f, vel.z);

        horizontal =
            Vector3.MoveTowards(
                horizontal,
                Vector3.zero,
                desaceleracao * Time.fixedDeltaTime
            );

        rb.linearVelocity =
            new Vector3(
                horizontal.x,
                vel.y,   // mantém a gravidade
                horizontal.z
            );
    }

    // =========================================================
    // DIREÇÃO DO CARRO
    // =========================================================

    void AplicarAderencia()
    {
        Vector3 frente = GetDirecaoFrenteHorizontal();

        Vector3 lado = Vector3.Cross(Vector3.up, frente);

        Vector3 vel = rb.linearVelocity;

        Vector3 horizontal =
            new Vector3(vel.x, 0f, vel.z);

        float vFrente = Vector3.Dot(horizontal, frente);

        float vLado = Vector3.Dot(horizontal, lado);

        // Os pneus "seguram" o carro: a velocidade lateral some
        // e o carro passa a andar para onde está apontando.
        float t = 1f - Mathf.Exp(-aderencia * Time.fixedDeltaTime);

        vLado = Mathf.Lerp(vLado, 0f, t);

        Vector3 nova = frente * vFrente + lado * vLado;

        rb.linearVelocity =
            new Vector3(nova.x, vel.y, nova.z);
    }

    // =========================================================
    // DIREÇÃO DO CARRO (curva depende da velocidade, como na vida real)
    // =========================================================

    void AplicarDirecao()
    {
        // Volante vira aos poucos
        direcaoSuave =
            Mathf.MoveTowards(
                direcaoSuave,
                direcao,
                velocidadeVolante * Time.fixedDeltaTime
            );

        // Evita giro estranho por colisões
        rb.angularVelocity = Vector3.zero;

        Vector3 frente = GetDirecaoFrenteHorizontal();

        float velocidadeFrente =
            Vector3.Dot(rb.linearVelocity, frente);

        // Parado = não gira
        if (Mathf.Abs(velocidadeFrente) < 0.1f)
            return;

        // Em alta velocidade o volante abre menos
        float percentual =
            Mathf.Clamp01(
                Mathf.Abs(velocidadeFrente) / velocidadeMaxima
            );

        float fator = Mathf.Lerp(1f, 0.35f, percentual);

        float anguloRad =
            direcaoSuave * anguloMaximo * fator * Mathf.Deg2Rad;

        // Modelo de bicicleta: giro = velocidade * tan(volante) / distância dos eixos
        // Na ré a velocidade é negativa, então a curva inverte sozinha.
        float giro =
            velocidadeFrente * Mathf.Tan(anguloRad) / distanciaEixos;

        float graus =
            giro * Mathf.Rad2Deg * Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation * Quaternion.Euler(0f, graus, 0f)
        );

        // Virando o volante enquanto anda = o carro desacelera
        Vector3 vel = rb.linearVelocity;

        Vector3 horizontal =
            new Vector3(vel.x, 0f, vel.z);

        float perda =
            Mathf.Abs(direcaoSuave) *
            perdaVelocidadeCurva *
            Time.fixedDeltaTime;

        horizontal =
            Vector3.MoveTowards(
                horizontal,
                Vector3.zero,
                perda
            );

        rb.linearVelocity =
            new Vector3(
                horizontal.x,
                vel.y,   // mantém a gravidade
                horizontal.z
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

        float percentual =
            Mathf.Clamp01(
                rb.linearVelocity.magnitude / velocidadeMaxima
            );

        float fator = Mathf.Lerp(1f, 0.35f, percentual);

        float angulo =
            direcaoSuave * anguloMaximoRodas * fator;

        Quaternion rotacaoRodas =
            Quaternion.Euler(0f, angulo, 0f);

        rodaFrenteEsquerda.localRotation =
            Quaternion.Slerp(
                rodaFrenteEsquerda.localRotation,
                rotacaoRodas,
                velocidadeRodas * Time.fixedDeltaTime
            );

        rodaFrenteDireita.localRotation =
            Quaternion.Slerp(
                rodaFrenteDireita.localRotation,
                rotacaoRodas,
                velocidadeRodas * Time.fixedDeltaTime
            );
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
                velocidade / velocidadeMaxima
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
        // Entrada
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

        // Saída
        if (pontoSaida != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(
                pontoSaida.position,
                0.3f
            );
        }

        // Frente do carro
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