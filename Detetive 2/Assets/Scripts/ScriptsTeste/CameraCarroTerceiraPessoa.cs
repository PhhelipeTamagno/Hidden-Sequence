using UnityEngine;

public class CameraCarroTerceiraPessoa : MonoBehaviour
{
    public Transform alvo;

    [Header("Posição")]
    public float distancia = 6f;
    public float altura = 2.5f;

    [Header("Suavidade")]
    public float suavidadePosicao = 6f;
    public float suavidadeRotacao = 5f;

    void LateUpdate()
    {
        if (alvo == null)
            return;

        Vector3 posicaoDesejada =
            alvo.position
            - alvo.forward * distancia
            + Vector3.up * altura;

        transform.position = Vector3.Lerp(
            transform.position,
            posicaoDesejada,
            suavidadePosicao * Time.deltaTime
        );

        Quaternion rotacaoDesejada =
            Quaternion.LookRotation(
                alvo.position + Vector3.up * 1f
                - transform.position
            );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacaoDesejada,
            suavidadeRotacao * Time.deltaTime
        );
    }
}