using UnityEngine;

public class LanternaInventario : MonoBehaviour
{
    public GameObject lanternaNaMao;

    private bool lanternaAtiva = false;

    void Start()
    {
        lanternaNaMao.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lanternaAtiva = !lanternaAtiva;
            lanternaNaMao.SetActive(lanternaAtiva);
        }
    }
}