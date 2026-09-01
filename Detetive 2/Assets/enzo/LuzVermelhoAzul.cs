using UnityEngine;

public class LuzVermelhoAzul : MonoBehaviour
{
    public Light luz;

    [Header("Configuração")]
    public float velocidade = 5f;

    void Update()
    {
        // Alterna suavemente entre vermelho e azul
        float valor = Mathf.PingPong(Time.time * velocidade, 1f);

        luz.color = Color.Lerp(Color.red, Color.blue, valor);
    }
}