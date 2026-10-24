
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

public class CutsceneExecucao : MonoBehaviour
{
    [Header("Personagens")]
    public Animator assassino;
    public Animator vitima;

    [Header("Estados dos Animators")]
    public string animacaoAssassino = "Matando";
    public string animacaoVitima = "Negando";

    [Header("Duração das animações")]
    public float duracaoAssassino = 3f;
    public float duracaoVitima = 3f;

    [Header("Câmera")]
    public Camera cameraCutscene;
    public Transform pontoInicialCamera;
    public Transform pontoFocoFaca;
    public float duracaoFocoCamera = 2f;

    [Header("Easy Transition")]
    public TransitionSettings efeitoTransicao;
    public string nomeProximaCena;
    public float atrasoTransicao = 0f;

    [Header("Configurações")]
    public bool iniciarAutomaticamente = false;
    public bool bloquearRepeticao = true;

    private bool executando;
    private bool concluida;

    private void Start()
    {
        if (iniciarAutomaticamente)
            IniciarCutscene();
    }

    public void IniciarCutscene()
    {
        if (executando)
            return;

        if (bloquearRepeticao && concluida)
            return;

        StartCoroutine(ExecutarCutscene());
    }

    private IEnumerator ExecutarCutscene()
    {
        executando = true;

        if (assassino == null || vitima == null)
        {
            Debug.LogError("Associe os Animators dos personagens.");
            executando = false;
            yield break;
        }

        if (cameraCutscene == null ||
            pontoInicialCamera == null ||
            pontoFocoFaca == null)
        {
            Debug.LogError("Associe a câmera e os pontos de câmera.");
            executando = false;
            yield break;
        }

        if (efeitoTransicao == null)
        {
            Debug.LogError("Associe o TransitionSettings do Easy Transition.");
            executando = false;
            yield break;
        }

        if (string.IsNullOrWhiteSpace(nomeProximaCena))
        {
            Debug.LogError("Informe o nome da próxima cena.");
            executando = false;
            yield break;
        }

        // Guarda as posições e rotações dos personagens.
        Vector3 posicaoAssassino = assassino.transform.position;
        Quaternion rotacaoAssassino = assassino.transform.rotation;

        Vector3 posicaoVitima = vitima.transform.position;
        Quaternion rotacaoVitima = vitima.transform.rotation;

        // Guarda as configurações de Root Motion.
        bool rootMotionAssassino = assassino.applyRootMotion;
        bool rootMotionVitima = vitima.applyRootMotion;

        assassino.applyRootMotion = false;
        vitima.applyRootMotion = false;

        // Posiciona a câmera no ponto inicial.
        cameraCutscene.transform.SetPositionAndRotation(
            pontoInicialCamera.position,
            pontoInicialCamera.rotation
        );

        // Inicia as duas animações ao mesmo tempo.
        assassino.Play(animacaoAssassino, 0, 0f);
        vitima.Play(animacaoVitima, 0, 0f);

        // Aguarda o início dos estados.
        assassino.Update(0f);
        vitima.Update(0f);

        // Espera a animação mais longa terminar.
        float duracaoTotal = Mathf.Max(
            duracaoAssassino,
            duracaoVitima
        );

        yield return new WaitForSeconds(duracaoTotal);

        // Mantém os personagens nas posições originais.
        assassino.transform.SetPositionAndRotation(
            posicaoAssassino,
            rotacaoAssassino
        );

        vitima.transform.SetPositionAndRotation(
            posicaoVitima,
            rotacaoVitima
        );

        // Guarda a posição inicial da câmera.
        Vector3 posicaoInicial = cameraCutscene.transform.position;
        Quaternion rotacaoInicial = cameraCutscene.transform.rotation;

        // Move a câmera suavemente até a faca.
        float tempo = 0f;

        while (tempo < duracaoFocoCamera)
        {
            tempo += Time.deltaTime;

            float progresso = Mathf.Clamp01(
                tempo / duracaoFocoCamera
            );

            progresso = Mathf.SmoothStep(0f, 1f, progresso);

            cameraCutscene.transform.position = Vector3.Lerp(
                posicaoInicial,
                pontoFocoFaca.position,
                progresso
            );

            cameraCutscene.transform.rotation = Quaternion.Slerp(
                rotacaoInicial,
                pontoFocoFaca.rotation,
                progresso
            );

            yield return null;
        }

        // Garante que a câmera chegou ao destino.
        cameraCutscene.transform.SetPositionAndRotation(
            pontoFocoFaca.position,
            pontoFocoFaca.rotation
        );

        // Restaura o Root Motion original.
        assassino.applyRootMotion = rootMotionAssassino;
        vitima.applyRootMotion = rootMotionVitima;

        concluida = true;

        Debug.Log("Câmera focou na faca. Iniciando Easy Transition.");

        // Inicia a transição para a próxima cena.
        TransitionManager.Instance().Transition(
            nomeProximaCena,
            efeitoTransicao,
            atrasoTransicao
        );
    }
}
