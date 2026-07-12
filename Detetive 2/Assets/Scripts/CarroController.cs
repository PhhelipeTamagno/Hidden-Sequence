using UnityEngine;

public class CarroController : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 10f;
    public float velocidadeRe = 5f;
    public float velocidadeRotacao = 50f;

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        float velAtual = vertical >= 0 ? velocidade : velocidadeRe;

        // Move sempre no eixo local Z (frente do carro)
        transform.Translate(Vector3.forward * vertical * velAtual * Time.deltaTime, Space.Self);

        // Só gira proporcional à velocidade (evita giro tremido/instável)
        float fatorGiro = Mathf.Clamp01(Mathf.Abs(vertical));
        float direcaoGiro = vertical >= 0 ? 1f : -1f;

        transform.Rotate(Vector3.up * horizontal * velocidadeRotacao * fatorGiro * direcaoGiro * Time.deltaTime, Space.Self);
    }
}