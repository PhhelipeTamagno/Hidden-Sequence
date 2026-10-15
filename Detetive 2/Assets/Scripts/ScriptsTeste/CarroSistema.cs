using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarroSistema : MonoBehaviour
{
    [Header("========== MOVIMENTO ==========")]

    public float velocidadeMaxima = 25f;
    public float velocidadeRe = 8f;
    public float forcaMotor = 9000f;

    [Header("========== FRENAGEM ==========")]

    public float forcaFreio = 15000f;
    public float desaceleracao = 6f;

    [Header("========== DIREÇÃO ==========")]

    public float anguloMaximo = 32f;
    public float distanciaEixos = 2.6f;
    public float velocidadeVolante = 5f;
    public float perdaVelocidadeCurva = 8f;

    [Header("========== ADERÊNCIA ==========")]

    public float aderencia = 10f;

    [Header("========== SENSORES ==========")]

    public SensorDirecaoCarro sensorFrente;
    public SensorDirecaoCarro sensorTras;
    public SensorDirecaoCarro sensorEsquerda;
    public SensorDirecaoCarro sensorDireita;

    [Header("========== PLAYER ==========")]

    public GameObject player;
    public Transform pontoEntrada;
    public Transform pontoSaida;
    public float distanciaEntrada = 3f;

    [Header("========== CÂMERA ==========")]

    public Camera cameraCarro;
    public float sensibilidadeMouseX = 0.1f;
    public float sensibilidadeMouseY = 0.07f;
    public float limiteHorizontal = 180f;
    public float limiteBaixo = 20f;
    public float limiteCima = 60f;
    public bool inverterMouseY = false;
    public bool esconderCursor = true;

    [Header("========== RODAS DIANTEIRAS ==========")]

    public Transform rodaFrenteEsquerda;
    public Transform rodaFrenteDireita;
    public float anguloMaximoRodas = 32f;
    public float velocidadeRodas = 8f;

    [Header("========== ÁUDIO ==========")]

    public AudioSource somMotor;
    public AudioSource somPorta;
    public AudioClip somPortaAbrindo;
    public AudioClip somPortaFechando;

    [Header("========== BUZINA (H) ==========")]

    public AudioSource somBuzina;
    public AudioClip clipeBuzina;

    [Header("========== SIRENE (T) ==========")]

    public AudioSource somSirene;
    public AudioClip clipeSirene;
    public GameObject[] luzesSirene;

    private bool sireneLigada = false;

    [Header("========== SOM DO MOTOR ==========")]

    public float pitchMinimo = 0.7f;
    public float pitchMaximo = 1.8f;

    [Range(0f, 1f)]
    public float volumeMotor = 1f;

    [Header("========== ORIENTAÇÃO ==========")]

    public bool frenteNoEixoX = true;

    private Rigidbody rb;
    private CharacterController playerController;

    private bool jogadorDentro = false;

    private float aceleracao = 0f;
    private float direcao = 0f;
    private float direcaoSuave = 0f;

    private Quaternion rotacaoInicialCamera = Quaternion.identity;

    private float giroHorizontal = 0f;
    private float giroVertical = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player != null)
        {
            playerController =
                player.GetComponent<CharacterController>();
        }

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        if (cameraCarro != null)
        {
            rotacaoInicialCamera =
                cameraCarro.transform.localRotation;

            cameraCarro.gameObject.SetActive(false);
        }

        if (somMotor != null)
        {
            somMotor.loop = true;
            somMotor.playOnAwake = false;
            somMotor.volume = volumeMotor;
        }

        if (somBuzina != null)
        {
            somBuzina.clip = clipeBuzina;
            somBuzina.loop = true;
            somBuzina.playOnAwake = false;
        }

        if (somSirene != null)
        {
            somSirene.clip = clipeSirene;
            somSirene.loop = true;
            somSirene.playOnAwake = false;
        }

        DefinirLuzesSirene(false);

        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
    }

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

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                SairDoCarro();
            }
        }

        AtualizarSomMotor();
    }

    void FixedUpdate()
    {
        if (!jogadorDentro)
            return;

        AplicarMotor();

        AplicarAderencia();

        AplicarDirecao();

        AtualizarRodasDianteiras();
    }

    Vector3 GetDirecaoFrente()
    {
        if (frenteNoEixoX)
            return transform.right;

        return transform.forward;
    }

    Vector3 GetDirecaoFrenteHorizontal()
    {
        Vector3 frente =
            GetDirecaoFrente();

        frente.y = 0f;

        return frente.normalized;
    }

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

    void EntrarNoCarro()
    {
        if (jogadorDentro)
            return;

        jogadorDentro = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (player != null &&
            pontoEntrada != null)
        {
            player.transform.position =
                pontoEntrada.position;

            player.transform.rotation =
                pontoEntrada.rotation;
        }

        if (player != null)
        {
            player.SetActive(false);
        }

        if (cameraCarro != null)
        {
            giroHorizontal = 0f;
            giroVertical = 0f;

            cameraCarro.transform.localRotation =
                rotacaoInicialCamera;

            cameraCarro.gameObject.SetActive(true);
        }

        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }

        if (somPorta != null &&
            somPortaFechando != null)
        {
            somPorta.PlayOneShot(
                somPortaFechando
            );
        }

        if (somMotor != null)
        {
            somMotor.Play();
        }
    }

    void SairDoCarro()
    {
        if (!jogadorDentro)
            return;

        jogadorDentro = false;

        if (cameraCarro != null)
        {
            cameraCarro.gameObject.SetActive(false);
        }

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

        if (esconderCursor)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }

        if (somPorta != null &&
            somPortaAbrindo != null)
        {
            somPorta.PlayOneShot(
                somPortaAbrindo
            );
        }

        if (somMotor != null)
        {
            somMotor.Stop();
        }

        if (somBuzina != null &&
            somBuzina.isPlaying)
        {
            somBuzina.Stop();
        }

        rb.linearVelocity =
            new Vector3(
                0f,
                rb.linearVelocity.y,
                0f
            );

        rb.angularVelocity =
            Vector3.zero;

        aceleracao = 0f;
        direcao = 0f;
        direcaoSuave = 0f;
    }

    void LerControles()
    {
        aceleracao = 0f;
        direcao = 0f;

        bool w =
            Keyboard.current.wKey.isPressed;

        bool s =
            Keyboard.current.sKey.isPressed;

        bool a =
            Keyboard.current.aKey.isPressed;

        bool d =
            Keyboard.current.dKey.isPressed;

        if (w &&
            !FrenteBloqueada())
        {
            aceleracao = 1f;
        }

        if (s &&
            !TrasBloqueado())
        {
            aceleracao = -1f;
        }

        if (a &&
            !EsquerdaBloqueada())
        {
            direcao = -1f;
        }

        if (d &&
            !DireitaBloqueada())
        {
            direcao = 1f;
        }
    }

    bool FrenteBloqueada()
    {
        if (sensorFrente == null)
            return false;

        return sensorFrente.Bloqueado;
    }

    bool TrasBloqueado()
    {
        if (sensorTras == null)
            return false;

        return sensorTras.Bloqueado;
    }

    bool EsquerdaBloqueada()
    {
        if (sensorEsquerda == null)
            return false;

        return sensorEsquerda.Bloqueado;
    }

    bool DireitaBloqueada()
    {
        if (sensorDireita == null)
            return false;

        return sensorDireita.Bloqueado;
    }

    void LerBuzinaESirene()
    {
        if (somBuzina != null &&
            clipeBuzina != null)
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

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            sireneLigada =
                !sireneLigada;

            if (somSirene != null &&
                clipeSirene != null)
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

            DefinirLuzesSirene(
                sireneLigada
            );
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

    void GirarCamera()
    {
        if (cameraCarro == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mouse =
            Mouse.current.delta.ReadValue();

        giroHorizontal +=
            mouse.x *
            sensibilidadeMouseX;

        giroHorizontal =
            Mathf.Clamp(
                giroHorizontal,
                -limiteHorizontal,
                limiteHorizontal
            );

        if (inverterMouseY)
        {
            giroVertical +=
                mouse.y *
                sensibilidadeMouseY;
        }
        else
        {
            giroVertical -=
                mouse.y *
                sensibilidadeMouseY;
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

    void AplicarMotor()
    {
        Vector3 frente =
            GetDirecaoFrente();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        if (aceleracao > 0f)
        {
            if (FrenteBloqueada())
            {
                if (velocidadeFrente > 0f)
                {
                    Vector3 velocidade =
                        rb.linearVelocity;

                    velocidade -=
                        frente *
                        velocidadeFrente;

                    rb.linearVelocity =
                        new Vector3(
                            velocidade.x,
                            rb.linearVelocity.y,
                            velocidade.z
                        );
                }

                return;
            }

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

        if (aceleracao < 0f)
        {
            if (TrasBloqueado())
            {
                if (velocidadeFrente < 0f)
                {
                    Vector3 velocidade =
                        rb.linearVelocity;

                    velocidade -=
                        frente *
                        velocidadeFrente;

                    rb.linearVelocity =
                        new Vector3(
                            velocidade.x,
                            rb.linearVelocity.y,
                            velocidade.z
                        );
                }

                return;
            }

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

        Vector3 vel =
            rb.linearVelocity;

        Vector3 horizontal =
            new Vector3(
                vel.x,
                0f,
                vel.z
            );

        horizontal =
            Vector3.MoveTowards(
                horizontal,
                Vector3.zero,
                desaceleracao *
                Time.fixedDeltaTime
            );

        rb.linearVelocity =
            new Vector3(
                horizontal.x,
                vel.y,
                horizontal.z
            );
    }

    void AplicarAderencia()
    {
        Vector3 frente =
            GetDirecaoFrenteHorizontal();

        Vector3 lado =
            Vector3.Cross(
                Vector3.up,
                frente
            );

        Vector3 vel =
            rb.linearVelocity;

        Vector3 horizontal =
            new Vector3(
                vel.x,
                0f,
                vel.z
            );

        float vFrente =
            Vector3.Dot(
                horizontal,
                frente
            );

        float vLado =
            Vector3.Dot(
                horizontal,
                lado
            );

        float t =
            1f -
            Mathf.Exp(
                -aderencia *
                Time.fixedDeltaTime
            );

        vLado =
            Mathf.Lerp(
                vLado,
                0f,
                t
            );

        Vector3 nova =
            frente *
            vFrente +
            lado *
            vLado;

        rb.linearVelocity =
            new Vector3(
                nova.x,
                vel.y,
                nova.z
            );
    }

    void AplicarDirecao()
    {
        direcaoSuave =
            Mathf.MoveTowards(
                direcaoSuave,
                direcao,
                velocidadeVolante *
                Time.fixedDeltaTime
            );

        rb.angularVelocity =
            Vector3.zero;

        Vector3 frente =
            GetDirecaoFrenteHorizontal();

        float velocidadeFrente =
            Vector3.Dot(
                rb.linearVelocity,
                frente
            );

        if (Mathf.Abs(
            velocidadeFrente
        ) < 0.1f)
        {
            return;
        }

        if (direcaoSuave < 0f &&
            EsquerdaBloqueada())
        {
            direcaoSuave = 0f;
        }

        if (direcaoSuave > 0f &&
            DireitaBloqueada())
        {
            direcaoSuave = 0f;
        }

        float percentual =
            Mathf.Clamp01(
                Mathf.Abs(
                    velocidadeFrente
                ) /
                velocidadeMaxima
            );

        float fator =
            Mathf.Lerp(
                1f,
                0.35f,
                percentual
            );

        float anguloRad =
            direcaoSuave *
            anguloMaximo *
            fator *
            Mathf.Deg2Rad;

        float giro =
            velocidadeFrente *
            Mathf.Tan(anguloRad) /
            distanciaEixos;

        float graus =
            giro *
            Mathf.Rad2Deg *
            Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(
                0f,
                graus,
                0f
            )
        );

        Vector3 vel =
            rb.linearVelocity;

        Vector3 horizontal =
            new Vector3(
                vel.x,
                0f,
                vel.z
            );

        float perda =
            Mathf.Abs(
                direcaoSuave
            ) *
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
                vel.y,
                horizontal.z
            );
    }

    void AtualizarRodasDianteiras()
    {
        if (rodaFrenteEsquerda == null ||
            rodaFrenteDireita == null)
        {
            return;
        }

        float percentual =
            Mathf.Clamp01(
                rb.linearVelocity.magnitude /
                velocidadeMaxima
            );

        float fator =
            Mathf.Lerp(
                1f,
                0.35f,
                percentual
            );

        float angulo =
            direcaoSuave *
            anguloMaximoRodas *
            fator;

        Quaternion rotacaoRodas =
            Quaternion.Euler(
                0f,
                angulo,
                0f
            );

        rodaFrenteEsquerda.localRotation =
            Quaternion.Slerp(
                rodaFrenteEsquerda.localRotation,
                rotacaoRodas,
                velocidadeRodas *
                Time.fixedDeltaTime
            );

        rodaFrenteDireita.localRotation =
            Quaternion.Slerp(
                rodaFrenteDireita.localRotation,
                rotacaoRodas,
                velocidadeRodas *
                Time.fixedDeltaTime
            );
    }

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

    void OnDrawGizmosSelected()
    {
        if (pontoEntrada != null)
        {
            Gizmos.color =
                Color.green;

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
            Gizmos.color =
                Color.red;

            Gizmos.DrawSphere(
                pontoSaida.position,
                0.3f
            );
        }

        Vector3 frente;

        if (frenteNoEixoX)
        {
            frente =
                transform.right;
        }
        else
        {
            frente =
                transform.forward;
        }

        Gizmos.color =
            Color.blue;

        Gizmos.DrawLine(
            transform.position,
            transform.position +
            frente * 3f
        );
    }
}