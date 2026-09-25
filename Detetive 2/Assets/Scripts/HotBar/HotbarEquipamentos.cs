using UnityEngine;
using UnityEngine.UI;

public class HotbarEquipamentos : MonoBehaviour
{
    [Header("Lanterna")]
    public GameObject lanternaNaMao;

    [Header("Destaque do Slot")]
    public GameObject destaque;

    private bool lanternaAtiva = false;

    void Start()
    {
        if (lanternaNaMao != null)
            lanternaNaMao.SetActive(false);

        if (destaque != null)
            destaque.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AlternarLanterna();
        }
    }

    void AlternarLanterna()
    {
        lanternaAtiva = !lanternaAtiva;

        if (lanternaNaMao != null)
            lanternaNaMao.SetActive(lanternaAtiva);

        if (destaque != null)
            destaque.SetActive(lanternaAtiva);
    }

    public bool LanternaEstaAtiva()
    {
        return lanternaAtiva;
    }
}