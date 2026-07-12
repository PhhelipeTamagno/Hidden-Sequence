using UnityEngine;
using System.Collections;

public class ComputadorInteract : MonoBehaviour
{
    public GameObject telaPC;
    public Transform cameraPC;
    public float velocidadeZoom = 2f;

    [Header("Mensagem ao abrir o pc")]
    public IntroUI introUI; // arraste o objeto que tem o script IntroUI
    public string mensagemAoAbrir = "Olhe o seu email";

    private Vector3 posOriginal;
    private Quaternion rotOriginal;
    private Transform camTransform;

    public void UsarComputador(PlayerMovement player)
    {
        Debug.Log("UsarComputador chamado. cameraPC atribuído? " + (cameraPC != null));

        if (cameraPC == null)
        {
            Debug.LogError("ComputadorInteract em " + gameObject.name + ": o campo 'Camera Pc' está vazio no Inspector!");
            return;
        }

        camTransform = player.cameraTransform;
        player.usandoComputador = true;

        posOriginal = camTransform.position;
        rotOriginal = camTransform.rotation;

        StartCoroutine(ZoomPC());
    }

    public void SairComputador(PlayerMovement player)
    {
        if (telaPC != null)
            telaPC.SetActive(false);

        StartCoroutine(SairZoom(player));
    }

    IEnumerator ZoomPC()
    {
        Vector3 posInicial = camTransform.position;
        Quaternion rotInicial = camTransform.rotation;
        float tempo = 0;

        while (tempo < 1)
        {
            tempo += Time.deltaTime * velocidadeZoom;
            camTransform.position = Vector3.Lerp(posInicial, cameraPC.position, tempo);
            camTransform.rotation = Quaternion.Slerp(rotInicial, cameraPC.rotation, tempo);
            yield return null;
        }

        camTransform.position = cameraPC.position;
        camTransform.rotation = cameraPC.rotation;

        if (telaPC != null)
            telaPC.SetActive(true);

        if (introUI != null)
            introUI.MostrarMensagem(mensagemAoAbrir);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator SairZoom(PlayerMovement player)
    {
        Vector3 posInicial = camTransform.position;
        Quaternion rotInicial = camTransform.rotation;
        float tempo = 0;

        while (tempo < 1)
        {
            tempo += Time.deltaTime * velocidadeZoom;
            camTransform.position = Vector3.Lerp(posInicial, posOriginal, tempo);
            camTransform.rotation = Quaternion.Slerp(rotInicial, rotOriginal, tempo);
            yield return null;
        }

        camTransform.position = posOriginal;
        camTransform.rotation = rotOriginal;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.usandoComputador = false;
    }
}