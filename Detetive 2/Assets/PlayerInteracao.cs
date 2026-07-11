using UnityEngine;

public class PlayerInteracao : MonoBehaviour
{
    [Header("Referências")]
    public Transform cameraTransform;

    [Header("Configuração")]
    public float distanciaInteracao = 4f;
    public LayerMask layerInteragivel;
    public KeyCode teclaInteragir = KeyCode.E;

    private CarroInteragir carroAtual;

    void Update()
    {
        DetectarMira();

        if (carroAtual != null && Input.GetKeyDown(teclaInteragir))
        {
            carroAtual.EntrarCarro(gameObject);
        }
    }

    void DetectarMira()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteracao, layerInteragivel))
        {
            CarroInteragir carro = hit.collider.GetComponentInParent<CarroInteragir>();

            if (carro != null)
            {
                if (carro != carroAtual)
                {
                    if (carroAtual != null)
                        carroAtual.RemoverDestaque();

                    carroAtual = carro;
                    carroAtual.Destacar();
                }
                return;
            }
        }

        if (carroAtual != null)
        {
            carroAtual.RemoverDestaque();
            carroAtual = null;
        }
    }
}