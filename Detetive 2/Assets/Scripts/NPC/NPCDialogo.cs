using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Fala
{
    [TextArea(2, 5)]
    public string texto;

    [Tooltip("Quanto tempo a fala fica na tela")]
    public float duracao = 2.5f;

    [Tooltip("Deixe vazio para usar a animação padrão de falar")]
    public string animacao = "";
}

[System.Serializable]
public class EtapaDialogo
{
    public string nome = "Etapa";
    public List<Fala> falas = new List<Fala>();
}

public class NPCDialogo : MonoBehaviour
{
    [Header("Texto")]
    public TMP_Text textoDialogo;

    [Tooltip("Objeto vazio filho da NPC, posicionado acima da cabeça")]
    public Transform pontoTexto;

    [Tooltip("Ajuste fino em cima do Ponto Texto")]
    public Vector3 deslocamentoTexto = new Vector3(0f, 0.5f, 0f);

    [Tooltip("Faz o texto sempre virar para a câmera (útil em World Space)")]
    public bool textoOlhaParaCamera = true;

    [Header("Diálogo (cada clique avança uma etapa)")]
    public List<EtapaDialogo> etapas = new List<EtapaDialogo>();

    [Tooltip("Se marcado, clicar depois do fim repete a última etapa")]
    public bool repetirUltimaEtapa = false;

    [Header("Interação")]
    public float distanciaMaxima = 3f;
    public Sprite imagemMouseEsquerdo;

    [Header("Animações")]
    public Animator animatorNPC;
    public string animacaoParado = "MexendoPC";
    public string animacaoFalando = "Falando";

    private Camera cameraPrincipal;
    private GameObject indicadorMouse;
    private int indiceEtapa = 0;
    private bool falando = false;
    private Coroutine rotina;

    private void Start()
    {
        cameraPrincipal = Camera.main;

        if (textoDialogo != null)
            textoDialogo.gameObject.SetActive(false);

        if (animatorNPC == null)
            animatorNPC = GetComponentInChildren<Animator>();

        CriarIndicadorMouse();
        TrocarAnimacao(animacaoParado);
    }

    private void Update()
    {
        if (cameraPrincipal == null)
            cameraPrincipal = Camera.main;

        if (cameraPrincipal == null)
            return;

        AtualizarPosicaoTexto();

        bool podeInteragir = !falando &&
            (indiceEtapa < etapas.Count ||
             (repetirUltimaEtapa && etapas.Count > 0));

        bool olhando = podeInteragir && EstaOlhandoParaMim();

        if (indicadorMouse != null)
            indicadorMouse.SetActive(olhando);

        if (olhando &&
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Interagir();
        }
    }

    private bool EstaOlhandoParaMim()
    {
        // Raio saindo do centro da tela (a mira)
        Ray raio = cameraPrincipal.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(raio, out RaycastHit hit, distanciaMaxima,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponentInParent<NPCDialogo>() == this;
        }

        return false;
    }

    private void Interagir()
    {
        int i = Mathf.Min(indiceEtapa, etapas.Count - 1);
        indiceEtapa++;

        if (rotina != null)
            StopCoroutine(rotina);

        rotina = StartCoroutine(ExecutarEtapa(etapas[i]));
    }

    private IEnumerator ExecutarEtapa(EtapaDialogo etapa)
    {
        falando = true;

        foreach (Fala fala in etapa.falas)
        {
            MostrarTexto(fala.texto);

            TrocarAnimacao(string.IsNullOrEmpty(fala.animacao)
                ? animacaoFalando
                : fala.animacao);

            yield return new WaitForSeconds(fala.duracao);
        }

        if (textoDialogo != null)
        {
            textoDialogo.text = "";
            textoDialogo.gameObject.SetActive(false);
        }

        TrocarAnimacao(animacaoParado);

        falando = false;
        rotina = null;
    }

    private void MostrarTexto(string mensagem)
    {
        if (textoDialogo == null)
            return;

        textoDialogo.gameObject.SetActive(true);
        textoDialogo.text = mensagem;
        AtualizarPosicaoTexto();
    }

    private void TrocarAnimacao(string nome)
    {
        if (animatorNPC == null || string.IsNullOrEmpty(nome))
            return;

        int hash = Animator.StringToHash(nome);

        if (animatorNPC.HasState(0, hash))
        {
            animatorNPC.speed = 1f;
            animatorNPC.CrossFade(hash, 0.15f);
        }
    }

    private void AtualizarPosicaoTexto()
    {
        if (textoDialogo == null || !textoDialogo.gameObject.activeSelf)
            return;

        Vector3 basePos = pontoTexto != null
            ? pontoTexto.position
            : transform.position;

        textoDialogo.transform.position = basePos + deslocamentoTexto;

        if (textoOlhaParaCamera && cameraPrincipal != null)
        {
            // Gira o texto para ficar de frente para a câmera
            textoDialogo.transform.rotation = cameraPrincipal.transform.rotation;
        }
    }

    private void CriarIndicadorMouse()
    {
        if (imagemMouseEsquerdo == null)
            return;

        GameObject canvasObj = new GameObject("CanvasIndicadorMouse");
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();

        indicadorMouse = new GameObject("IndicadorClique");
        indicadorMouse.transform.SetParent(canvasObj.transform, false);

        Image img = indicadorMouse.AddComponent<Image>();
        img.sprite = imagemMouseEsquerdo;
        img.raycastTarget = false;

        indicadorMouse.GetComponent<RectTransform>().sizeDelta =
            new Vector2(45f, 45f);

        indicadorMouse.SetActive(false);
    }

    private void OnDisable()
    {
        if (rotina != null)
            StopCoroutine(rotina);

        falando = false;

        if (animatorNPC != null)
            animatorNPC.speed = 1f;
    }
}