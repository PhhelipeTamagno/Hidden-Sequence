using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ComputadorInteract : MonoBehaviour
{
    public GameObject telaPC;
    public Transform cameraPC;
    public float velocidadeZoom = 4f;

    [Header("Mensagem ao abrir o pc")]
    public IntroUI introUI; // arraste o objeto que tem o script IntroUI
    public string mensagemAoAbrir = "Olhe o seu email";

    [System.Serializable]
    public class IconeTela
    {
        public string nome;
        public GameObject icone; // o ícone clicável (precisa de um Image com Raycast Target marcado)
        public GameObject tela;  // a tela/janela que abre quando clica nesse ícone
    }

    [Header("Telas do computador (email, lixo, files, web...)")]
    public IconeTela[] telasPC;

    [Header("Objetos sempre visíveis na tela do pc (ex: barra)")]
    public GameObject[] semprevisiveis;

    private Vector3 posOriginal;
    private Quaternion rotOriginal;
    private Transform camTransform;
    private bool dentroDoComputador = false;

    void Update()
    {
        if (dentroDoComputador && Input.GetMouseButtonDown(0))
        {
            VerificarCliqueIcone();
        }
    }

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

        FecharTodasTelas();
        dentroDoComputador = false;

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

        // começa sempre com a "área inicial" (desktop) e liga o que deve ficar sempre visível
        FecharTodasTelas();
        if (semprevisiveis != null)
        {
            foreach (var obj in semprevisiveis)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
        dentroDoComputador = true;

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

    // ---------- Lógica das telas/ícones ----------

    void VerificarCliqueIcone()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> resultados = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, resultados);

        foreach (var resultado in resultados)
        {
            foreach (var item in telasPC)
            {
                if (item.icone != null && resultado.gameObject == item.icone)
                {
                    AbrirTela(item.tela);
                    return;
                }
            }
        }
    }

    // Abre a tela passada e fecha as outras. Clicar de novo no mesmo ícone fecha (toggle).
    public void AbrirTela(GameObject telaAlvo)
    {
        if (telaAlvo == null) return;

        bool jaEstavaAberta = telaAlvo.activeSelf;

        foreach (var item in telasPC)
        {
            if (item.tela != null)
                item.tela.SetActive(false);
        }

        if (!jaEstavaAberta)
            telaAlvo.SetActive(true);
    }

    public void FecharTodasTelas()
    {
        foreach (var item in telasPC)
        {
            if (item.tela != null)
                item.tela.SetActive(false);
        }
    }
}