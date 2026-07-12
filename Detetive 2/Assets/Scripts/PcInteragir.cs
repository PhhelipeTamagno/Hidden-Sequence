using UnityEngine;

public class PcInteragir : MonoBehaviour
{
    public Material materialNormal;
    public Material materialDestaque;

    private Renderer rend;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("PcInteragir: nenhum Renderer encontrado em " + gameObject.name + " ou seus filhos.");
            return;
        }
        rend.material = materialNormal;
    }

    public void Destacar()
    {
        if (rend != null)
            rend.material = materialDestaque;
    }

    public void RemoverDestaque()
    {
        if (rend != null)
            rend.material = materialNormal;
    }
}