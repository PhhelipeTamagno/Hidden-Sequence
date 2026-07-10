using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movimento")]
    public float velocidade = 5f;

    [Header("Mouse")]
    public float sensibilidadeMouse = 100f;
    public Transform cameraTransform;
    private CharacterController controller;
    public Animator animator;
    public TMP_InputField campoTexto;
    bool mouseTravado = true;

    float rotacaoX = 0f;

    void Start()
{
    controller = GetComponent<CharacterController>();

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    void Update()
    {
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
        // Mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        rotacaoX -= mouseY;
        rotacaoX = Mathf.Clamp(rotacaoX, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movimento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movimento =
            transform.right * horizontal +
            transform.forward * vertical;

        controller.Move(movimento.normalized * velocidade * Time.deltaTime);

        bool andando = Input.GetAxisRaw("Horizontal") != 0 ||
               Input.GetAxisRaw("Vertical") != 0;

animator.SetBool("isWalking", andando);

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

        campoTexto.ActivateInputField();
        campoTexto.Select();
    }
}
    }
}