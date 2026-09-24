using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 0f;

    [Tooltip("Velocidade que o jogador terá quando a investigação for liberada.")]
    public float velocidadeAposInvestigacao = 3f;

    [Header("Gravidade")]
    public float gravidade = -9.81f;
    public float velocidadeQueda = -2f;

    [Header("Pulo")]
    public float forcaPulo = 5f;

    [Header("Mouse")]
    public float sensibilidadeMouse = 100f;
    public Transform cameraTransform;

    private CharacterController controller;

    public Animator animator;
    public TMP_InputField campoTexto;

    bool mouseTravado = true;

    private PcInteragir ultimoPC;

    [Header("UI de interação")]
    public PromptInteracaoUI promptInteracao;

    public bool usandoComputador = false;
    private ComputadorInteract computadorEmUso;

    float rotacaoX = 0f;
    float velocidadeY = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        if (usandoComputador)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (computadorEmUso != null)
                    computadorEmUso.SairComputador(this);
            }

            return;
        }

        if (!mouseTravado)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                mouseTravado = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            return;
        }

        float mouseX =
            Input.GetAxis("Mouse X") *
            sensibilidadeMouse *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            sensibilidadeMouse *
            Time.deltaTime;

        rotacaoX -= mouseY;

        rotacaoX =
            Mathf.Clamp(
                rotacaoX,
                -90f,
                90f
            );

        cameraTransform.localRotation =
            Quaternion.Euler(
                rotacaoX,
                0f,
                0f
            );

        transform.Rotate(
            Vector3.up * mouseX
        );

        float horizontal =
            Input.GetAxisRaw("Horizontal");

        float vertical =
            Input.GetAxisRaw("Vertical");

        Vector3 movimento =
            transform.right * horizontal +
            transform.forward * vertical;

        movimento =
            movimento.normalized *
            velocidade;

        if (controller.isGrounded)
        {
            if (velocidadeY < 0)
                velocidadeY = velocidadeQueda;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocidadeY = forcaPulo;
            }
        }
        else
        {
            velocidadeY +=
                gravidade *
                Time.deltaTime;
        }

        movimento.y = velocidadeY;

        controller.Move(
            movimento *
            Time.deltaTime
        );
        bool andando =
            horizontal != 0 ||
            vertical != 0;

        if (animator != null)
        {
            animator.SetBool(
                "isWalking",
                andando
            );
        }

        Ray ray =
            new Ray(
                cameraTransform.position,
                cameraTransform.forward
            );

        RaycastHit hit;

        if (
            Physics.Raycast(
                ray,
                out hit,
                Mathf.Infinity
            )
        )
        {
            PcInteragir pc =
                hit.collider.GetComponentInParent<PcInteragir>();

            if (pc != null)
            {
                if (ultimoPC != pc)
                {
                    if (ultimoPC != null)
                        ultimoPC.RemoverDestaque();

                    pc.Destacar();

                    ultimoPC = pc;

                    if (promptInteracao != null)
                        promptInteracao.Mostrar();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log(
                        "Tecla E pressionada, objeto acertado: " +
                        hit.collider.gameObject.name
                    );

                    ComputadorInteract comp =
                        hit.collider.GetComponentInParent<ComputadorInteract>();

                    if (comp != null)
                    {
                        Debug.Log(
                            "ComputadorInteract encontrado em: " +
                            comp.gameObject.name
                        );

                        computadorEmUso = comp;

                        comp.UsarComputador(this);

                        if (promptInteracao != null)
                            promptInteracao.Esconder();
                    }
                    else
                    {
                        Debug.LogWarning(
                            "Nenhum ComputadorInteract encontrado no objeto ou nos pais de: " +
                            hit.collider.gameObject.name
                        );
                    }
                }
            }
            else
            {
                if (ultimoPC != null)
                {
                    ultimoPC.RemoverDestaque();

                    ultimoPC = null;

                    if (promptInteracao != null)
                        promptInteracao.Esconder();
                }
            }
        }
        else
        {
            if (ultimoPC != null)
            {
                ultimoPC.RemoverDestaque();

                ultimoPC = null;

                if (promptInteracao != null)
                    promptInteracao.Esconder();
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            mouseTravado = !mouseTravado;

            if (mouseTravado)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (campoTexto != null)
                {
                    campoTexto.ActivateInputField();
                    campoTexto.Select();
                }
            }
        }
    }
}